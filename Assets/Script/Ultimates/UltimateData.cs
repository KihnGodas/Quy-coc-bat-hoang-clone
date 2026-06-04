using System;
using UnityEngine;

[Serializable]
public sealed class UltimateData
{
    [SerializeField] private UltimateType ultimateType = UltimateType.WoodGrandRoots;
    [SerializeField] private SpellType linkedSpell = SpellType.WoodVine;
    [SerializeField, Min(0f)] private float damageMultiplier = 1f;
    [SerializeField, Min(0.1f)] private float cooldown = 20f;
    [SerializeField, Min(0f)] private float radius = 3f;
    [SerializeField, Min(0f)] private float duration = 0f;
    [SerializeField, Min(0f)] private float castDelay = 0f;
    [SerializeField, Min(0f)] private float rectangleLength = 0f;
    [SerializeField, Min(0f)] private float rectangleWidth = 0f;
    [SerializeField, Min(0f)] private float knockbackSpeed = 0f;
    [SerializeField, Min(0f)] private float knockbackDuration = 0f;
    [SerializeField, Min(0.01f)] private float hitRadius = 1f;
    [SerializeField, Min(0f)] private float projectileSpeed = 14f;
    [SerializeField, Min(0.05f)] private float projectileLifetime = 1f;
    [SerializeField, Min(1)] private int projectileCount = 1;
    [SerializeField, Range(0f, 360f)] private float spreadAngle = 0f;
    [SerializeField] private bool pierceTargets;
    [SerializeField] private Color primaryColor = Color.white;
    [SerializeField] private Color secondaryColor = Color.white;

    public UltimateType UltimateType => ultimateType;
    public SpellType LinkedSpell => linkedSpell;
    public float DamageMultiplier => damageMultiplier;
    public float Cooldown => cooldown;
    public float Radius => radius;
    public float Duration => duration;
    public float CastDelay => castDelay;
    public float RectangleLength => rectangleLength;
    public float RectangleWidth => rectangleWidth;
    public float KnockbackSpeed => knockbackSpeed;
    public float KnockbackDuration => knockbackDuration;
    public float HitRadius => hitRadius;
    public float ProjectileSpeed => projectileSpeed;
    public float ProjectileLifetime => projectileLifetime;
    public int ProjectileCount => projectileCount;
    public float SpreadAngle => spreadAngle;
    public bool PierceTargets => pierceTargets;
    public Color PrimaryColor => primaryColor;
    public Color SecondaryColor => secondaryColor;

    public UltimateData()
    {
    }

    public UltimateData(
        UltimateType ultimateType,
        SpellType linkedSpell,
        float damageMultiplier,
        float cooldown,
        float radius,
        float duration,
        float castDelay,
        float rectangleLength,
        float rectangleWidth,
        float knockbackSpeed,
        float knockbackDuration,
        float hitRadius,
        float projectileSpeed,
        float projectileLifetime,
        int projectileCount,
        float spreadAngle,
        bool pierceTargets,
        Color primaryColor,
        Color secondaryColor)
    {
        this.ultimateType = ultimateType;
        this.linkedSpell = linkedSpell;
        this.damageMultiplier = Mathf.Max(0f, damageMultiplier);
        this.cooldown = Mathf.Max(0.1f, cooldown);
        this.radius = Mathf.Max(0f, radius);
        this.duration = Mathf.Max(0f, duration);
        this.castDelay = Mathf.Max(0f, castDelay);
        this.rectangleLength = Mathf.Max(0f, rectangleLength);
        this.rectangleWidth = Mathf.Max(0f, rectangleWidth);
        this.knockbackSpeed = Mathf.Max(0f, knockbackSpeed);
        this.knockbackDuration = Mathf.Max(0f, knockbackDuration);
        this.hitRadius = Mathf.Max(0.01f, hitRadius);
        this.projectileSpeed = Mathf.Max(0f, projectileSpeed);
        this.projectileLifetime = Mathf.Max(0.05f, projectileLifetime);
        this.projectileCount = Mathf.Max(1, projectileCount);
        this.spreadAngle = Mathf.Clamp(spreadAngle, 0f, 360f);
        this.pierceTargets = pierceTargets;
        this.primaryColor = primaryColor;
        this.secondaryColor = secondaryColor;
    }
}
