using UnityEngine;
using UnityEngine.Events;

public sealed class SimpleHealth : MonoBehaviour
{
    [SerializeField, Min(1f)] private float maxHealth = 3f;
    [SerializeField] private bool destroyOnDeath = true;

    public float CurrentHealth { get; private set; }
    public float MaxHealth => maxHealth;
    public float HealthPercent => CurrentHealth / maxHealth;
    public bool IsDead => CurrentHealth <= 0f;

    public UnityEvent OnDeath = new UnityEvent();

    private void Awake()
    {
        CurrentHealth = maxHealth;
    }

    public void SetMaxHealth(float value)
    {
        maxHealth = Mathf.Max(1f, value);
        CurrentHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        if (IsDead || damage <= 0f)
        {
            return;
        }

        CurrentHealth = Mathf.Max(CurrentHealth - damage, 0f);

        if (IsDead)
        {
            Die();
        }
    }

    private void Die()
    {
        OnDeath.Invoke();
        if (destroyOnDeath)
        {
            Destroy(gameObject);
        }
    }
}
