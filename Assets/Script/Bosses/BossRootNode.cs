using UnityEngine;

[RequireComponent(typeof(Health))]
[RequireComponent(typeof(Collider2D))]
public sealed class BossRootNode : MonoBehaviour
{
    [SerializeField] private Health health;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Act1WoodBossController owner;
    [SerializeField, Min(1f)] private float maxHP = 700f;

    private Color baseColor;
    private float flashEndTime;

    public bool IsDead => health == null || health.IsDead;
    public Health Health => health;

    private void Awake()
    {
        ResolveReferences();
        health.Initialize(maxHP, true);
        baseColor = spriteRenderer != null ? spriteRenderer.color : Color.white;
        health.OnDeath += HandleDeath;
        health.OnDamaged += HandleDamaged;
    }

    private void Update()
    {
        if (spriteRenderer != null && Time.time > flashEndTime)
        {
            spriteRenderer.color = baseColor;
        }
    }

    private void OnDestroy()
    {
        if (health != null)
        {
            health.OnDeath -= HandleDeath;
            health.OnDamaged -= HandleDamaged;
        }
    }

    public void Initialize(Act1WoodBossController rootOwner, float rootHP, Color rootColor)
    {
        owner = rootOwner;
        maxHP = Mathf.Max(1f, rootHP);
        ResolveReferences();
        health.Initialize(maxHP, true);
        if (spriteRenderer != null)
        {
            spriteRenderer.color = rootColor;
            baseColor = rootColor;
        }
    }

    private void ResolveReferences()
    {
        if (health == null)
        {
            health = GetComponent<Health>();
        }

        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }

        Collider2D rootCollider = GetComponent<Collider2D>();
        rootCollider.isTrigger = true;
    }

    private void HandleDamaged(DamageInfo damageInfo)
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = new Color(1f, 0.2f, 0.15f, 1f);
            flashEndTime = Time.time + 0.08f;
        }
    }

    private void HandleDeath()
    {
        owner?.NotifyRootDestroyed(this);
    }
}
