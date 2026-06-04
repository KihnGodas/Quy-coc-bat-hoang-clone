using System.Collections;
using UnityEngine;

public sealed class WeaponVisualEffect2D : MonoBehaviour
{
    [SerializeField, Min(0.02f)] private float effectLifetime = 0.18f;
    [SerializeField, Min(0.01f)] private float lineWidth = 0.12f;
    [SerializeField, Min(3)] private int arcSegments = 24;
    [SerializeField, Min(0.1f)] private float projectileVisualLength = 1.6f;
    [SerializeField] private int sortingOrder = 6;
    [SerializeField] private Color normalColor = new Color(0.7f, 0.95f, 1f, 0.85f);
    [SerializeField] private Color skillColor = new Color(1f, 0.65f, 0.2f, 0.9f);

    private Material lineMaterial;

    public void PlayAttack(
        WeaponType weaponType,
        AttackShape shape,
        Vector2 origin,
        Vector2 direction,
        float range,
        float width,
        float angle,
        float radius,
        int projectileCount,
        float spreadAngle,
        bool isSkill)
    {
        direction = direction.sqrMagnitude > 0.0001f ? direction.normalized : Vector2.right;
        Color color = GetColor(weaponType, isSkill);

        if (shape == AttackShape.Circle)
        {
            CreateCircle(origin, Mathf.Max(radius, 0.1f), color);
            return;
        }

        if (shape == AttackShape.Rectangle)
        {
            CreateRectangle(origin, direction, Mathf.Max(range, 0.1f), Mathf.Max(width, lineWidth), color);
            return;
        }

        if (shape == AttackShape.Cone)
        {
            CreateCone(origin, direction, Mathf.Max(range, 0.1f), Mathf.Clamp(angle, 1f, 360f), color);
            return;
        }

        if (shape == AttackShape.Projectile || shape == AttackShape.ProjectileSpread)
        {
            CreateProjectileFan(origin, direction, Mathf.Max(1, projectileCount), spreadAngle, color);
        }
    }

    private void CreateCircle(Vector2 origin, float radius, Color color)
    {
        LineRenderer line = CreateLine("WeaponCircleVisual", color);
        int count = Mathf.Max(arcSegments, 12) + 1;
        line.positionCount = count;
        line.loop = true;

        for (int i = 0; i < count; i++)
        {
            float radians = Mathf.PI * 2f * i / (count - 1);
            Vector2 point = origin + new Vector2(Mathf.Cos(radians), Mathf.Sin(radians)) * radius;
            line.SetPosition(i, point);
        }

        StartCoroutine(FadeAndDestroy(line, color));
    }

    private void CreateRectangle(Vector2 origin, Vector2 direction, float length, float width, Color color)
    {
        Vector2 right = new Vector2(-direction.y, direction.x);
        Vector2 startLeft = origin - right * width * 0.5f;
        Vector2 startRight = origin + right * width * 0.5f;
        Vector2 endLeft = startLeft + direction * length;
        Vector2 endRight = startRight + direction * length;

        LineRenderer line = CreateLine("WeaponRectangleVisual", color);
        line.positionCount = 5;
        line.SetPosition(0, startLeft);
        line.SetPosition(1, startRight);
        line.SetPosition(2, endRight);
        line.SetPosition(3, endLeft);
        line.SetPosition(4, startLeft);

        LineRenderer centerLine = CreateLine("WeaponRectangleCenterVisual", color);
        centerLine.startWidth = lineWidth * 0.65f;
        centerLine.endWidth = lineWidth * 0.65f;
        centerLine.positionCount = 2;
        centerLine.SetPosition(0, origin);
        centerLine.SetPosition(1, origin + direction * length);

        LineRenderer impactLine = CreateLine("WeaponRectangleImpactVisual", color);
        impactLine.startWidth = lineWidth * 0.5f;
        impactLine.endWidth = lineWidth * 0.5f;
        impactLine.positionCount = 2;
        Vector2 impactCenter = origin + direction * length;
        impactLine.SetPosition(0, impactCenter - right * width * 0.35f);
        impactLine.SetPosition(1, impactCenter + right * width * 0.35f);

        StartCoroutine(FadeAndDestroy(line, color));
        StartCoroutine(FadeAndDestroy(centerLine, color));
        StartCoroutine(FadeAndDestroy(impactLine, color));
    }

