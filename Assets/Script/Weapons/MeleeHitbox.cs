using System.Collections.Generic;
using UnityEngine;

public sealed class MeleeHitbox : MonoBehaviour
{
    [SerializeField] private LayerMask targetLayers;
    [SerializeField] private bool drawDebugGizmos = true;
    [SerializeField] private Color gizmoColor = new Color(1f, 0.25f, 0.15f, 0.75f);

    private AttackShape lastShape;
    private Vector2 lastOrigin;
    private Vector2 lastDirection = Vector2.right;
    private float lastRange;
    private float lastWidth;
    private float lastAngle;
    private float lastRadius;
    private float lastGizmoEndTime;

    public int ApplyAttack(
        AttackShape shape,
        Vector2 origin,
        Vector2 direction,
        float range,
        float width,
        float angle,
        float radius,
        DamageInfo damageInfo)
    {
        float overlapRadius = shape == AttackShape.Circle ? radius : Mathf.Max(range, width);
        Collider2D[] candidates = Physics2D.OverlapCircleAll(origin, overlapRadius, targetLayers);
        HashSet<IDamageable> damagedTargets = new HashSet<IDamageable>();
        int hitCount = 0;

        foreach (Collider2D candidate in candidates)
        {
            if (damageInfo.source != null && candidate.transform.root == damageInfo.source.transform.root)
            {
                continue;
            }

            if (!IsColliderInsideAttack(candidate, shape, origin, direction, range, width, angle, radius))
            {
                continue;
            }

            IDamageable damageable = candidate.GetComponentInParent<IDamageable>();
            if (damageable == null || damagedTargets.Contains(damageable))
            {
                continue;
            }

            DamageInfo targetDamageInfo = damageInfo;
            targetDamageInfo.hitDirection = ((Vector2)candidate.transform.position - origin).normalized;
            damageable.TakeDamage(targetDamageInfo);
            damagedTargets.Add(damageable);
            hitCount++;
        }

        CacheGizmo(shape, origin, direction, range, width, angle, radius);
        return hitCount;
    }

    private static bool IsColliderInsideAttack(
        Collider2D collider,
        AttackShape shape,
        Vector2 origin,
        Vector2 direction,
        float range,
        float width,
        float angle,
        float radius)
    {
        Bounds bounds = collider.bounds;
        Vector2 center = bounds.center;
        if (WeaponAttackShape.Contains(shape, origin, direction, center, range, width, angle, radius))
        {
            return true;
        }

        if (WeaponAttackShape.Contains(shape, origin, direction, collider.ClosestPoint(origin), range, width, angle, radius))
        {
            return true;
        }

        Vector2 shapeCenter = GetShapeCenter(shape, origin, direction, range, radius);
        if (WeaponAttackShape.Contains(shape, origin, direction, collider.ClosestPoint(shapeCenter), range, width, angle, radius))
        {
            return true;
        }

        Vector2 min = bounds.min;
        Vector2 max = bounds.max;
        return WeaponAttackShape.Contains(shape, origin, direction, new Vector2(min.x, min.y), range, width, angle, radius)
            || WeaponAttackShape.Contains(shape, origin, direction, new Vector2(min.x, max.y), range, width, angle, radius)
            || WeaponAttackShape.Contains(shape, origin, direction, new Vector2(max.x, min.y), range, width, angle, radius)
            || WeaponAttackShape.Contains(shape, origin, direction, new Vector2(max.x, max.y), range, width, angle, radius);
    }

    private static Vector2 GetShapeCenter(AttackShape shape, Vector2 origin, Vector2 direction, float range, float radius)
    {
        if (shape == AttackShape.Circle)
        {
            return origin;
        }

        Vector2 normalizedDirection = direction.sqrMagnitude > 0.0001f ? direction.normalized : Vector2.right;
        float forwardDistance = shape == AttackShape.Cone ? range * 0.65f : range * 0.5f;
        return origin + normalizedDirection * Mathf.Max(forwardDistance, radius);
    }

    private void CacheGizmo(AttackShape shape, Vector2 origin, Vector2 direction, float range, float width, float angle, float radius)
    {
        lastShape = shape;
        lastOrigin = origin;
        lastDirection = direction.sqrMagnitude > 0.0001f ? direction.normalized : Vector2.right;
        lastRange = range;
        lastWidth = width;
        lastAngle = angle;
        lastRadius = radius;
        lastGizmoEndTime = Time.time + 0.25f;
    }

    private void OnDrawGizmos()
    {
        if (!drawDebugGizmos || Application.isPlaying && Time.time > lastGizmoEndTime)
        {
            return;
        }

        Gizmos.color = gizmoColor;
        WeaponAttackShape.DrawGizmo(lastShape, lastOrigin, lastDirection, lastRange, lastWidth, lastAngle, lastRadius);
    }
}
