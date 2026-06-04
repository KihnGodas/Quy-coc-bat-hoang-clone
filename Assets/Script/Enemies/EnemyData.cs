using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "Combat/Enemy Data")]
public sealed class EnemyData : ScriptableObject
{
    [SerializeField] private string enemyName = "Enemy";
    [SerializeField] private EnemyRole enemyRole = EnemyRole.MeleeChaser;
    [SerializeField, Min(1f)] private float maxHP = 100f;
    [SerializeField, Min(0f)] private float damage = 10f;
    [SerializeField, Min(0f)] private float moveSpeed = 5f;
    [SerializeField, Min(0f)] private float attackRange = 1.1f;
    [SerializeField, Min(0f)] private float attackCooldown = 1f;
    [SerializeField] private bool rotateToMoveDirection;
    [SerializeField, Min(0f)] private float arenaPadding = 0.35f;
    [Header("Visual")]
    [SerializeField] private Color visualColor = Color.white;
    [SerializeField] private Vector2 visualScale = Vector2.one;
    [SerializeField] private Vector2 colliderSize = Vector2.one;
    [SerializeField] private Vector2 colliderOffset = Vector2.zero;
    [Header("Low Health")]
    [SerializeField, Range(0.05f, 1f)] private float lowHealthThreshold = 0.5f;
    [SerializeField, Min(0.1f)] private float lowHealthMoveSpeedMultiplier = 1f;
    [SerializeField, Min(0.1f)] private float lowHealthDamageMultiplier = 1f;
    [SerializeField, Min(0.1f)] private float lowHealthAttackRateMultiplier = 1f;
    [SerializeField, Min(0.1f)] private float lowHealthMaxHealthMultiplier = 1f;
    [Header("Charge")]
    [SerializeField, Min(0f)] private float chargeRange = 5f;
    [SerializeField, Min(0f)] private float chargeCooldown = 4f;
    [SerializeField, Min(0f)] private float chargeWindup = 0.4f;
    [SerializeField, Min(0f)] private float chargeDistance = 6f;
    [SerializeField, Min(0f)] private float chargeSpeed = 18f;
    [SerializeField, Min(0f)] private float chargeDamageMultiplier = 1.5f;
    [Header("Projectile")]
    [SerializeField] private Projectile2D projectilePrefab;
    [SerializeField, Min(0f)] private float projectileRange = 8f;
    [SerializeField, Min(0f)] private float projectileRetreatRange;
    [SerializeField, Min(0f)] private float projectileResumeRange;
    [SerializeField, Min(0f)] private float projectileDamage = 20f;
    [SerializeField, Min(0f)] private float projectileSpeed = 9f;
    [SerializeField, Min(0.1f)] private float projectileLifetime = 2f;
    [SerializeField, Min(0f)] private float projectileCooldown = 4f;
    [SerializeField, Min(0f)] private float projectileSpawnOffset = 0.75f;
    [SerializeField, Min(0f)] private float projectileChargeTime;
    [SerializeField, Min(0f)] private float projectileFlashTime;
    [SerializeField] private bool projectilePiercing;
    [SerializeField, Min(0f)] private float projectilePoisonDuration;
    [SerializeField, Min(0f)] private float projectilePoisonTickDamage;
    [SerializeField, Min(0.1f)] private float projectilePoisonTickInterval = 1f;
    [SerializeField] private Color projectilePoisonTintColor = new Color(0.25f, 1f, 0.15f, 1f);
    [Header("Rock Throw")]
    [SerializeField, Min(0f)] private float rockWarningTime = 0.6f;
    [SerializeField, Min(0f)] private float rockFallTime = 0.45f;
    [SerializeField, Min(0f)] private float rockRadius = 1.25f;
    [Header("Root")]
    [SerializeField, Min(0f)] private float rootRange = 8f;
    [SerializeField, Min(0f)] private float rootCooldown = 5f;
    [SerializeField, Min(0f)] private float rootWarningTime = 0.5f;
    [SerializeField, Min(0f)] private float rootDuration = 1f;
    [SerializeField, Min(0f)] private float rootRadius = 1.5f;
    [SerializeField, Min(0f)] private float rootDamage = 0f;
    [SerializeField, Min(0f)] private float rootTrapDamageMultiplier = 0.5f;
    [Header("Progression")]
    [SerializeField, Min(0f)] private float experienceReward;

