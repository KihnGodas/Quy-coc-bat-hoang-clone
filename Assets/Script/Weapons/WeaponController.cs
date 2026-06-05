using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public sealed class WeaponController : MonoBehaviour
{
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private PlayerAim2D playerAim;
    [SerializeField] private PlayerMovement2D playerMovement;
    [SerializeField] private MeleeHitbox meleeHitbox;
    [SerializeField] private WeaponVisualEffect2D weaponVisualEffect;
    [SerializeField] private WeaponSpriteVisual2D weaponSpriteVisual;
    [SerializeField] private ProjectileSpawner projectileSpawner;
    [SerializeField] private MuzzleFlash2D muzzleFlash;
    [SerializeField] private Projectile2D projectilePrefab;
    [SerializeField] private LayerMask targetLayers;
    [SerializeField, Min(0f)] private float meleeOriginForwardOffset = 0.45f;
    [SerializeField] private bool useAutomaticNormalAttack = true;
    [SerializeField, Min(0f)] private float autoAttackRangePadding = 0.45f;
    [SerializeField, Min(0f)] private float autoTargetShapeTolerance = 0.45f;
    [SerializeField, Min(0.05f)] private float autoTargetRefreshInterval = 0.12f;
    [SerializeField] private WeaponType currentWeapon = WeaponType.FlyingSword;
    [SerializeField] private bool disableLegacyCombatOnAwake = true;
    [SerializeField] private List<WeaponData> weapons = new List<WeaponData>();

    private float nextNormalAttackTime;
    private float nextSkillTime;
    private Transform autoTarget;
    private Vector2 autoAimDirection = Vector2.right;
    private float nextAutoTargetRefreshTime;

    public WeaponType CurrentWeapon => currentWeapon;
    public bool IsNormalAttackReady => Time.time >= nextNormalAttackTime;
    public bool IsSkillReady => Time.time >= nextSkillTime;
    public float NormalAttackCooldownRemaining => Mathf.Max(nextNormalAttackTime - Time.time, 0f);
    public float SkillCooldownRemaining => Mathf.Max(nextSkillTime - Time.time, 0f);
    public float CurrentSkillCooldown
    {
        get
        {
            WeaponData data = GetCurrentWeaponData();
            return data != null ? data.SkillCooldown : 1f;
        }
    }

    private void Reset()
    {
        weapons = CreateDefaultWeaponData();
    }

    private void Awake()
    {
        ResolveReferences();
        EnsureWeaponData();

        if (disableLegacyCombatOnAwake)
        {
            PlayerCombat2D legacyCombat = GetComponent<PlayerCombat2D>();
            if (legacyCombat != null)
            {
                legacyCombat.SetLegacyCombatEnabled(false);
            }
        }
    }

    private void Update()
    {
        UpdateAutoTarget();
        UpdateWeaponVisual();

        if (useAutomaticNormalAttack && autoTarget != null)
        {
            TryNormalAttack(autoAimDirection);
        }

        Keyboard keyboard = Keyboard.current;
        if (keyboard != null && keyboard.qKey.wasPressedThisFrame)
        {
            TrySkill();
        }
    }

    public void SetWeapon(WeaponType weaponType)
    {
        currentWeapon = weaponType;
        UpdateWeaponVisual();
    }

    public void TryNormalAttack()
    {
        TryNormalAttack(GetAttackDirection());
    }

    private void TryNormalAttack(Vector2 attackDirection)
    {
        WeaponData weaponData = GetCurrentWeaponData();
        if (weaponData == null || !IsNormalAttackReady)
        {
            return;
        }

        ExecuteAttack(
            weaponData.NormalAttackShape,
            weaponData.NormalDamageMultiplier,
            weaponData.NormalRange,
            weaponData.NormalWidth,
            weaponData.NormalAngle,
            weaponData.NormalRadius,
            weaponData.NormalProjectileCount,
            weaponData.NormalSpreadAngle,
            weaponData.NormalProjectileSpeed,
            weaponData.NormalProjectileLifetime,
            weaponData,
            false,
            attackDirection);

        nextNormalAttackTime = Time.time + weaponData.NormalAttackCooldown;
    }

    public void TrySkill()
    {
        WeaponData weaponData = GetCurrentWeaponData();
        if (weaponData == null || !IsSkillReady)
        {
            return;
        }

        ExecuteAttack(
            weaponData.SkillAttackShape,
            weaponData.SkillDamageMultiplier,
            weaponData.SkillRange,
            weaponData.SkillWidth,
            weaponData.SkillAngle,
            weaponData.SkillRadius,
            weaponData.SkillProjectileCount,
            weaponData.SkillSpreadAngle,
            weaponData.SkillProjectileSpeed,
            weaponData.SkillProjectileLifetime,
            weaponData,
            true,
            GetAttackDirection());

        nextSkillTime = Time.time + weaponData.SkillCooldown;
    }

    private void ExecuteAttack(
        AttackShape shape,
        float damageMultiplier,
        float range,
        float width,
        float angle,
        float radius,
        int projectileCount,
        float spreadAngle,
        float projectileSpeed,
        float projectileLifetime,
        WeaponData weaponData,
        bool isSkill,
        Vector2 attackDirection)
    {
        Vector2 direction = attackDirection.sqrMagnitude > 0.0001f ? attackDirection.normalized : GetAttackDirection();
        Vector2 origin = GetAttackOrigin(shape, direction);
        float damage = GetBaseDamage() * damageMultiplier;
        bool canCrit = weaponData.CritChance > 0f;

        PlayVisual(
            shape,
            origin,
            direction,
            range,
            width,
            angle,
            radius,
            projectileCount,
            spreadAngle,
            weaponData,
            isSkill);

        if (shape == AttackShape.Projectile || shape == AttackShape.ProjectileSpread)
        {
            SpawnProjectiles(origin, direction, projectileCount, spreadAngle, damage, projectileSpeed, projectileLifetime, canCrit, weaponData, isSkill);
            return;
        }

        if (meleeHitbox == null)
        {
            return;
        }

        DamageInfo damageInfo = new DamageInfo(
            damage,
            gameObject,
            canCrit,
            weaponData.CritChance,
            weaponData.CritMultiplier,
            direction);

        meleeHitbox.ApplyAttack(shape, origin, direction, range, width, angle, radius, damageInfo);
    }

    private void PlayVisual(
        AttackShape shape,
        Vector2 origin,
        Vector2 direction,
        float range,
        float width,
        float angle,
        float radius,
        int projectileCount,
        float spreadAngle,
        WeaponData weaponData,
        bool isSkill)
    {
        Vector2 visualOrigin = origin;
        if ((shape == AttackShape.Projectile || shape == AttackShape.ProjectileSpread)
            && playerAim != null
            && playerAim.FirePoint != null)
        {
            visualOrigin = playerAim.FirePoint.position;
        }

        if (weaponVisualEffect != null)
        {
            weaponVisualEffect.PlayAttack(
                weaponData.WeaponType,
                shape,
                visualOrigin,
                direction,
                range,
                width,
                angle,
                radius,
                projectileCount,
                spreadAngle,
                isSkill);
        }

        if (weaponSpriteVisual != null)
        {
            weaponSpriteVisual.PlayAttack(weaponData.WeaponType, direction, isSkill);
        }
    }

    private void SpawnProjectiles(
        Vector2 origin,
        Vector2 direction,
        int projectileCount,
        float spreadAngle,
        float damage,
        float projectileSpeed,
        float projectileLifetime,
        bool canCrit,
        WeaponData weaponData,
        bool isSkill)
    {
        if (projectileSpawner == null)
        {
            return;
        }

        Transform spawnPoint = playerAim != null && playerAim.FirePoint != null
            ? playerAim.FirePoint
            : transform;

        projectileSpawner.SpawnSpread(
            projectilePrefab,
            spawnPoint.position,
            direction,
            projectileCount,
            spreadAngle,
            damage,
            projectileSpeed,
            projectileLifetime,
            gameObject,
            canCrit,
            weaponData.CritChance,
            weaponData.CritMultiplier,
            targetLayers,
            isSkill ? weaponData.SkillPierceTargets : weaponData.NormalPierceTargets);

        if (muzzleFlash != null)
        {
            muzzleFlash.PlayFlash(spawnPoint, direction);
        }
    }

    private WeaponData GetCurrentWeaponData()
    {
        EnsureWeaponData();

        for (int i = 0; i < weapons.Count; i++)
        {
            if (weapons[i] != null && weapons[i].WeaponType == currentWeapon)
            {
                return weapons[i];
            }
        }

        return weapons.Count > 0 ? weapons[0] : null;
    }

    private float GetBaseDamage()
    {
        return playerStats != null ? playerStats.BaseDamage : 50f;
    }

    private Vector2 GetAttackDirection()
    {
        if (playerAim != null && playerAim.AimDirection.sqrMagnitude > 0.0001f)
        {
            return playerAim.AimDirection.normalized;
        }

        if (playerMovement != null && playerMovement.LastMoveDirection.sqrMagnitude > 0.0001f)
        {
            return playerMovement.LastMoveDirection.normalized;
        }

        return Vector2.right;
    }

    private Vector2 GetAttackOrigin(AttackShape shape, Vector2 direction)
    {
        Vector2 origin = transform.position;
        if (shape == AttackShape.Cone || shape == AttackShape.Rectangle)
        {
            Vector2 normalizedDirection = direction.sqrMagnitude > 0.0001f ? direction.normalized : Vector2.right;
            return origin + normalizedDirection * meleeOriginForwardOffset;
        }

        return origin;
    }

    private void UpdateWeaponVisual()
    {
        if (weaponSpriteVisual == null)
        {
            return;
        }

        weaponSpriteVisual.SetWeapon(currentWeapon);
        weaponSpriteVisual.SetAimDirection(autoTarget != null ? autoAimDirection : GetAttackDirection());
    }

    private void UpdateAutoTarget()
    {
        if (!useAutomaticNormalAttack || Time.time < nextAutoTargetRefreshTime)
        {
            return;
        }

        nextAutoTargetRefreshTime = Time.time + autoTargetRefreshInterval;
        WeaponData weaponData = GetCurrentWeaponData();
        if (weaponData == null)
        {
            autoTarget = null;
            return;
        }

        float range = GetAutoAttackRange(weaponData);
        Collider2D[] candidates = Physics2D.OverlapCircleAll(transform.position, range, ResolveTargetLayers());
        Transform bestTarget = null;
        float bestDistanceSqr = float.MaxValue;

        for (int i = 0; i < candidates.Length; i++)
        {
            Collider2D candidate = candidates[i];
            if (candidate == null || candidate.transform.root == transform.root)
            {
                continue;
            }

            IDamageable damageable = candidate.GetComponentInParent<IDamageable>();
            if (damageable == null)
            {
                continue;
            }

            Transform root = candidate.transform.root;
            Vector2 directionToTarget = (Vector2)(root.position - transform.position);
            if (directionToTarget.sqrMagnitude <= 0.0001f)
            {
                continue;
            }

            if (!IsInsideAutoAttackShape(weaponData, candidate, directionToTarget.normalized))
            {
                continue;
            }

            float distanceSqr = ((Vector2)root.position - (Vector2)transform.position).sqrMagnitude;
            if (distanceSqr < bestDistanceSqr)
            {
                bestDistanceSqr = distanceSqr;
                bestTarget = root;
            }
        }

        autoTarget = bestTarget;
        if (autoTarget != null)
        {
            Vector2 direction = (Vector2)(autoTarget.position - transform.position);
            if (direction.sqrMagnitude > 0.0001f)
            {
                autoAimDirection = direction.normalized;
            }
        }
    }

    private float GetAutoAttackRange(WeaponData weaponData)
    {
        float reach = weaponData.GetMaxNormalReach();
        if (weaponData.NormalAttackShape == AttackShape.Projectile || weaponData.NormalAttackShape == AttackShape.ProjectileSpread)
        {
            reach = Mathf.Max(reach, weaponData.NormalProjectileSpeed * weaponData.NormalProjectileLifetime);
            return Mathf.Max(0.1f, reach + autoAttackRangePadding);
        }

        return Mathf.Max(0.1f, reach + meleeOriginForwardOffset + autoAttackRangePadding);
    }

    private bool IsInsideAutoAttackShape(WeaponData weaponData, Collider2D candidate, Vector2 direction)
    {
        AttackShape shape = weaponData.NormalAttackShape;
        if (shape == AttackShape.Projectile || shape == AttackShape.ProjectileSpread)
        {
            return true;
        }

        Vector2 origin = GetAttackOrigin(shape, direction);
        float range = weaponData.NormalRange + autoTargetShapeTolerance;
        float width = weaponData.NormalWidth + autoTargetShapeTolerance * 2f;
        float angle = Mathf.Clamp(weaponData.NormalAngle + autoTargetShapeTolerance * 18f, 1f, 360f);
        float radius = weaponData.NormalRadius + autoTargetShapeTolerance;
        Bounds bounds = candidate.bounds;

        if (WeaponAttackShape.Contains(shape, origin, direction, bounds.center, range, width, angle, radius))
        {
            return true;
        }

        if (WeaponAttackShape.Contains(shape, origin, direction, candidate.ClosestPoint(origin), range, width, angle, radius))
        {
            return true;
        }

        Vector2 shapeCenter = GetAutoAttackShapeCenter(shape, origin, direction, range, radius);
        if (WeaponAttackShape.Contains(shape, origin, direction, candidate.ClosestPoint(shapeCenter), range, width, angle, radius))
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

    private static Vector2 GetAutoAttackShapeCenter(AttackShape shape, Vector2 origin, Vector2 direction, float range, float radius)
    {
        if (shape == AttackShape.Circle)
        {
            return origin;
        }

        direction = direction.sqrMagnitude > 0.0001f ? direction.normalized : Vector2.right;
        float forwardDistance = shape == AttackShape.Cone ? range * 0.65f : range * 0.5f;
        return origin + direction * Mathf.Max(forwardDistance, radius);
    }

    private LayerMask ResolveTargetLayers()
    {
        if (targetLayers.value != 0)
        {
            return targetLayers;
        }

        int enemyLayer = LayerMask.NameToLayer("Enemy");
        return enemyLayer >= 0 ? 1 << enemyLayer : ~0;
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

        if (playerMovement == null)
        {
            playerMovement = GetComponent<PlayerMovement2D>();
        }

        if (meleeHitbox == null)
        {
            meleeHitbox = GetComponent<MeleeHitbox>();
        }

        if (weaponVisualEffect == null)
        {
            weaponVisualEffect = GetComponent<WeaponVisualEffect2D>();
        }

        if (weaponSpriteVisual == null)
        {
            weaponSpriteVisual = GetComponentInChildren<WeaponSpriteVisual2D>();
        }

        if (projectileSpawner == null)
        {
            projectileSpawner = GetComponent<ProjectileSpawner>();
        }

        if (muzzleFlash == null)
        {
            muzzleFlash = GetComponent<MuzzleFlash2D>();
        }
    }

    private void EnsureWeaponData()
    {
        if (weapons == null)
        {
            weapons = new List<WeaponData>();
        }

        if (weapons.Count == 0)
        {
            weapons = CreateDefaultWeaponData();
        }
    }

    private static List<WeaponData> CreateDefaultWeaponData()
    {
        return new List<WeaponData>
        {
            new WeaponData(
                WeaponType.Sword,
                AttackShape.Cone,
                AttackShape.Projectile,
                10f,
                2f,
                0.1f,
                1.5f,
                7f,
                3.8f,
                2f,
                200f,
                1f,
                1,
                0f,
                16f,
                2f,
                false,
                0f,
                1f,
                1f,
                1f,
                1,
                0f,
                16f,
                2f,
                true),
            new WeaponData(
                WeaponType.Spear,
                AttackShape.Rectangle,
                AttackShape.Circle,
                1.9f,
                2.1f,
                0.1f,
                1.2f,
                5.2f,
                6.2f,
                2.1f,
                1f,
                1f,
                1,
                0f,
                16f,
                1f,
                false,
                0f,
                1f,
                1f,
                6.2f,
                1,
                0f,
                16f,
                1f,
                false),
            new WeaponData(
                WeaponType.Axe,
                AttackShape.Cone,
                AttackShape.Rectangle,
                2.5f,
                3.15f,
                0.05f,
                1.9f,
                8f,
                3.4f,
                2f,
                135f,
                1f,
                1,
                0f,
                16f,
                1f,
                false,
                4.8f,
                2.8f,
                1f,
                1f,
                1,
                0f,
                16f,
                1f,
                false),
            new WeaponData(
                WeaponType.FlyingSword,
                AttackShape.ProjectileSpread,
                AttackShape.ProjectileSpread,
                0.38f,
                0.75f,
                0.2f,
                1.15f,
                7f,
                0f,
                1f,
                1f,
                1f,
                3,
                18f,
                16f,
                1f,
                false,
                0f,
                1f,
                1f,
                1f,
                7,
                80f,
                16f,
                1f,
                false)
        };
    }
}
