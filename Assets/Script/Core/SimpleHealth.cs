using UnityEngine;

public sealed class SimpleHealth : MonoBehaviour
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

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        TakeDamage(damage, Vector2.zero);
    }

    public void TakeDamage(float damage, Vector2 hitDirection)
    {
        if (IsDead || damage <= 0f)
        {
            return;
        }

        currentHealth = Mathf.Max(currentHealth - damage, 0f);
        Damaged?.Invoke(damage, hitDirection);

        if (IsDead)
        {
            Die();
        }
    }

    private void Die()
    {
        Died?.Invoke();
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
