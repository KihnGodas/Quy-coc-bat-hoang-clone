using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class EnemyRootAttack : EnemyAttackBase
{
    private bool isCasting;

    private void Update()
    {
        ResolveReferences();

        if (enemyBase == null || enemyBase.Data == null || enemyBase.Role != EnemyRole.RootMage)
        {
            return;
        }

        if (enemyBase.Target == null || enemyBase.State == EnemyState.Dead || isCasting || Time.time < nextAttackTime)
        {
            return;
        }

        EnemyData data = enemyBase.Data;
        float distance = Vector2.Distance(transform.position, enemyBase.Target.position);
        if (distance > data.RootRange)
        {
            enemyBase.SetMovementLocked(false);
            return;
        }

        enemyBase.SetMovementLocked(true);
        StartCoroutine(RootRoutine(data, enemyBase.Target));
    }

    private IEnumerator RootRoutine(EnemyData data, Transform target)
    {
        isCasting = true;
        enemyBase.SetMovementLocked(true);
        Vector2 targetPosition = target.position;
        SpriteRenderer spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        Sprite sprite = spriteRenderer != null ? spriteRenderer.sprite : null;
        int sortingOrder = spriteRenderer != null ? spriteRenderer.sortingOrder + 3 : 3;
        EnemyTelegraph2D.CreateCircle(
            "MocYeuRootWarning",
            sprite,
            targetPosition,
            data.RootRadius,
            new Color(0.38f, 0.95f, 0.24f, 0.38f),
            sortingOrder,
            Mathf.Max(data.RootWarningTime, 0.1f),
            null,
            false);

        if (data.RootWarningTime > 0f)
        {
            yield return new WaitForSeconds(data.RootWarningTime);
        }

        CreateRootBurst(targetPosition, data, sprite, sortingOrder);
        ApplyRoot(targetPosition, data);
        StartCoroutine(RootTrapRoutine(targetPosition, data));

        if (data.RootDuration > 0f)
        {
            yield return new WaitForSeconds(data.RootDuration);
        }

        nextAttackTime = Time.time + data.RootCooldown;
        isCasting = false;
        enemyBase.SetMovementLocked(false);
    }

    private void ApplyRoot(Vector2 center, EnemyData data)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(center, data.RootRadius);
        HashSet<IDamageable> damagedTargets = new HashSet<IDamageable>();
        for (int i = 0; i < hits.Length; i++)
        {
            Collider2D hit = hits[i];
            if (hit == null || !hit.transform.root.CompareTag(targetTag))
            {
                continue;
            }

            PlayerStatus2D status = hit.GetComponentInParent<PlayerStatus2D>();
            if (status == null)
            {
                status = hit.transform.root.gameObject.AddComponent<PlayerStatus2D>();
            }

            status.ApplyRoot(data.RootDuration);

            if (data.RootDamage > 0f)
            {
                IDamageable damageable = hit.GetComponentInParent<IDamageable>();
                if (damageable != null && !damagedTargets.Contains(damageable))
                {
                    Vector2 hitDirection = ((Vector2)hit.transform.position - center).normalized;
                    damageable.TakeDamage(new DamageInfo(data.RootDamage, gameObject, false, 0f, 1f, hitDirection));
                    damagedTargets.Add(damageable);
                }
            }
        }
    }

    private IEnumerator RootTrapRoutine(Vector2 center, EnemyData data)
    {
        float trapDuration = Mathf.Max(data.RootDuration, 0.01f);
        float endTime = Time.time + trapDuration;
        float trapDamage = data.RootDamage * data.RootTrapDamageMultiplier;
        HashSet<Transform> damagedRoots = new HashSet<Transform>();

        while (Time.time < endTime && enemyBase != null && enemyBase.State != EnemyState.Dead)
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll(center, data.RootRadius);
            for (int i = 0; i < hits.Length; i++)
            {
                Collider2D hit = hits[i];
                if (hit == null || !hit.transform.root.CompareTag(targetTag) || damagedRoots.Contains(hit.transform.root))
                {
                    continue;
                }

                IDamageable damageable = hit.GetComponentInParent<IDamageable>();
                if (damageable != null && trapDamage > 0f)
                {
                    Vector2 hitDirection = ((Vector2)hit.transform.position - center).normalized;
                    damageable.TakeDamage(new DamageInfo(trapDamage, gameObject, false, 0f, 1f, hitDirection));
                    damagedRoots.Add(hit.transform.root);
                }
            }

            yield return null;
        }
    }

    private void CreateRootBurst(Vector2 position, EnemyData data, Sprite sprite, int sortingOrder)
    {
        EnemyTelegraph2D.CreateCircle(
            "MocYeuRootBurst",
            sprite,
            position,
            data.RootRadius,
            new Color(0.22f, 0.82f, 0.18f, 0.5f),
            sortingOrder + 1,
            Mathf.Max(data.RootDuration, 0.1f),
            null,
            true);
    }

    private void OnDisable()
    {
        if (enemyBase != null)
        {
            enemyBase.SetMovementLocked(false);
        }

        isCasting = false;
    }

    private void OnDrawGizmosSelected()
    {
        if (enemyBase == null || enemyBase.Data == null)
        {
            return;
        }

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, enemyBase.Data.RootRange);
    }
}
