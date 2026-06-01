using UnityEngine;

public sealed class PlayerHealth2D : MonoBehaviour
{
    [SerializeField, Min(1f)] private float maxHealth = 100f;
    [SerializeField] private float currentHealth;
    [SerializeField] private bool disableControlsOnDeath = true;

    public float MaxHealth => maxHealth;
    public float CurrentHealth => currentHealth;
    public bool IsDead => CurrentHealth <= 0f;
    public bool IsInvincible { get; private set; }

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        if (IsDead || IsInvincible || damage <= 0f)
        {
            return;
        }

        currentHealth = Mathf.Max(currentHealth - damage, 0f);

        if (IsDead)
        {
            Die();
        }
    }

    public void SetInvincible(bool isInvincible)
    {
        IsInvincible = isInvincible;
    }

    private void Die()
    {
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
