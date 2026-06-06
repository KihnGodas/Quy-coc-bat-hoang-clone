using UnityEngine;

[RequireComponent(typeof(SimpleHealth))]
[RequireComponent(typeof(Rigidbody2D))]
public sealed class EnemyHitFeedback2D : MonoBehaviour
{
    [SerializeField] private SpriteRenderer targetRenderer;
    [SerializeField] private Color hitColor = new Color(1f, 0.15f, 0.1f, 1f);
    [SerializeField, Min(0f)] private float flashDuration = 0.12f;
    [SerializeField, Min(0f)] private float knockbackSpeed = 3.5f;
    [SerializeField, Min(0f)] private float knockbackDuration = 0.08f;

    private SimpleHealth health;
    private Rigidbody2D body;
    private Color defaultColor;
    private Vector2 knockbackDirection;
    private float flashEndTime;
    private float knockbackEndTime;
    private bool hasDefaultColor;
    private EnemyStatus2D enemyStatus;

    public bool IsFlashing => Time.time < flashEndTime;
    public Color HitColor => hitColor;

    private void Awake()
    {
        health = GetComponent<SimpleHealth>();
        body = GetComponent<Rigidbody2D>();
        enemyStatus = GetComponent<EnemyStatus2D>();

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

    public void OnEnemyDataChanged(EnemyData enemyData)
    {
        defaultColor = enemyData != null ? enemyData.VisualColor : Color.white;
        hasDefaultColor = true;
        RestoreColor();
    }

    private void Update()
    {
        if (targetRenderer == null || !hasDefaultColor)
        {
            return;
        }

        if (enemyStatus == null)
        {
            enemyStatus = GetComponent<EnemyStatus2D>();
        }

        if (enemyStatus != null)
        {
            return;
        }

        targetRenderer.color = IsFlashing ? hitColor : defaultColor;
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

        EnemyBase enemyBase = GetComponent<EnemyBase>();
        defaultColor = enemyBase != null && enemyBase.Data != null ? enemyBase.Data.VisualColor : Color.white;
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
