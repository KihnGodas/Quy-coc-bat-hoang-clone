using UnityEngine;

public sealed class ProjectileSpawner : MonoBehaviour
{
    [SerializeField] private Projectile2D defaultProjectilePrefab;
    [SerializeField] private BoomerangProjectile2D defaultBoomerangProjectilePrefab;
    [SerializeField] private LayerMask defaultTargetLayers;

    public Projectile2D SpawnSingle(
        Projectile2D projectilePrefab,
        Vector2 position,
        Vector2 direction,
        float damage,
        float speed,
        float lifetime,
        GameObject source,
        bool canCrit,
        float critChance,
        float critMultiplier,
        LayerMask targetLayers,
        bool pierceTargets = false)
    {
        Projectile2D prefab = projectilePrefab != null ? projectilePrefab : defaultProjectilePrefab;
        if (prefab == null)
        {
            return null;
        }

        direction = direction.sqrMagnitude > 0.0001f ? direction.normalized : Vector2.right;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Projectile2D projectile = Instantiate(prefab, position, Quaternion.Euler(0f, 0f, angle));
        projectile.SetTargetLayers(targetLayers.value != 0 ? targetLayers : defaultTargetLayers);
        projectile.ConfigureCrit(canCrit, critChance, critMultiplier);
        projectile.ConfigurePierce(pierceTargets);
        projectile.Init(direction, damage, speed, lifetime, source);
        return projectile;
    }

    public void SpawnPattern(
        ProjectilePattern pattern,
        Projectile2D projectilePrefab,
        BoomerangProjectile2D boomerangProjectilePrefab,
        Vector2 position,
        Vector2 direction,
        int count,
        float spreadAngle,
        float damage,
        float speed,
        float lifetime,
        GameObject source,
        bool canCrit,
        float critChance,
        float critMultiplier,
        LayerMask targetLayers,
        bool pierceTargets = false,
        float boomerangReturnDelay = 0.45f,
        Transform boomerangReturnTarget = null)
    {
        switch (pattern)
        {
            case ProjectilePattern.Spread:
                SpawnSpread(projectilePrefab, position, direction, count, spreadAngle, damage, speed, lifetime, source, canCrit, critChance, critMultiplier, targetLayers, pierceTargets);
                break;
            case ProjectilePattern.Radial:
                SpawnRadial(projectilePrefab, position, count, damage, speed, lifetime, source, canCrit, critChance, critMultiplier, targetLayers, pierceTargets);
                break;
            case ProjectilePattern.Boomerang:
                SpawnBoomerang(boomerangProjectilePrefab, projectilePrefab, position, direction, damage, speed, lifetime, source, canCrit, critChance, critMultiplier, targetLayers, boomerangReturnDelay, boomerangReturnTarget);
                break;
            default:
                SpawnSingle(projectilePrefab, position, direction, damage, speed, lifetime, source, canCrit, critChance, critMultiplier, targetLayers, pierceTargets);
                break;
        }
    }

    public BoomerangProjectile2D SpawnBoomerang(
        BoomerangProjectile2D boomerangProjectilePrefab,
        Projectile2D fallbackProjectilePrefab,
        Vector2 position,
        Vector2 direction,
        float damage,
        float speed,
        float lifetime,
        GameObject source,
        bool canCrit,
        float critChance,
        float critMultiplier,
        LayerMask targetLayers,
        float returnDelay,
        Transform returnTarget = null)
    {
        BoomerangProjectile2D prefab = ResolveBoomerangPrefab(boomerangProjectilePrefab, fallbackProjectilePrefab);
        if (prefab == null)
        {
            return null;
        }

        direction = NormalizeDirection(direction);
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        BoomerangProjectile2D projectile = Instantiate(prefab, position, Quaternion.Euler(0f, 0f, angle));
        projectile.SetTargetLayers(targetLayers.value != 0 ? targetLayers : defaultTargetLayers);
        projectile.ConfigureCrit(canCrit, critChance, critMultiplier);
        projectile.ConfigureReturn(returnDelay, returnTarget != null ? returnTarget : source != null ? source.transform : null);
        projectile.Init(direction, damage, speed, lifetime, source);
        return projectile;
    }

    public void SpawnSpread(
        Projectile2D projectilePrefab,
        Vector2 position,
        Vector2 direction,
        int count,
        float spreadAngle,
        float damage,
        float speed,
        float lifetime,
        GameObject source,
        bool canCrit,
        float critChance,
        float critMultiplier,
        LayerMask targetLayers,
        bool pierceTargets = false)
    {
        count = Mathf.Max(1, count);
        direction = NormalizeDirection(direction);

        if (count == 1 || spreadAngle <= 0f)
        {
            SpawnSingle(projectilePrefab, position, direction, damage, speed, lifetime, source, canCrit, critChance, critMultiplier, targetLayers, pierceTargets);
            return;
        }

        float startAngle = -spreadAngle * 0.5f;
        float step = spreadAngle / (count - 1);

        for (int i = 0; i < count; i++)
        {
            float angle = startAngle + step * i;
            Vector2 shotDirection = Quaternion.Euler(0f, 0f, angle) * direction;
            SpawnSingle(projectilePrefab, position, shotDirection, damage, speed, lifetime, source, canCrit, critChance, critMultiplier, targetLayers, pierceTargets);
        }
    }

    public void SpawnRadial(
        Projectile2D projectilePrefab,
        Vector2 position,
        int count,
        float damage,
        float speed,
        float lifetime,
        GameObject source,
        bool canCrit,
        float critChance,
        float critMultiplier,
        LayerMask targetLayers,
        bool pierceTargets = false)
    {
        count = Mathf.Max(1, count);
        float step = 360f / count;

        for (int i = 0; i < count; i++)
        {
            Vector2 direction = Quaternion.Euler(0f, 0f, step * i) * Vector2.right;
            SpawnSingle(projectilePrefab, position, direction, damage, speed, lifetime, source, canCrit, critChance, critMultiplier, targetLayers, pierceTargets);
        }
    }

    private BoomerangProjectile2D ResolveBoomerangPrefab(BoomerangProjectile2D boomerangProjectilePrefab, Projectile2D fallbackProjectilePrefab)
    {
        if (boomerangProjectilePrefab != null)
        {
            return boomerangProjectilePrefab;
        }

        if (defaultBoomerangProjectilePrefab != null)
        {
            return defaultBoomerangProjectilePrefab;
        }

        if (fallbackProjectilePrefab is BoomerangProjectile2D fallbackBoomerangPrefab)
        {
            return fallbackBoomerangPrefab;
        }

        return defaultProjectilePrefab as BoomerangProjectile2D;
    }

    private static Vector2 NormalizeDirection(Vector2 direction)
    {
        return direction.sqrMagnitude > 0.0001f ? direction.normalized : Vector2.right;
    }
}
