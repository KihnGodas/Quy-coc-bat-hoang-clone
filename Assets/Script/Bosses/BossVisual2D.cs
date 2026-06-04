using UnityEngine;

[RequireComponent(typeof(BossBase))]
public sealed class BossVisual2D : MonoBehaviour
{
    [SerializeField] private BossBase boss;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Color highHpColor = new Color(0.18f, 0.95f, 0.35f, 1f);
    [SerializeField] private Color lowHpColor = new Color(1f, 0.25f, 0.16f, 1f);
    [SerializeField, Min(0f)] private float pulseAmount = 0.08f;
    [SerializeField, Min(0f)] private float pulseSpeed = 3f;

    private Vector3 baseScale;

    private void Awake()
    {
        ResolveReferences();
        baseScale = transform.localScale;
    }

    private void Update()
    {
        ResolveReferences();
        if (boss == null || spriteRenderer == null)
        {
            return;
        }

        spriteRenderer.color = Color.Lerp(lowHpColor, highHpColor, boss.HealthPercent);
        float pulse = 1f + Mathf.Sin(Time.time * pulseSpeed) * pulseAmount;
        transform.localScale = baseScale * pulse;
    }

    private void ResolveReferences()
    {
        if (boss == null)
        {
            boss = GetComponent<BossBase>();
        }

        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }
    }
}
