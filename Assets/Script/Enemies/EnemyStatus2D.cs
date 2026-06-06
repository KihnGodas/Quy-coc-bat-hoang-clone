using UnityEngine;

public sealed class EnemyStatus2D : MonoBehaviour
{
    [SerializeField] private Color rootTint = new Color(0.35f, 1f, 0.35f, 1f);
    [SerializeField] private Color stunTint = new Color(1f, 0.9f, 0.25f, 1f);

    private SpriteRenderer spriteRenderer;
    private EnemyHitFeedback2D hitFeedback;
    private Rigidbody2D body;
    private Color originalColor = Color.white;
    private float rootEndTime;
    private float stunEndTime;
    private float knockbackEndTime;
    private Vector2 knockbackVelocity;

    public bool IsRooted => Time.time < rootEndTime;
    public bool IsStunned => Time.time < stunEndTime;
    public bool IsKnockbacked => Time.time < knockbackEndTime;
    public bool BlocksMovement => IsRooted || IsStunned || IsKnockbacked;
    public bool BlocksAttack => IsStunned;

    private void Awake()
    {
        ResolveReferences();
    }

    private void Update()
    {
        ResolveReferences();
        UpdateTint();
    }

    private void FixedUpdate()
    {
        if (!IsKnockbacked || body == null)
        {
            return;
        }

        body.MovePosition(body.position + knockbackVelocity * Time.fixedDeltaTime);
    }

    public void ApplyRoot(float duration)
    {
        if (duration <= 0f)
        {
            return;
        }

        rootEndTime = Mathf.Max(rootEndTime, Time.time + duration);
        SpellVisualEffect2D.CreateVines(transform.position, 0.75f, rootTint, Mathf.Min(duration, 0.8f));
    }

    public void ApplyStun(float duration)
    {
        if (duration <= 0f)
        {
            return;
        }

        stunEndTime = Mathf.Max(stunEndTime, Time.time + duration);
        SpellVisualEffect2D.CreateBurst("Enemy_Stun_Burst", transform.position, 0.8f, 8, stunTint, 0.35f, 0.08f, 36);
    }

    public void ApplyKnockback(Vector2 direction, float speed, float duration)
    {
        if (duration <= 0f || speed <= 0f)
        {
            return;
        }

        direction = direction.sqrMagnitude > 0.0001f ? direction.normalized : Vector2.right;
        knockbackVelocity = direction * speed;
        knockbackEndTime = Mathf.Max(knockbackEndTime, Time.time + duration);
        SpellVisualEffect2D.CreateProjectileLaunch("Enemy_Knockback_Line", transform.position, direction, 0.9f, new Color(0.4f, 0.9f, 1f, 0.9f), 0.2f, 0.08f);
    }

    public void OnEnemyDataChanged(EnemyData enemyData)
    {
        originalColor = enemyData != null ? enemyData.VisualColor : Color.white;
        UpdateTint();
    }

    private void UpdateTint()
    {
        if (spriteRenderer == null)
        {
            return;
        }

        if (IsStunned)
        {
            spriteRenderer.color = GetFinalTint(stunTint);
        }
        else if (IsRooted)
        {
            spriteRenderer.color = GetFinalTint(rootTint);
        }
        else
        {
            spriteRenderer.color = GetFinalTint(originalColor);
        }
    }

    private Color GetFinalTint(Color baseTint)
    {
        if (hitFeedback != null && hitFeedback.IsFlashing)
        {
            return Color.Lerp(baseTint, hitFeedback.HitColor, 0.8f);
        }

        return baseTint;
    }

    private void ResolveReferences()
    {
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                EnemyBase enemyBase = GetComponent<EnemyBase>();
                originalColor = enemyBase != null && enemyBase.Data != null ? enemyBase.Data.VisualColor : Color.white;
            }
        }

        if (body == null)
        {
            body = GetComponent<Rigidbody2D>();
        }

        if (hitFeedback == null)
        {
            hitFeedback = GetComponent<EnemyHitFeedback2D>();
        }
    }
}
