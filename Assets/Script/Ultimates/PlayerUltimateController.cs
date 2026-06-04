using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public sealed class PlayerUltimateController : MonoBehaviour
{
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private PlayerAim2D playerAim;
    [SerializeField] private PlayerSpellController spellController;
    [SerializeField] private PlayerCultivationState cultivationState;
    [SerializeField] private ProjectileSpawner projectileSpawner;
    [SerializeField] private Projectile2D projectilePrefab;
    [SerializeField] private LayerMask targetLayers;
    [SerializeField] private List<UltimateData> ultimates = new List<UltimateData>();

    private float nextUltimateTime;
    private float nextResolveTime;
    private const float RESOLVE_INTERVAL = 0.5f;

    public bool IsUltimateUnlocked => cultivationState == null || cultivationState.CanUseUltimate;
    public bool IsUltimateReady => IsUltimateUnlocked && Time.time >= nextUltimateTime;
    public float UltimateCooldownRemaining => Mathf.Max(0f, nextUltimateTime - Time.time);
    public float CurrentUltimateCooldown
    {
        get
        {
            UltimateData data = GetCurrentUltimateData();
            return data != null ? data.Cooldown : 1f;
        }
    }
    public UltimateType CurrentUltimateType => GetCurrentUltimateData()?.UltimateType ?? UltimateType.WoodGrandRoots;

    private void Awake()
    {
        ResolveReferences();
        EnsureDefaultUltimates();
    }

    private void Update()
    {
        if (!ReferencesResolved() && Time.time >= nextResolveTime)
        {
            nextResolveTime = Time.time + RESOLVE_INTERVAL;
            ResolveReferences();
        }

        if (Keyboard.current != null && Keyboard.current.rKey.wasPressedThisFrame)
        {
            TryCastUltimate();
        }
    }

    private bool ReferencesResolved()
    {
        return playerStats != null
            && playerAim != null
            && spellController != null
            && cultivationState != null
            && projectileSpawner != null;
    }

    public bool TryCastUltimate()
    {
        UltimateData data = GetCurrentUltimateData();
        if (data == null || !IsUltimateUnlocked || Time.time < nextUltimateTime)
        {
            return false;
        }

        nextUltimateTime = Time.time + data.Cooldown;

        switch (data.UltimateType)
        {
            case UltimateType.WoodGrandRoots:
                CastWoodGrandRoots(data);
                break;
            case UltimateType.FireNova:
                CastFireNova(data);
                break;
            case UltimateType.EarthQuake:
                CastEarthQuake(data);
                break;
            case UltimateType.WaterStorm:
                CastWaterStorm(data);
                break;
            case UltimateType.MetalJudgement:
                CastMetalJudgement(data);
                break;
        }

        return true;
    }

    private void CastWoodGrandRoots(UltimateData data)
    {
        Vector2 center = transform.position;
        SpellVisualEffect2D.CreateUltimateWoodRoots(center, data.Radius, data.PrimaryColor, data.SecondaryColor, 1.1f);

        HashSet<IDamageable> damagedTargets = new HashSet<IDamageable>();
        foreach (Collider2D target in FindTargetsInRadius(center, data.Radius))
        {
            IDamageable damageable = GetUniqueDamageable(target, damagedTargets);
            if (damageable == null)
            {
                continue;
            }

            DamageTarget(damageable, target, GetDamage(data), center);
            EnemyStatus2D status = GetOrCreateEnemyStatus(target);
            if (status != null)
            {
                status.ApplyRoot(data.Duration);
            }
        }
    }

    private void CastFireNova(UltimateData data)
    {
        Vector2 center = GetMouseWorldPosition();
        StartCoroutine(CastFireNovaRoutine(data, center));
    }

    private IEnumerator CastFireNovaRoutine(UltimateData data, Vector2 center)
    {
        SpellVisualEffect2D.CreateUltimateFireWarning(center, data.Radius, data.PrimaryColor, data.SecondaryColor, data.CastDelay);

        if (data.CastDelay > 0f)
        {
            yield return new WaitForSeconds(data.CastDelay);
        }

        SpellVisualEffect2D.CreateUltimateFireNova(center, data.Radius, data.PrimaryColor, data.SecondaryColor, 0.95f);

        HashSet<IDamageable> damagedTargets = new HashSet<IDamageable>();
        foreach (Collider2D target in FindTargetsInRadius(center, data.Radius))
        {
            IDamageable damageable = GetUniqueDamageable(target, damagedTargets);
            if (damageable != null)
            {
                DamageTarget(damageable, target, GetDamage(data), center);
            }
        }
    }

    private void CastEarthQuake(UltimateData data)
    {
        Vector2 center = transform.position;
        SpellVisualEffect2D.CreateUltimateEarthQuake(center, data.Radius, data.PrimaryColor, data.SecondaryColor, 1.05f);

        HashSet<IDamageable> damagedTargets = new HashSet<IDamageable>();
        foreach (Collider2D target in FindTargetsInRadius(center, data.Radius))
        {
            IDamageable damageable = GetUniqueDamageable(target, damagedTargets);
            if (damageable == null)
            {
                continue;
            }

            DamageTarget(damageable, target, GetDamage(data), center);
            EnemyStatus2D status = GetOrCreateEnemyStatus(target);
            if (status != null)
            {
                status.ApplyStun(data.Duration);
            }
        }
    }

    private void CastWaterStorm(UltimateData data)
    {
        Vector2 origin = transform.position;
        Vector2 direction = GetAimDirection();
        SpellVisualEffect2D.CreateUltimateWaterWave(origin, direction, data.RectangleLength, data.RectangleWidth, data.PrimaryColor, data.SecondaryColor, 0.85f);

        HashSet<IDamageable> damagedTargets = new HashSet<IDamageable>();
        foreach (Collider2D target in FindTargetsInRectangle(origin, direction, data.RectangleLength, data.RectangleWidth))
        {
            IDamageable damageable = GetUniqueDamageable(target, damagedTargets);
            if (damageable == null)
            {
                continue;
            }

            DamageTarget(damageable, target, GetDamage(data), origin);
            EnemyStatus2D status = GetOrCreateEnemyStatus(target);
            if (status != null)
            {
                status.ApplyKnockback(direction, data.KnockbackSpeed, data.KnockbackDuration);
            }
        }
    }

    private void CastMetalJudgement(UltimateData data)
    {
        Vector2 center = GetMouseWorldPosition();
        StartCoroutine(CastMetalJudgementRoutine(data, center));
    }

    private IEnumerator CastMetalJudgementRoutine(UltimateData data, Vector2 center)
    {
        SpellVisualEffect2D.CreateUltimateMetalRainWarning(center, data.Radius, data.PrimaryColor, data.SecondaryColor, data.CastDelay);

        if (data.CastDelay > 0f)
        {
            yield return new WaitForSeconds(data.CastDelay);
        }

        SpellVisualEffect2D.CreateUltimateMetalRain(center, data.Radius, data.ProjectileCount, data.PrimaryColor, data.SecondaryColor, 1f);

        int count = Mathf.Max(1, data.ProjectileCount);
        for (int i = 0; i < count; i++)
        {
            Vector2 hitPoint = center + Random.insideUnitCircle * data.Radius;
            SpellVisualEffect2D.CreateCrescent(hitPoint, Vector2.down, data.HitRadius * 0.8f, data.PrimaryColor, 0.32f);
            HashSet<IDamageable> damagedTargets = new HashSet<IDamageable>();
            foreach (Collider2D target in FindTargetsInRadius(hitPoint, data.HitRadius))
            {
                IDamageable damageable = GetUniqueDamageable(target, damagedTargets);
                if (damageable != null)
                {
                    DamageTarget(damageable, target, GetDamage(data) / count, hitPoint);
                }
            }
        }
    }

    private Projectile2D SpawnProjectile(UltimateData data, Vector2 origin, Vector2 direction, float damage, bool forcePierce)
    {
        if (projectileSpawner == null)
        {
            return null;
        }

        return projectileSpawner.SpawnSingle(
            projectilePrefab,
            origin,
            direction,
            damage,
            data.ProjectileSpeed,
            data.ProjectileLifetime,
            gameObject,
            false,
            0f,
            1f,
            ResolveTargetLayers(),
            forcePierce || data.PierceTargets);
    }

    private Collider2D[] FindTargetsInRadius(Vector2 center, float radius)
    {
        return Physics2D.OverlapCircleAll(center, Mathf.Max(0.01f, radius), ResolveTargetLayers());
    }

    private Collider2D[] FindTargetsInRectangle(Vector2 origin, Vector2 direction, float length, float width)
    {
        direction = direction.sqrMagnitude > 0.0001f ? direction.normalized : Vector2.right;
        length = Mathf.Max(0.01f, length);
        width = Mathf.Max(0.01f, width);

        Vector2 center = origin + direction * (length * 0.5f);
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        return Physics2D.OverlapBoxAll(center, new Vector2(length, width), angle, ResolveTargetLayers());
    }

    private IDamageable GetUniqueDamageable(Collider2D target, HashSet<IDamageable> damagedTargets)
    {
        if (target == null || target.transform.root == transform.root)
        {
            return null;
        }

        IDamageable damageable = target.GetComponentInParent<IDamageable>();
        if (damageable == null || damagedTargets.Contains(damageable))
        {
            return null;
        }

        damagedTargets.Add(damageable);
        return damageable;
    }

    private void DamageTarget(IDamageable damageable, Collider2D target, float damage, Vector2 origin)
    {
        Vector2 hitDirection = ((Vector2)target.transform.position - origin).normalized;
        damageable.TakeDamage(new DamageInfo(damage, gameObject, false, 0f, 1f, hitDirection));
    }

    private EnemyStatus2D GetOrCreateEnemyStatus(Collider2D target)
    {
        if (target == null)
        {
            return null;
        }

        EnemyBase enemy = target.GetComponentInParent<EnemyBase>();
        if (enemy == null)
        {
            return null;
        }

        EnemyStatus2D status = enemy.GetComponent<EnemyStatus2D>();
        if (status == null)
        {
            status = enemy.gameObject.AddComponent<EnemyStatus2D>();
        }

        return status;
    }

    private UltimateData GetCurrentUltimateData()
    {
        EnsureDefaultUltimates();
        SpellType linkedSpell = spellController != null ? spellController.CurrentSpell : SpellType.WoodVine;
        return ultimates.Find(ultimate => ultimate.LinkedSpell == linkedSpell);
    }

    private Vector2 GetMouseWorldPosition()
    {
        return playerAim != null ? playerAim.MouseWorldPosition : transform.position;
    }

    private Vector2 GetFireOrigin()
    {
        if (playerAim != null && playerAim.FirePoint != null)
        {
            return playerAim.FirePoint.position;
        }

        return transform.position;
    }

    private Vector2 GetAimDirection()
    {
        if (playerAim != null && playerAim.AimDirection.sqrMagnitude > 0.0001f)
        {
            return playerAim.AimDirection;
        }

        return Vector2.right;
    }

    private float GetDamage(UltimateData data)
    {
        float baseDamage = playerStats != null ? playerStats.BaseDamage : 50f;
        return baseDamage * data.DamageMultiplier;
    }

    private LayerMask ResolveTargetLayers()
    {
        if (targetLayers.value != 0)
        {
            return targetLayers;
        }

        int enemyLayer = LayerMask.NameToLayer("Enemy");
        if (enemyLayer >= 0)
        {
            return 1 << enemyLayer;
        }

        return ~0;
    }

    private void ResolveReferences()
    {
        if (playerStats == null)
        {
            playerStats = GetComponent<PlayerStats>();
        }

        if (playerAim == null)
        {
            playerAim = GetComponent<PlayerAim2D>();
        }

        if (spellController == null)
        {
            spellController = GetComponent<PlayerSpellController>();
        }

        if (cultivationState == null)
        {
            cultivationState = GetComponent<PlayerCultivationState>();
        }

        if (projectileSpawner == null)
        {
            projectileSpawner = GetComponentInChildren<ProjectileSpawner>();
        }

        if (projectileSpawner == null)
        {
#if UNITY_2023_1_OR_NEWER
            projectileSpawner = FindFirstObjectByType<ProjectileSpawner>();
#else
            projectileSpawner = FindObjectOfType<ProjectileSpawner>();
#endif
        }
    }

    private void EnsureDefaultUltimates()
    {
        if (ultimates.Count > 0)
        {
            return;
        }

        ultimates.Add(new UltimateData(UltimateType.WoodGrandRoots, SpellType.WoodVine, 4f, 25f, 8f, 3f, 0f, 0f, 0f, 0f, 0f, 1f, 0f, 0.1f, 1, 0f, false, new Color(0.15f, 1f, 0.35f, 0.95f), new Color(0.05f, 0.55f, 0.18f, 0.95f)));
        ultimates.Add(new UltimateData(UltimateType.FireNova, SpellType.Fireball, 6f, 25f, 5f, 0f, 0.8f, 0f, 0f, 0f, 0f, 1f, 0f, 0.1f, 1, 0f, false, new Color(1f, 0.18f, 0.03f, 0.95f), new Color(1f, 0.72f, 0.08f, 0.95f)));
        ultimates.Add(new UltimateData(UltimateType.EarthQuake, SpellType.EarthSpike, 5f, 25f, 6f, 2f, 0f, 0f, 0f, 0f, 0f, 1f, 0f, 0.1f, 1, 0f, false, new Color(0.95f, 0.66f, 0.18f, 0.95f), new Color(0.42f, 0.24f, 0.08f, 0.95f)));
        ultimates.Add(new UltimateData(UltimateType.WaterStorm, SpellType.WaterArrows, 4.5f, 25f, 0f, 0f, 0f, 15f, 8f, 12f, 0.25f, 1f, 0f, 0.1f, 1, 0f, false, new Color(0.1f, 0.8f, 1f, 0.9f), new Color(0.45f, 1f, 1f, 0.85f)));
        ultimates.Add(new UltimateData(UltimateType.MetalJudgement, SpellType.MetalBlade, 7f, 25f, 10f, 0f, 0.6f, 0f, 0f, 0f, 0f, 1f, 0f, 0.1f, 20, 0f, false, new Color(1f, 0.95f, 0.45f, 0.95f), new Color(0.95f, 0.95f, 1f, 0.9f)));
    }
}
