using UnityEngine;

public sealed class SimpleHealth : MonoBehaviour
{
    [SerializeField, Min(1f)] private float maxHealth = 3f;
    [SerializeField] private bool destroyOnDeath = true;

    public float CurrentHealth { get; private set; }
    public bool IsDead => CurrentHealth <= 0f;

    private void Awake()
    {
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
        if (destroyOnDeath)
        {
            Destroy(gameObject);
        }
    }
}
