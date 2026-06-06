using UnityEngine;

[RequireComponent(typeof(Health))]
[RequireComponent(typeof(Rigidbody2D))]
public sealed class TutorialTarget : MonoBehaviour
{
    [SerializeField] private Health health;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private float respawnDelay = 2f;
    [SerializeField] private Color hitFlashColor = new Color(1f, 0.3f, 0.3f, 1f);

    private Color originalColor = Color.white;
    private bool isRespawning;
    private float respawnTime;

    private void Awake()
    {
        if (health == null) health = GetComponent<Health>();
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }

        if (health != null)
        {
            health.OnDamaged += OnDamaged;
            health.OnDeath += OnDeath;
        }

        Rigidbody2D body = GetComponent<Rigidbody2D>();
        if (body != null)
        {
            body.bodyType = RigidbodyType2D.Kinematic;
            body.simulated = true;
        }
    }

    private void OnDestroy()
    {
        if (health != null)
        {
            health.OnDamaged -= OnDamaged;
            health.OnDeath -= OnDeath;
        }
    }

    private void OnDamaged(DamageInfo info)
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = hitFlashColor;
            Invoke(nameof(ResetColor), 0.12f);
        }
    }

    private void ResetColor()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = originalColor;
        }
    }

    private void OnDeath()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = false;
        }

        isRespawning = true;
        respawnTime = Time.time + respawnDelay;
    }

    private void Update()
    {
        if (isRespawning && Time.time >= respawnTime)
        {
            Respawn();
        }
    }

    private void Respawn()
    {
        isRespawning = false;

        if (health != null)
        {
            health.Initialize(health.MaxHP, true);
        }

        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = true;
            spriteRenderer.color = originalColor;
        }
    }
}
