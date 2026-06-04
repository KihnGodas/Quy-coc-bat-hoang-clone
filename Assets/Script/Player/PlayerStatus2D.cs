using UnityEngine;

public sealed class PlayerStatus2D : MonoBehaviour
{
    private float rootEndTime;
    private float poisonEndTime;
    private float poisonTickDamage;
    private float poisonTickInterval = 1f;
    private float nextPoisonTickTime;
    private GameObject poisonSource;
    private SpriteRenderer statusRenderer;
    private Color originalRendererColor;
    private Color poisonTintColor = new Color(0.25f, 1f, 0.15f, 1f);
    private bool hasOriginalRendererColor;

    public bool IsRooted => Time.time < rootEndTime;
    public float RootRemaining => Mathf.Max(rootEndTime - Time.time, 0f);
    public bool IsPoisoned => Time.time < poisonEndTime;
    public float PoisonRemaining => Mathf.Max(poisonEndTime - Time.time, 0f);

    private void Awake()
    {
        ResolveRenderer();
    }

    private void Update()
    {
        UpdatePoisonDamage();
        UpdatePoisonVisual();
    }

    public void ApplyRoot(float duration)
    {
        if (duration <= 0f)
        {
            return;
        }

        rootEndTime = Mathf.Max(rootEndTime, Time.time + duration);
    }

    public void ApplyPoison(float duration, float tickDamage, float tickInterval, GameObject source, Color tintColor)
    {
        if (duration <= 0f || tickDamage <= 0f)
        {
            return;
        }

        poisonEndTime = Mathf.Max(poisonEndTime, Time.time + duration);
        poisonTickDamage = Mathf.Max(poisonTickDamage, tickDamage);
        poisonTickInterval = Mathf.Max(tickInterval, 0.1f);
        nextPoisonTickTime = Time.time + poisonTickInterval;
        poisonSource = source;
        poisonTintColor = tintColor;
        ResolveRenderer();
    }

    private void UpdatePoisonDamage()
    {
        if (!IsPoisoned)
        {
            return;
        }

        IDamageable damageable = GetComponent<IDamageable>();
        if (damageable == null)
        {
            return;
        }

        while (Time.time >= nextPoisonTickTime && nextPoisonTickTime <= poisonEndTime)
        {
            damageable.TakeDamage(new DamageInfo(poisonTickDamage, poisonSource));
            nextPoisonTickTime += poisonTickInterval;
        }
    }

    private void UpdatePoisonVisual()
    {
        ResolveRenderer();

        if (statusRenderer == null || !hasOriginalRendererColor)
        {
            return;
        }

        if (!IsPoisoned)
        {
            statusRenderer.color = originalRendererColor;
            return;
        }

        float pulse = 0.45f + Mathf.PingPong(Time.time * 3f, 0.35f);
        statusRenderer.color = Color.Lerp(originalRendererColor, poisonTintColor, pulse);
    }

    private void ResolveRenderer()
    {
        if (statusRenderer != null)
        {
            return;
        }

        statusRenderer = GetComponentInChildren<SpriteRenderer>();
        if (statusRenderer == null)
        {
            return;
        }

        originalRendererColor = statusRenderer.color;
        hasOriginalRendererColor = true;
    }
}
