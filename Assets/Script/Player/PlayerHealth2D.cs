using UnityEngine;

public sealed class PlayerHealth2D : MonoBehaviour, IDamageable
{
    [SerializeField, Min(1f)] private float maxHealth = 100f;
    [SerializeField] private float currentHealth;
    [SerializeField] private bool disableControlsOnDeath = true;
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private bool usePlayerStats = true;

    public float MaxHealth => maxHealth;
    public float CurrentHealth => currentHealth;
    public bool IsDead => CurrentHealth <= 0f;
    public bool IsInvincible { get; private set; }
    public event System.Action<DamageInfo> OnDamaged;
    public event System.Action OnDeath;

    private void Awake()
    {
        if (playerStats == null)
        {
            playerStats = GetComponent<PlayerStats>();
        }

        if (usePlayerStats && playerStats != null)
        {
            maxHealth = playerStats.MaxHP;
        }

        currentHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        TakeDamage(new DamageInfo(damage, null));
    }

    public void TakeDamage(DamageInfo damageInfo)
    {
        if (IsDead || IsInvincible || damageInfo.amount <= 0f)
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

    public void SetInvincible(bool isInvincible)
    {
        IsInvincible = isInvincible;
    }

    private void Die()
    {
        OnDeath?.Invoke();
        Debug.Log("Player Dead");

        if (!disableControlsOnDeath)
        {
            return;
        }

        PlayerMovement2D movement = GetComponent<PlayerMovement2D>();
        if (movement != null)
        {
            movement.enabled = false;
        }

        PlayerAim2D aim = GetComponent<PlayerAim2D>();
        if (aim != null)
        {
            aim.enabled = false;
        }

        PlayerCombat2D combat = GetComponent<PlayerCombat2D>();
        if (combat != null)
        {
            combat.enabled = false;
        }

        PlayerDash2D dash = GetComponent<PlayerDash2D>();
        if (dash != null)
        {
            dash.enabled = false;
        }
    }
}
