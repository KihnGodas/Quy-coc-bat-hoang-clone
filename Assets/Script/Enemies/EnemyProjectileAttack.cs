using UnityEngine;
using System.Collections;

public sealed class EnemyProjectileAttack : EnemyAttackBase
{
    [SerializeField] private LayerMask targetLayers;
    [SerializeField] private EnemyMovement movement;

    private bool isCasting;
    private bool isRetreating;

    private void Update()
    {
        ResolveReferences();

        if (enemyBase == null || enemyBase.Data == null || enemyBase.Target == null)
        {
            return;
        }

        EnemyRole role = enemyBase.Role;
        if (role != EnemyRole.RangedProjectile)
        {
            return;
        }

        if (enemyBase.State == EnemyState.Dead || isCasting)
        {
            return;
        }

        EnemyData data = enemyBase.Data;
        float range = data.ProjectileRange > 0f ? data.ProjectileRange : enemyBase.AttackRange;
        float distance = Vector2.Distance(transform.position, enemyBase.Target.position);
        UpdateRetreatState(data, distance, range);

        if (isRetreating || Time.time < nextAttackTime)
        {
            return;
        }

        if (distance > range)
        {
            return;
        }

        StartCoroutine(ShootRoutine(data));
    }

    private void FixedUpdate()
    {
        if (!isRetreating || enemyBase == null || enemyBase.Data == null || enemyBase.Target == null || movement == null)
        {
            return;
        }

        Vector2 currentPosition = transform.position;
        Vector2 awayDirection = (currentPosition - (Vector2)enemyBase.Target.position).normalized;
        if (awayDirection.sqrMagnitude < 0.0001f)
        {
            awayDirection = Vector2.right;
        }

        Vector2 retreatTarget = currentPosition + awayDirection * 2f;
        movement.MoveTowards(
            retreatTarget,
            enemyBase.MoveSpeed,
            enemyBase.Data.RotateToMoveDirection,
            enemyBase.Data.ArenaPadding);
    }

    private IEnumerator ShootRoutine(EnemyData data)
    {
        isCasting = true;
        enemyBase.SetMovementLocked(true);

        SpriteRenderer spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        Sprite sprite = spriteRenderer != null ? spriteRenderer.sprite : null;
        int sortingOrder = spriteRenderer != null ? spriteRenderer.sortingOrder + 3 : 3;
        EnemyTelegraph2D.CreateCircle(
            $"{data.EnemyName}ProjectileCharge",
            sprite,
            transform.position,
            0.75f,
            new Color(1f, 0f, 0f, 0.75f),
            sortingOrder,
            Mathf.Max(data.ProjectileChargeTime, 0.1f),
            transform,
            false,
            true);
        EnemyTelegraph2D aimLine = null;

        float chargeElapsed = 0f;
        while (chargeElapsed < data.ProjectileChargeTime && enemyBase.Target != null && enemyBase.State != EnemyState.Dead)
        {
            chargeElapsed += Time.deltaTime;
            Vector2 start = transform.position;
            Vector2 direction = ((Vector2)enemyBase.Target.position - start).normalized;
            if (direction.sqrMagnitude < 0.0001f)
            {
                direction = Vector2.right;
            }

            if (aimLine == null)
            {
                aimLine = EnemyTelegraph2D.CreateLine(
                    $"{data.EnemyName}ProjectileAim",
                    sprite,
                    start,
                    direction,
                    data.ProjectileRange,
                    0.08f,
                    new Color(1f, 0.05f, 0.05f, 0.55f),
                    sortingOrder,
                    Mathf.Max(data.ProjectileChargeTime, 0.1f));
            }
            else
            {
                aimLine.SetLine(start, direction, data.ProjectileRange, 0.08f);
            }

            yield return null;
        }

        if (enemyBase.Target == null || enemyBase.State == EnemyState.Dead)
        {
            enemyBase.SetMovementLocked(false);
            isCasting = false;
            yield break;
        }

        if (data.ProjectileFlashTime > 0f)
        {
            EnemyTelegraph2D.CreateFlash(
                $"{data.EnemyName}ProjectileFlash",
                sprite,
                transform.position,
                new Vector2(1.25f, 1.25f),
                new Color(1f, 1f, 1f, 0.95f),
                sortingOrder + 1,
                data.ProjectileFlashTime,
                transform,
                true,
                12f);
            yield return new WaitForSeconds(data.ProjectileFlashTime);
        }

        FireProjectile(data);
        enemyBase.SetMovementLocked(false);
        isCasting = false;
    }

    private void FireProjectile(EnemyData data)
    {
        if (data.ProjectilePrefab == null)
        {
            MarkProjectileUsed(data);
            return;
        }

        Vector2 direction = ((Vector2)enemyBase.Target.position - (Vector2)transform.position).normalized;
        if (direction.sqrMagnitude < 0.0001f)
        {
            direction = Vector2.right;
        }

        Vector2 spawnPosition = (Vector2)transform.position + direction * data.ProjectileSpawnOffset;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Projectile2D projectile = Instantiate(data.ProjectilePrefab, spawnPosition, Quaternion.Euler(0f, 0f, angle));
        projectile.SetTargetLayers(ResolveTargetLayers());
        projectile.ConfigurePierce(data.ProjectilePiercing);
        projectile.ConfigurePoison(
            data.ProjectilePoisonDuration,
            data.ProjectilePoisonTickDamage,
            data.ProjectilePoisonTickInterval,
            data.ProjectilePoisonTintColor);
        projectile.Init(direction, data.ProjectileDamage, data.ProjectileSpeed, data.ProjectileLifetime, gameObject);
        MarkProjectileUsed(data);
    }

    private void UpdateRetreatState(EnemyData data, float distance, float range)
    {
        float retreatRange = data.ProjectileRetreatRange > 0f ? data.ProjectileRetreatRange : range * 0.55f;
        float resumeRange = data.ProjectileResumeRange > 0f ? data.ProjectileResumeRange : range * 0.75f;

        if (distance < retreatRange)
        {
            isRetreating = true;
            enemyBase.SetMovementLocked(true);
            return;
        }

        if (isRetreating && distance < resumeRange)
        {
            enemyBase.SetMovementLocked(true);
            return;
        }

        if (isRetreating)
        {
            isRetreating = false;
            enemyBase.SetMovementLocked(false);
        }
    }

    private LayerMask ResolveTargetLayers()
    {
        if (targetLayers.value != 0)
        {
            return targetLayers;
        }

        int playerLayer = LayerMask.NameToLayer("Player");
        if (playerLayer >= 0)
        {
            return 1 << playerLayer;
        }

        return ~0;
    }

    private void MarkProjectileUsed(EnemyData data)
    {
        float cooldown = data.ProjectileCooldown > 0f ? data.ProjectileCooldown : enemyBase.AttackCooldown;
        nextAttackTime = Time.time + cooldown;
    }

    private void OnDisable()
    {
        if (enemyBase != null)
        {
            enemyBase.SetMovementLocked(false);
        }

        isCasting = false;
        isRetreating = false;
    }

    public override void ResolveReferences()
    {
        base.ResolveReferences();

        if (movement == null)
        {
            movement = GetComponent<EnemyMovement>();
        }
    }
}
