using System;
using UnityEngine;

[Serializable]
public sealed class SpellData
{
    [SerializeField] private SpellType spellType = SpellType.WoodVine;
    [SerializeField, Min(0f)] private float damageMultiplier = 1f;
    [SerializeField, Min(0.05f)] private float cooldown = 1f;
    [SerializeField, Min(0f)] private float radius = 1f;
    [SerializeField, Min(0f)] private float duration = 0f;
    [SerializeField, Min(0f)] private float projectileSpeed = 12f;
    [SerializeField, Min(0.05f)] private float projectileLifetime = 1.5f;
    [SerializeField, Min(1)] private int projectileCount = 1;
    [SerializeField, Min(0f)] private float projectileDelay = 0f;
    [SerializeField, Range(0f, 180f)] private float spreadAngle = 0f;
    [SerializeField] private bool pierceTargets;
    [SerializeField] private Color visualColor = Color.white;

    public SpellType SpellType => spellType;
    public float DamageMultiplier => damageMultiplier;
    public float Cooldown => cooldown;
    public float Radius => radius;
    public float Duration => duration;
    public float ProjectileSpeed => projectileSpeed;
    public float ProjectileLifetime => projectileLifetime;
    public int ProjectileCount => projectileCount;
    public float ProjectileDelay => projectileDelay;
    public float SpreadAngle => spreadAngle;
    public bool PierceTargets => pierceTargets;
    public Color VisualColor => visualColor;

    public SpellData()
    {
    }

    public SpellData(
        SpellType spellType,
        float damageMultiplier,
        float cooldown,
        float radius,
        float duration,
        float projectileSpeed,
        float projectileLifetime,
        int projectileCount,
        float projectileDelay,
        float spreadAngle,
        bool pierceTargets,
        Color visualColor)
    {
        this.spellType = spellType;
        this.damageMultiplier = Mathf.Max(0f, damageMultiplier);
        this.cooldown = Mathf.Max(0.05f, cooldown);
        this.radius = Mathf.Max(0f, radius);
        this.duration = Mathf.Max(0f, duration);
        this.projectileSpeed = Mathf.Max(0f, projectileSpeed);
        this.projectileLifetime = Mathf.Max(0.05f, projectileLifetime);
        this.projectileCount = Mathf.Max(1, projectileCount);
        this.projectileDelay = Mathf.Max(0f, projectileDelay);
        this.spreadAngle = Mathf.Clamp(spreadAngle, 0f, 180f);
        this.pierceTargets = pierceTargets;
        this.visualColor = visualColor;
    }
}
