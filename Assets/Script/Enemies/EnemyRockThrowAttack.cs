using System.Collections;
using UnityEngine;

public sealed class EnemyRockThrowAttack : EnemyAttackBase
{
    private bool isThrowing;

    private void Update()
    {
        ResolveReferences();

        if (enemyBase == null || enemyBase.Data == null || enemyBase.Role != EnemyRole.HybridThrower)
        {
            return;
        }

        if (enemyBase.Target == null || enemyBase.State == EnemyState.Dead)
        {
            enemyBase.SetMovementLocked(false);
            return;
        }

        EnemyData data = enemyBase.Data;
        float range = data.ProjectileRange > 0f ? data.ProjectileRange : 8f;
        float distance = Vector2.Distance(transform.position, enemyBase.Target.position);
        if (distance > range)
        {
            if (!isThrowing)
            {
                enemyBase.SetMovementLocked(false);
            }

            return;
        }

        enemyBase.SetMovementLocked(true);
        if (!isThrowing && Time.time >= nextAttackTime)
        {
            StartCoroutine(ThrowRoutine(data, enemyBase.Target.position));
        }
    }

    private IEnumerator ThrowRoutine(EnemyData data, Vector2 targetSnapshot)
    {
        isThrowing = true;
        enemyBase.SetMovementLocked(true);

        SpriteRenderer spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        Sprite sprite = spriteRenderer != null ? spriteRenderer.sprite : null;
        int sortingOrder = spriteRenderer != null ? spriteRenderer.sortingOrder + 3 : 3;

        EnemyTelegraph2D.CreateCircle(
            "KimVienRockAoEWarning",
            sprite,
            targetSnapshot,
            data.RockRadius,
            new Color(1f, 0f, 0f, 0.65f),
            sortingOrder,
            Mathf.Max(data.RockWarningTime + data.RockFallTime, 0.1f),
            null,
            false);

        if (data.RockWarningTime > 0f)
        {
            yield return new WaitForSeconds(data.RockWarningTime);
        }

        EnemyTelegraph2D.CreateArc(
            "KimVienRockArc",
            sprite,
            (Vector2)transform.position + Vector2.up * 0.45f,
            targetSnapshot,
            new Vector2(data.RockRadius * 0.65f, data.RockRadius * 0.65f),
            Mathf.Max(data.RockRadius * 2f, 1.8f),
            new Color(0.35f, 0.35f, 0.35f, 0.95f),
            sortingOrder + 1,
            Mathf.Max(data.RockFallTime, 0.1f));

        if (data.RockFallTime > 0f)
        {
            yield return new WaitForSeconds(data.RockFallTime);
        }

        EnemyTelegraph2D.CreateFlash(
            "KimVienRockImpact",
            sprite,
            targetSnapshot,
            new Vector2(data.RockRadius * 2f, data.RockRadius * 2f),
            new Color(1f, 0.7f, 0.2f, 0.85f),
            sortingOrder + 2,
            0.2f);

        DamageTargets(targetSnapshot, data.RockRadius, data.ProjectileDamage);
        nextAttackTime = Time.time + data.ProjectileCooldown;
        isThrowing = false;
    }

    private void DamageTargets(Vector2 center, float radius, float damage)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(center, radius);
        for (int i = 0; i < hits.Length; i++)
        {
            Collider2D hit = hits[i];
            if (hit == null || !hit.transform.root.CompareTag(targetTag))
            {
                continue;
            }

            IDamageable damageable = hit.GetComponentInParent<IDamageable>();
            if (damageable == null)
            {
                continue;
            }

            Vector2 hitDirection = ((Vector2)hit.transform.position - center).normalized;
            damageable.TakeDamage(new DamageInfo(damage, gameObject, false, 0f, 1f, hitDirection));
        }
    }

    private void OnDisable()
    {
        if (enemyBase != null)
        {
            enemyBase.SetMovementLocked(false);
        }

        isThrowing = false;
    }
}
