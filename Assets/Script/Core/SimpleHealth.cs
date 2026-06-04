using UnityEngine;

public sealed class SimpleHealth : MonoBehaviour, IDamageable
{
    [SerializeField, Min(1f)] private float maxHealth = 50f;
    [SerializeField] private bool destroyOnDeath = true;
    [SerializeField] private bool spawnDeathEffect = true;
    [SerializeField] private Color deathEffectColor = new Color(1f, 0.35f, 0.25f, 0.85f);
    [SerializeField, Min(0.1f)] private float deathEffectScale = 1.4f;
    [SerializeField, Min(0.01f)] private float deathEffectLifetime = 0.3f;

    private float currentHealth;

    public float MaxHealth => maxHealth;
    public float CurrentHealth => currentHealth;
    public bool IsDead => currentHealth <= 0f;
    public event System.Action<float, Vector2> Damaged;
    public event System.Action Died;
    public event System.Action<DamageInfo> OnDamaged;
    public event System.Action OnDeath;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public void Initialize(float newMaxHealth, bool refill = true)
    {
        maxHealth = Mathf.Max(1f, newMaxHealth);
        currentHealth = refill ? maxHealth : Mathf.Clamp(currentHealth, 0f, maxHealth);
    }

    public void TakeDamage(float damage)
    {
        TakeDamage(damage, Vector2.zero);
    }

    public void TakeDamage(float damage, Vector2 hitDirection)
    {
        TakeDamage(new DamageInfo(damage, null, false, 0f, 1f, hitDirection));
    }

    public void TakeDamage(DamageInfo damageInfo)
    {
        if (IsDead || damageInfo.amount <= 0f)
        {
            return;
        }

        float finalDamage = damageInfo.RollFinalAmount();
        if (finalDamage <= 0f)
        {
            return;
        }

        currentHealth = Mathf.Max(currentHealth - finalDamage, 0f);
        damageInfo.amount = finalDamage;
        Damaged?.Invoke(finalDamage, damageInfo.hitDirection);
        OnDamaged?.Invoke(damageInfo);

        if (IsDead)
        {
            Die();
        }
    }

    public void Heal(float amount)
    {
        if (IsDead || amount <= 0f)
        {
            return;
        }

        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
    }

    private void Die()
    {
        Died?.Invoke();
        OnDeath?.Invoke();
        SpawnDeathEffect();

        if (destroyOnDeath)
        {
            Destroy(gameObject);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    private void SpawnDeathEffect()
    {
        if (!spawnDeathEffect)
        {
            return;
        }

        SpriteRenderer spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (spriteRenderer == null || spriteRenderer.sprite == null)
        {
            return;
        }

        TemporaryEffect2D.CreateSpriteEffect(
            "EnemyDeathEffect",
            spriteRenderer.sprite,
            transform.position,
            transform.rotation,
            transform.lossyScale * deathEffectScale,
            deathEffectColor,
            spriteRenderer.sortingOrder + 1,
            deathEffectLifetime);
    }
}
