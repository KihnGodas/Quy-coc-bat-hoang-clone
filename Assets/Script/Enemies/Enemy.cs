using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(SimpleHealth))]
[RequireComponent(typeof(EnemyChaseAI2D))]
public sealed class Enemy : MonoBehaviour
{
    [SerializeField] private EnemyData data;

    private SpriteRenderer spriteRenderer;
    private SimpleHealth health;
    private EnemyChaseAI2D ai;
    private Sprite fallbackSprite;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        fallbackSprite = spriteRenderer.sprite;
        health = GetComponent<SimpleHealth>();
        ai = GetComponent<EnemyChaseAI2D>();
    }

    private void Start()
    {
        ApplyData();
    }

    public void SetData(EnemyData newData)
    {
        data = newData;
        if (isActiveAndEnabled)
        {
            ApplyData();
        }
    }

    public EnemyData Data => data;

    private void ApplyData()
    {
        if (data == null)
        {
            return;
        }

        UpdateVisual();
        health.SetMaxHealth(data.MaxHealth);
        ai.SetMoveSpeed(data.MoveSpeed);
    }

    private void UpdateVisual()
    {
        spriteRenderer.enabled = true;

        if (data.Sprite != null)
        {
            spriteRenderer.sprite = data.Sprite;
        }
        else if (fallbackSprite != null)
        {
            spriteRenderer.sprite = fallbackSprite;
        }

        if (spriteRenderer.sprite != null)
        {
            spriteRenderer.color = data.ColorTint;
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        HandleContactDamage(other.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        HandleContactDamage(other.gameObject);
    }

    private void HandleContactDamage(GameObject other)
    {
        if (data == null || data.ContactDamage <= 0f)
        {
            return;
        }

        if (other == gameObject || other.transform.root == transform.root)
        {
            return;
        }

        SimpleHealth targetHealth = other.GetComponentInParent<SimpleHealth>();
        if (targetHealth != null)
        {
            targetHealth.TakeDamage(data.ContactDamage);
        }
    }
}
