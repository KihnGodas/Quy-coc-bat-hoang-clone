using UnityEngine;

[RequireComponent(typeof(Health))]
[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public sealed class BossBase : MonoBehaviour
{
    [SerializeField] private string bossName = "Boss Prototype";
    [SerializeField, Min(1f)] private float maxHP = 4500f;
    [SerializeField, Min(0f)] private float baseDamage = 60f;
    [SerializeField] private Health health;
    [SerializeField] private Transform target;

    public string BossName => bossName;
    public float MaxHP => health != null ? health.MaxHP : maxHP;
    public float CurrentHP => health != null ? health.CurrentHP : 0f;
    public float BaseDamage => baseDamage;
    public float HealthPercent => MaxHP > 0f ? Mathf.Clamp01(CurrentHP / MaxHP) : 0f;
    public Health Health => health;
    public Transform Target => target;
    public bool IsInvulnerable => health != null && health.IsInvulnerable;

    private void Awake()
    {
        ResolveReferences();
        health.Initialize(maxHP, true);
    }

    private void OnValidate()
    {
        maxHP = Mathf.Max(1f, maxHP);
        baseDamage = Mathf.Max(0f, baseDamage);
    }

    public void Initialize(string newBossName, float newMaxHP, float newBaseDamage, Transform newTarget)
    {
        bossName = string.IsNullOrWhiteSpace(newBossName) ? bossName : newBossName;
        maxHP = Mathf.Max(1f, newMaxHP);
        baseDamage = Mathf.Max(0f, newBaseDamage);
        target = newTarget;

        ResolveReferences();
        health.Initialize(maxHP, true);
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    public void SetInvulnerable(bool invulnerable)
    {
        ResolveReferences();
        health.SetInvulnerable(invulnerable);
    }

    private void ResolveReferences()
    {
        if (health == null)
        {
            health = GetComponent<Health>();
        }

        Rigidbody2D body = GetComponent<Rigidbody2D>();
        body.bodyType = RigidbodyType2D.Kinematic;
        body.gravityScale = 0f;

        Collider2D bossCollider = GetComponent<Collider2D>();
        bossCollider.isTrigger = true;
    }
}
