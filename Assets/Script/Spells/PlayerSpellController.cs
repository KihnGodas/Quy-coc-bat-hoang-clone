using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public sealed class PlayerSpellController : MonoBehaviour
{
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private PlayerAim2D playerAim;
    [SerializeField] private ProjectileSpawner projectileSpawner;
    [SerializeField] private Projectile2D projectilePrefab;
    [SerializeField] private LayerMask targetLayers;
    [SerializeField] private SpellType currentSpell = SpellType.WoodVine;
    [SerializeField] private bool allowDebugSpellHotkeys = true;
    [SerializeField] private List<SpellData> spells = new List<SpellData>();

    private readonly Dictionary<SpellType, float> nextCastTimes = new Dictionary<SpellType, float>();
    private Coroutine waterRoutine;

    public SpellType CurrentSpell => currentSpell;

    public float GetCurrentSpellCooldownRemaining()
    {
        SpellData data = GetSpellData(currentSpell);
        if (data == null) return 0f;
        return Mathf.Max(0f, GetNextCastTime(currentSpell) - Time.time);
    }

    public float GetCurrentSpellTotalCooldown()
    {
        SpellData data = GetSpellData(currentSpell);
        return data != null ? data.Cooldown : 0f;
    }

    public SpellData GetCurrentSpellData()
    {
        return GetSpellData(currentSpell);
    }

    public bool CanCastCurrentSpell()
    {
        SpellData data = GetSpellData(currentSpell);
        return data != null && Time.time >= GetNextCastTime(data.SpellType);
    }

    public float GetCurrentSpellCooldownRemainingRaw()
    {
        SpellData data = GetSpellData(currentSpell);
        if (data == null) return 0f;
        return GetNextCastTime(currentSpell) - Time.time;
    }

    private void Awake()
    {
        ResolveReferences();
        EnsureDefaultSpells();
    }

    private void Update()
    {
        ResolveReferences();
        HandleDebugSelectionInput();

        if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            TryCastCurrentSpell();
        }
    }

    public bool TryCastCurrentSpell()
    {
        SpellData data = GetSpellData(currentSpell);
        if (data == null || Time.time < GetNextCastTime(data.SpellType))
        {
            return false;
        }

        nextCastTimes[data.SpellType] = Time.time + data.Cooldown;

        switch (data.SpellType)
        {
            case SpellType.WoodVine:
                CastWoodVine(data);
                break;
            case SpellType.Fireball:
                CastFireball(data);
                break;
            case SpellType.EarthSpike:
                CastEarthSpike(data);
                break;
            case SpellType.WaterArrows:
                CastWaterArrows(data);
                break;
            case SpellType.MetalBlade:
                CastMetalBlade(data);
                break;
        }

        return true;
    }

    public void SelectSpell(SpellType spellType)
    {
        currentSpell = spellType;
    }

    private void CastWoodVine(SpellData data)
    {
        Vector2 center = GetMouseWorldPosition();
        float damage = GetDamage(data);
        SpellVisualEffect2D.CreateVines(center, data.Radius, data.VisualColor, 0.55f);
        HashSet<IDamageable> damagedTargets = new HashSet<IDamageable>();

        foreach (Collider2D target in FindTargetsInRadius(center, data.Radius))
        {
            IDamageable damageable = GetUniqueDamageable(target, damagedTargets);
            if (damageable == null)
            {
                continue;
            }

            DamageTarget(damageable, target, damage, center);
            EnemyStatus2D status = GetOrCreateEnemyStatus(target);
            if (status != null)
            {
                status.ApplyRoot(data.Duration);
            }
        }
    }

    private void CastFireball(SpellData data)
    {
        Vector2 origin = GetFireOrigin();
        Vector2 direction = GetAimDirection();
        Projectile2D projectile = SpawnProjectile(data, origin, direction, GetDamage(data), false);
        if (projectile != null)
        {
            projectile.ConfigureImpactEffect(new Color(1f, 0.35f, 0.05f, 0.9f), 1.35f, 0.22f);
        }

        SpellVisualEffect2D.CreateBurst("Fireball_Cast_Burst", origin, 0.75f, 10, data.VisualColor, 0.22f, 0.1f, 34);
        SpellVisualEffect2D.CreateProjectileLaunch("Fireball_Trail", origin, direction, 1.4f, data.VisualColor, 0.25f, 0.16f);
    }

    private void CastEarthSpike(SpellData data)
    {
        Vector2 center = GetMouseWorldPosition();
        SpellVisualEffect2D.CreateSpikes(center, data.Radius, data.VisualColor, 0.5f);

        float damage = GetDamage(data);
        HashSet<IDamageable> damagedTargets = new HashSet<IDamageable>();
        foreach (Collider2D target in FindTargetsInRadius(center, data.Radius))
        {
            IDamageable damageable = GetUniqueDamageable(target, damagedTargets);
            if (damageable != null)
            {
                DamageTarget(damageable, target, damage, center);
            }
        }
    }

    private void CastWaterArrows(SpellData data)
    {
        if (waterRoutine != null)
        {
            StopCoroutine(waterRoutine);
        }

        waterRoutine = StartCoroutine(CastWaterArrowsRoutine(data));
    }

    private IEnumerator CastWaterArrowsRoutine(SpellData data)
    {
        int count = Mathf.Max(1, data.ProjectileCount);
        for (int i = 0; i < count; i++)
        {
            Vector2 origin = GetFireOrigin();
            Vector2 direction = GetAimDirection();
            float offsetAngle = count > 1 ? Mathf.Lerp(-data.SpreadAngle * 0.5f, data.SpreadAngle * 0.5f, count == 1 ? 0.5f : (float)i / (count - 1)) : 0f;
            Vector2 shotDirection = Quaternion.Euler(0f, 0f, offsetAngle) * direction;
            Projectile2D projectile = SpawnProjectile(data, origin, shotDirection, GetDamage(data), false);
            if (projectile != null)
            {
                projectile.ConfigureImpactEffect(new Color(0.25f, 0.75f, 1f, 0.9f), 0.95f, 0.18f);
            }

            SpellVisualEffect2D.CreateProjectileLaunch("WaterArrow_Shot", origin, shotDirection, 1f, data.VisualColor, 0.18f, 0.09f);

            if (data.ProjectileDelay > 0f && i < count - 1)
            {
                yield return new WaitForSeconds(data.ProjectileDelay);
            }
        }

        waterRoutine = null;
    }

    private void CastMetalBlade(SpellData data)
    {
        Vector2 origin = GetFireOrigin();
        Vector2 direction = GetAimDirection();
        Projectile2D projectile = SpawnProjectile(data, origin, direction, GetDamage(data), true);
        if (projectile != null)
        {
            projectile.ConfigureImpactEffect(new Color(1f, 0.92f, 0.45f, 0.9f), 1.05f, 0.18f);
        }

        SpellVisualEffect2D.CreateCrescent(origin + direction * 0.35f, direction, 0.8f, data.VisualColor, 0.28f);
        SpellVisualEffect2D.CreateProjectileLaunch("MetalBlade_Line", origin, direction, 1.6f, data.VisualColor, 0.22f, 0.08f);
    }

    private Projectile2D SpawnProjectile(SpellData data, Vector2 origin, Vector2 direction, float damage, bool forcePierce)
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

    private Vector2 GetMouseWorldPosition()
    {
        if (playerAim != null)
        {
            return playerAim.MouseWorldPosition;
        }

        return transform.position;
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

    private float GetDamage(SpellData data)
    {
        float baseDamage = playerStats != null ? playerStats.BaseDamage : 50f;
        return baseDamage * data.DamageMultiplier;
    }

    private float GetNextCastTime(SpellType spellType)
    {
        return nextCastTimes.TryGetValue(spellType, out float nextTime) ? nextTime : 0f;
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

    private SpellData GetSpellData(SpellType spellType)
    {
        EnsureDefaultSpells();
        return spells.Find(spell => spell.SpellType == spellType);
    }

    private void HandleDebugSelectionInput()
    {
        if (!allowDebugSpellHotkeys || Keyboard.current == null)
        {
            return;
        }

        if (Keyboard.current.digit1Key.wasPressedThisFrame)
        {
            SelectSpell(SpellType.WoodVine);
        }
        else if (Keyboard.current.digit2Key.wasPressedThisFrame)
        {
            SelectSpell(SpellType.Fireball);
        }
        else if (Keyboard.current.digit3Key.wasPressedThisFrame)
        {
            SelectSpell(SpellType.EarthSpike);
        }
        else if (Keyboard.current.digit4Key.wasPressedThisFrame)
        {
            SelectSpell(SpellType.WaterArrows);
        }
        else if (Keyboard.current.digit5Key.wasPressedThisFrame)
        {
            SelectSpell(SpellType.MetalBlade);
        }
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

    private void EnsureDefaultSpells()
    {
        if (spells.Count > 0)
        {
            return;
        }

        spells.Add(new SpellData(SpellType.WoodVine, 1.3f, 9f, 2.4f, 1.5f, 0f, 0.1f, 1, 0f, 0f, false, new Color(0.2f, 1f, 0.35f, 0.9f)));
        spells.Add(new SpellData(SpellType.Fireball, 2.3f, 8f, 0f, 0f, 15f, 1.4f, 1, 0f, 0f, false, new Color(1f, 0.28f, 0.05f, 0.95f)));
        spells.Add(new SpellData(SpellType.EarthSpike, 1.8f, 9f, 2.5f, 0f, 0f, 0.1f, 1, 0f, 0f, false, new Color(0.9f, 0.62f, 0.2f, 0.95f)));
        spells.Add(new SpellData(SpellType.WaterArrows, 0.75f, 7f, 0f, 0f, 14f, 1.3f, 3, 0.12f, 8f, false, new Color(0.15f, 0.78f, 1f, 0.9f)));
        spells.Add(new SpellData(SpellType.MetalBlade, 1.7f, 8f, 0f, 0f, 16f, 1.4f, 1, 0f, 0f, true, new Color(1f, 0.95f, 0.45f, 0.95f)));
    }
}
