using System;
using UnityEngine;

public sealed class Health : MonoBehaviour, IDamageable
{
    [SerializeField, Min(1f)] private float maxHP = 100f;
    [SerializeField] private float currentHP;
    [SerializeField] private bool startFull = true;
    [SerializeField] private bool destroyOnDeath = true;
    [SerializeField] private bool isInvulnerable;
    [SerializeField, Min(0f)] private float damageTakenMultiplier = 1f;
    [SerializeField] private bool spawnDeathEffect;
    [SerializeField] private Color deathEffectColor = new Color(1f, 0.35f, 0.25f, 0.85f);
    [SerializeField, Min(0.1f)] private float deathEffectScale = 1.4f;
    [SerializeField, Min(0.01f)] private float deathEffectLifetime = 0.3f;

    public float MaxHP => maxHP;
    public float CurrentHP => currentHP;
    public bool IsDead => currentHP <= 0f;
    public bool IsInvulnerable => isInvulnerable;

    public event Action<DamageInfo> OnDamaged;
    public event Action OnDeath;

    private void Awake()
    {
        if (startFull || currentHP <= 0f)
        {
            currentHP = maxHP;
        }
        else
        {
            currentHP = Mathf.Clamp(currentHP, 0f, maxHP);
        }
    }

    private void OnValidate()
    {
        maxHP = Mathf.Max(1f, maxHP);
        currentHP = Mathf.Clamp(currentHP, 0f, maxHP);
        damageTakenMultiplier = Mathf.Max(0f, damageTakenMultiplier);
    }

    public void Initialize(float newMaxHP, bool refill = true)
    {
        maxHP = Mathf.Max(1f, newMaxHP);

        if (refill)
        {
            currentHP = maxHP;
        }
        else
        {
            currentHP = Mathf.Clamp(currentHP, 0f, maxHP);
        }
    }

    public void TakeDamage(DamageInfo damageInfo)
    {
        if (IsDead || isInvulnerable)
        {
            return;
        }

        float finalAmount = damageInfo.RollFinalAmount() * damageTakenMultiplier;
        if (finalAmount <= 0f)
        {
            return;
        }

        currentHP = Mathf.Max(currentHP - finalAmount, 0f);
        damageInfo.amount = finalAmount;
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

        currentHP = Mathf.Min(currentHP + amount, maxHP);
    }

    public void SetInvulnerable(bool invulnerable)
    {
        isInvulnerable = invulnerable;
    }

    private void Die()
    {
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
            $"{name}DeathEffect",
            spriteRenderer.sprite,
            transform.position,
            transform.rotation,
            transform.lossyScale * deathEffectScale,
            deathEffectColor,
            spriteRenderer.sortingOrder + 1,
            deathEffectLifetime);
    }
}