    private void CreateCone(Vector2 origin, Vector2 direction, float range, float angle, Color color)
    {
        LineRenderer line = CreateLine("WeaponConeVisual", color);
        int segmentCount = Mathf.Max(arcSegments, 8);
        line.positionCount = segmentCount + 3;
        line.SetPosition(0, origin);

        float startAngle = -angle * 0.5f;
        float step = angle / segmentCount;

        for (int i = 0; i <= segmentCount; i++)
        {
            Vector2 pointDirection = Quaternion.Euler(0f, 0f, startAngle + step * i) * direction;
            line.SetPosition(i + 1, origin + pointDirection * range);
        }

        line.SetPosition(segmentCount + 2, origin);
        LineRenderer innerArc = CreateLine("WeaponConeInnerVisual", color);
        innerArc.startWidth = lineWidth * 0.55f;
        innerArc.endWidth = lineWidth * 0.55f;
        innerArc.positionCount = segmentCount + 1;

        for (int i = 0; i <= segmentCount; i++)
        {
            Vector2 pointDirection = Quaternion.Euler(0f, 0f, startAngle + step * i) * direction;
            innerArc.SetPosition(i, origin + pointDirection * range * 0.62f);
        }

        StartCoroutine(FadeAndDestroy(line, color));
        StartCoroutine(FadeAndDestroy(innerArc, color));
    }

    private void CreateProjectileFan(Vector2 origin, Vector2 direction, int count, float spreadAngle, Color color)
    {
        if (count <= 1 || spreadAngle <= 0f)
        {
            CreateProjectileRay(origin, direction, color);
            return;
        }

        float startAngle = -spreadAngle * 0.5f;
        float step = spreadAngle / (count - 1);

        for (int i = 0; i < count; i++)
        {
            Vector2 rayDirection = Quaternion.Euler(0f, 0f, startAngle + step * i) * direction;
            CreateProjectileRay(origin, rayDirection, color);
        }
    }

    private void CreateProjectileRay(Vector2 origin, Vector2 direction, Color color)
    {
        LineRenderer line = CreateLine("WeaponProjectileRayVisual", color);
        line.startWidth = lineWidth * 0.75f;
        line.endWidth = lineWidth * 0.25f;
        line.positionCount = 2;
        line.SetPosition(0, origin);
        line.SetPosition(1, origin + direction.normalized * projectileVisualLength);
        LineRenderer glowLine = CreateLine("WeaponProjectileGlowVisual", color);
        glowLine.startWidth = lineWidth * 1.2f;
        glowLine.endWidth = lineWidth * 0.1f;
        glowLine.positionCount = 2;
        glowLine.SetPosition(0, origin - direction.normalized * 0.15f);
        glowLine.SetPosition(1, origin + direction.normalized * projectileVisualLength * 0.55f);
        StartCoroutine(FadeAndDestroy(line, color));
        StartCoroutine(FadeAndDestroy(glowLine, color));
    }

    private LineRenderer CreateLine(string effectName, Color color)
    {
        GameObject effectObject = new GameObject(effectName);
        effectObject.transform.position = Vector3.zero;

        LineRenderer line = effectObject.AddComponent<LineRenderer>();
        line.useWorldSpace = true;
        line.material = GetLineMaterial();
        line.startColor = color;
        line.endColor = color;
        line.startWidth = lineWidth;
        line.endWidth = lineWidth;
        line.numCapVertices = 4;
        line.numCornerVertices = 4;
        line.sortingOrder = sortingOrder;
        return line;
    }

    private IEnumerator FadeAndDestroy(LineRenderer line, Color startColor)
    {
        float startTime = Time.time;

        while (line != null)
        {
            float progress = Mathf.Clamp01((Time.time - startTime) / effectLifetime);
            Color color = startColor;
            color.a = Mathf.Lerp(startColor.a, 0f, progress);
            line.startColor = color;
            line.endColor = color;

            if (progress >= 1f)
            {
                Destroy(line.gameObject);
                yield break;
            }

            yield return null;
        }
    }

    private Color GetColor(WeaponType weaponType, bool isSkill)
    {
        if (isSkill)
        {
            return skillColor;
        }

        if (weaponType == WeaponType.Axe)
        {
            return new Color(1f, 0.45f, 0.2f, normalColor.a);
        }

        if (weaponType == WeaponType.Spear)
        {
            return new Color(1f, 0.9f, 0.45f, normalColor.a);
        }

        if (weaponType == WeaponType.FlyingSword)
        {
            return new Color(0.55f, 0.85f, 1f, normalColor.a);
        }

        return normalColor;
    }

    private Material GetLineMaterial()
    {
        if (lineMaterial != null)
        {
            return lineMaterial;
        }

        Shader shader = Shader.Find("Sprites/Default");
        lineMaterial = new Material(shader);
        return lineMaterial;
    }
}
