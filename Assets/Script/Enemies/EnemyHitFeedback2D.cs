using UnityEngine;

[RequireComponent(typeof(SimpleHealth))]
[RequireComponent(typeof(Rigidbody2D))]
public sealed class EnemyHitFeedback2D : MonoBehaviour
{
    [SerializeField] private SpriteRenderer targetRenderer;
    [SerializeField] private Color hitColor = Color.white;
    [SerializeField, Min(0f)] private float flashDuration = 0.08f;
    [SerializeField, Min(0f)] private float knockbackSpeed = 3.5f;
    [SerializeField, Min(0f)] private float knockbackDuration = 0.08f;

    private SimpleHealth health;
    private Rigidbody2D body;
    private Color defaultColor;
    private Vector2 knockbackDirection;
    private float flashEndTime;
    private float knockbackEndTime;
    private bool hasDefaultColor;

    private void Awake()
    {
        health = GetComponent<SimpleHealth>();
        body = GetComponent<Rigidbody2D>();

        if (targetRenderer == null)
        {
            targetRenderer = GetComponentInChildren<SpriteRenderer>();
        }

        CacheDefaultColor();
    }

    private void OnEnable()
    {
        if (health == null)
        {
            health = GetComponent<SimpleHealth>();
        }

        health.Damaged += HandleDamaged;
    }

    private void OnDisable()
    {
        if (health != null)
        {
            health.Damaged -= HandleDamaged;
        }

        RestoreColor();
    }

    private void Update()
    {
        if (targetRenderer == null || !hasDefaultColor)
        {
            return;
        }

        targetRenderer.color = Time.time < flashEndTime ? hitColor : defaultColor;
    }

    private void FixedUpdate()
    {
        if (Time.time >= knockbackEndTime || knockbackDirection.sqrMagnitude < 0.0001f)
        {
            return;
        }

        Vector2 nextPosition = body.position + knockbackDirection * knockbackSpeed * Time.fixedDeltaTime;
        body.MovePosition(nextPosition);
    }

    private void HandleDamaged(float damage, Vector2 hitDirection)
    {
        flashEndTime = Time.time + flashDuration;

        if (hitDirection.sqrMagnitude > 0.0001f)
        {
            knockbackDirection = hitDirection.normalized;
            knockbackEndTime = Time.time + knockbackDuration;
        }
    }

    private void CacheDefaultColor()
    {
        if (targetRenderer == null)
        {
            return;
        }

        defaultColor = targetRenderer.color;
        hasDefaultColor = true;
    }

    private void RestoreColor()
    {
        if (targetRenderer != null && hasDefaultColor)
        {
            targetRenderer.color = defaultColor;
        }
    }
}
