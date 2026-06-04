using System;
using UnityEngine;

[Serializable]
public sealed class WeaponData
{
    [SerializeField] private WeaponType weaponType;
    [SerializeField] private AttackShape normalAttackShape;
    [SerializeField] private AttackShape skillAttackShape;
    [SerializeField, Min(0f)] private float normalDamageMultiplier = 1f;
    [SerializeField, Min(0f)] private float skillDamageMultiplier = 1f;
    [SerializeField, Range(0f, 1f)] private float critChance;
    [SerializeField, Min(1f)] private float critMultiplier = 2f;
    [SerializeField, Min(0.01f)] private float normalAttackCooldown = 1f;
    [SerializeField, Min(0.01f)] private float skillCooldown = 5f;
    [SerializeField, Min(0f)] private float normalRange = 2f;
    [SerializeField, Min(0f)] private float normalWidth = 1f;
    [SerializeField, Range(1f, 360f)] private float normalAngle = 90f;
    [SerializeField, Min(0f)] private float normalRadius = 1f;
    [SerializeField, Min(1)] private int normalProjectileCount = 1;
    [SerializeField, Range(0f, 360f)] private float normalSpreadAngle;
    [SerializeField, Min(0.01f)] private float normalProjectileSpeed = 16f;
    [SerializeField, Min(0.01f)] private float normalProjectileLifetime = 1f;
    [SerializeField] private bool normalPierceTargets;
    [SerializeField, Min(0f)] private float skillRange = 2f;
    [SerializeField, Min(0f)] private float skillWidth = 1f;
    [SerializeField, Range(1f, 360f)] private float skillAngle = 90f;
    [SerializeField, Min(0f)] private float skillRadius = 1f;
    [SerializeField, Min(1)] private int skillProjectileCount = 1;
    [SerializeField, Range(0f, 360f)] private float skillSpreadAngle;
    [SerializeField, Min(0.01f)] private float skillProjectileSpeed = 16f;
    [SerializeField, Min(0.01f)] private float skillProjectileLifetime = 1f;
    [SerializeField] private bool skillPierceTargets;

    public WeaponType WeaponType => weaponType;
    public AttackShape NormalAttackShape => normalAttackShape;
    public AttackShape SkillAttackShape => skillAttackShape;
    public float NormalDamageMultiplier => normalDamageMultiplier;
    public float SkillDamageMultiplier => skillDamageMultiplier;
    public float CritChance => critChance;
    public float CritMultiplier => critMultiplier;
    public float NormalAttackCooldown => normalAttackCooldown;
    public float SkillCooldown => skillCooldown;
    public float NormalRange => normalRange;
    public float NormalWidth => normalWidth;
    public float NormalAngle => normalAngle;
    public float NormalRadius => normalRadius;
    public int NormalProjectileCount => normalProjectileCount;
    public float NormalSpreadAngle => normalSpreadAngle;
    public float NormalProjectileSpeed => normalProjectileSpeed;
    public float NormalProjectileLifetime => normalProjectileLifetime;
    public bool NormalPierceTargets => normalPierceTargets;
    public float SkillRange => skillRange;
    public float SkillWidth => skillWidth;
    public float SkillAngle => skillAngle;
    public float SkillRadius => skillRadius;
    public int SkillProjectileCount => skillProjectileCount;
    public float SkillSpreadAngle => skillSpreadAngle;
    public float SkillProjectileSpeed => skillProjectileSpeed;
    public float SkillProjectileLifetime => skillProjectileLifetime;
    public bool SkillPierceTargets => skillPierceTargets;

    public WeaponData(
        WeaponType weaponType,
        AttackShape normalAttackShape,
        AttackShape skillAttackShape,
        float normalDamageMultiplier,
        float skillDamageMultiplier,
        float critChance,
        float normalAttackCooldown,
        float skillCooldown,
        float normalRange,
        float normalWidth,
        float normalAngle,
        float normalRadius,
        int normalProjectileCount,
        float normalSpreadAngle,
        float normalProjectileSpeed,
        float normalProjectileLifetime,
        bool normalPierceTargets,
        float skillRange,
        float skillWidth,
        float skillAngle,
        float skillRadius,
        int skillProjectileCount,
        float skillSpreadAngle,
        float skillProjectileSpeed,
        float skillProjectileLifetime,
        bool skillPierceTargets)
    {
        this.weaponType = weaponType;
        this.normalAttackShape = normalAttackShape;
        this.skillAttackShape = skillAttackShape;
        this.normalDamageMultiplier = normalDamageMultiplier;
        this.skillDamageMultiplier = skillDamageMultiplier;
        this.critChance = critChance;
        this.normalAttackCooldown = normalAttackCooldown;
        this.skillCooldown = skillCooldown;
        this.normalRange = normalRange;
        this.normalWidth = normalWidth;
        this.normalAngle = normalAngle;
        this.normalRadius = normalRadius;
        this.normalProjectileCount = normalProjectileCount;
        this.normalSpreadAngle = normalSpreadAngle;
        this.normalProjectileSpeed = normalProjectileSpeed;
        this.normalProjectileLifetime = normalProjectileLifetime;
        this.normalPierceTargets = normalPierceTargets;
        this.skillRange = skillRange;
        this.skillWidth = skillWidth;
        this.skillAngle = skillAngle;
        this.skillRadius = skillRadius;
        this.skillProjectileCount = skillProjectileCount;
        this.skillSpreadAngle = skillSpreadAngle;
        this.skillProjectileSpeed = skillProjectileSpeed;
        this.skillProjectileLifetime = skillProjectileLifetime;
        this.skillPierceTargets = skillPierceTargets;
    }

    public float GetMaxNormalReach()
    {
        if (normalAttackShape == AttackShape.Circle)
        {
            return normalRadius;
        }

        return Mathf.Max(normalRange, normalWidth);
    }

    public float GetMaxSkillReach()
    {
        if (skillAttackShape == AttackShape.Circle)
        {
            return skillRadius;
        }

        return Mathf.Max(skillRange, skillWidth);
    }
}
