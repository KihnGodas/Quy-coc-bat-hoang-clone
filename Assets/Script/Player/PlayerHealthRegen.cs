using UnityEngine;

public sealed class PlayerHealthRegen : MonoBehaviour
{
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private PlayerHealth2D playerHealth2D;
    [SerializeField] private Health health;
    [SerializeField, Min(0f)] private float fallbackRegenPerSecond = 1f;

    private void Awake()
    {
        ResolveReferences();
    }

    private void Update()
    {
        ResolveReferences();

        float regenPerSecond = playerStats != null ? playerStats.HealthRegen : fallbackRegenPerSecond;
        if (regenPerSecond <= 0f)
        {
            return;
        }

        float amount = regenPerSecond * Time.deltaTime;

        if (playerHealth2D != null)
        {
            playerHealth2D.Heal(amount);
            return;
        }

        if (health != null)
        {
            health.Heal(amount);
        }
    }

    private void ResolveReferences()
    {
        if (playerStats == null)
        {
            playerStats = GetComponent<PlayerStats>();
        }

        if (playerHealth2D == null)
        {
            playerHealth2D = GetComponent<PlayerHealth2D>();
        }

        if (health == null)
        {
            health = GetComponent<Health>();
        }
    }
}