    public string EnemyName => enemyName;
    public EnemyRole EnemyRole => enemyRole;
    public float MaxHP => maxHP;
    public float Damage => damage;
    public float MoveSpeed => moveSpeed;
    public float AttackRange => attackRange;
    public float AttackCooldown => attackCooldown;
    public bool RotateToMoveDirection => rotateToMoveDirection;
    public float ArenaPadding => arenaPadding;
    public Color VisualColor => visualColor;
    public Vector2 VisualScale => visualScale;
    public Vector2 ColliderSize => colliderSize;
    public Vector2 ColliderOffset => colliderOffset;
    public float LowHealthThreshold => lowHealthThreshold;
    public float LowHealthMoveSpeedMultiplier => lowHealthMoveSpeedMultiplier;
    public float LowHealthDamageMultiplier => lowHealthDamageMultiplier;
    public float LowHealthAttackRateMultiplier => lowHealthAttackRateMultiplier;
    public float LowHealthMaxHealthMultiplier => lowHealthMaxHealthMultiplier;
    public float ChargeRange => chargeRange;
    public float ChargeCooldown => chargeCooldown;
    public float ChargeWindup => chargeWindup;
    public float ChargeDistance => chargeDistance;
    public float ChargeSpeed => chargeSpeed;
    public float ChargeDamageMultiplier => chargeDamageMultiplier;
    public Projectile2D ProjectilePrefab => projectilePrefab;
    public float ProjectileRange => projectileRange;
    public float ProjectileRetreatRange => projectileRetreatRange;
    public float ProjectileResumeRange => projectileResumeRange;
    public float ProjectileDamage => projectileDamage;
    public float ProjectileSpeed => projectileSpeed;
    public float ProjectileLifetime => projectileLifetime;
    public float ProjectileCooldown => projectileCooldown;
    public float ProjectileSpawnOffset => projectileSpawnOffset;
    public float ProjectileChargeTime => projectileChargeTime;
    public float ProjectileFlashTime => projectileFlashTime;
    public bool ProjectilePiercing => projectilePiercing;
    public float ProjectilePoisonDuration => projectilePoisonDuration;
    public float ProjectilePoisonTickDamage => projectilePoisonTickDamage;
    public float ProjectilePoisonTickInterval => projectilePoisonTickInterval;
    public Color ProjectilePoisonTintColor => projectilePoisonTintColor;
    public float RockWarningTime => rockWarningTime;
    public float RockFallTime => rockFallTime;
    public float RockRadius => rockRadius;
    public float RootRange => rootRange;
    public float RootCooldown => rootCooldown;
    public float RootWarningTime => rootWarningTime;
    public float RootDuration => rootDuration;
    public float RootRadius => rootRadius;
    public float RootDamage => rootDamage;
    public float RootTrapDamageMultiplier => rootTrapDamageMultiplier;
    public float ExperienceReward => experienceReward > 0f ? experienceReward : GetDefaultExperienceReward();

    private float GetDefaultExperienceReward()
    {
        float reward = maxHP * 0.12f + damage * 0.5f;

        reward += enemyRole switch
        {
            EnemyRole.SpeedChaser => 2f,
            EnemyRole.Charger => 4f,
            EnemyRole.Tank => 2f,
            EnemyRole.Berserker => 6f,
            EnemyRole.HybridThrower => 5f,
            EnemyRole.RangedProjectile => 4f,
            EnemyRole.RootMage => 5f,
            _ => 0f
        };

        return Mathf.Max(8f, Mathf.Round(reward));
    }
}
