using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public sealed class EnemyTelegraph2D : MonoBehaviour
{
    [SerializeField, Min(0.01f)] private float duration = 0.5f;
    [SerializeField] private bool fadeOut = true;
    [SerializeField] private bool fadeIn;
    [SerializeField] private bool blink;
    [SerializeField, Min(0.1f)] private float blinkFrequency = 8f;
    [SerializeField] private bool pulseScale = true;
    [SerializeField] private Transform followTarget;

    private SpriteRenderer spriteRenderer;
    private Color startColor;
    private Vector3 startScale;
    private float startTime;
    private bool arcMotion;
    private Vector2 arcStart;
    private Vector2 arcEnd;
    private float arcHeight;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        startColor = spriteRenderer.color;
        startScale = transform.localScale;
        startTime = Time.time;
    }

    private void Update()
    {
        float progress = Mathf.Clamp01((Time.time - startTime) / duration);

        if (arcMotion)
        {
            Vector2 arcPosition = Vector2.Lerp(arcStart, arcEnd, progress);
            arcPosition += Vector2.up * (Mathf.Sin(progress * Mathf.PI) * arcHeight);
            transform.position = arcPosition;
        }
        else if (followTarget != null)
        {
            transform.position = followTarget.position;
        }

        if (fadeOut || fadeIn || blink)
        {
            Color color = startColor;
            float alpha = startColor.a;
            if (fadeIn)
            {
                alpha = Mathf.Lerp(0f, startColor.a, progress);
            }
            else if (fadeOut)
            {
                alpha = Mathf.Lerp(startColor.a, 0f, progress);
            }

            if (blink)
            {
                float blinkAlpha = Mathf.Lerp(0.55f, 1f, Mathf.PingPong(Time.time * blinkFrequency, 1f));
                alpha *= blinkAlpha;
            }

            color.a = alpha;
            spriteRenderer.color = color;
        }

        if (pulseScale)
        {
            float pulse = 1f + Mathf.Sin(progress * Mathf.PI) * 0.055f;
            transform.localScale = startScale * pulse;
        }

        if (progress >= 1f)
        {
            Destroy(gameObject);
        }
    }

    public void Initialize(
        float effectDuration,
        bool shouldFadeOut,
        bool shouldPulseScale,
        Transform targetToFollow = null,
        bool shouldFadeIn = false,
        bool shouldBlink = false,
        float effectBlinkFrequency = 8f)
    {
        duration = Mathf.Max(effectDuration, 0.01f);
        fadeOut = shouldFadeOut;
        fadeIn = shouldFadeIn;
        blink = shouldBlink;
        blinkFrequency = Mathf.Max(effectBlinkFrequency, 0.1f);
        pulseScale = shouldPulseScale;
        followTarget = targetToFollow;
        arcMotion = false;
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        startColor = spriteRenderer != null ? spriteRenderer.color : Color.white;
        startScale = transform.localScale;
        startTime = Time.time;
    }

    public void SetArc(Vector2 start, Vector2 end, float height)
    {
        arcMotion = true;
        arcStart = start;
        arcEnd = end;
        arcHeight = Mathf.Max(height, 0f);
        transform.position = start;
    }

    public void SetPosition(Vector2 position)
    {
        transform.position = position;
    }

    public void SetLine(Vector2 start, Vector2 direction, float length, float width)
    {
        direction = direction.sqrMagnitude > 0.0001f ? direction.normalized : Vector2.right;
        transform.position = start + direction * (length * 0.5f);
        transform.right = direction;
        transform.localScale = new Vector3(Mathf.Max(length, 0.01f), Mathf.Max(width, 0.01f), 1f);
        startScale = transform.localScale;
    }

    public static EnemyTelegraph2D CreateCircle(
        string effectName,
        Sprite sprite,
        Vector2 position,
        float radius,
        Color color,
        int sortingOrder,
        float effectDuration,
        Transform targetToFollow = null,
        bool fadeOut = true,
        bool fadeIn = false,
        bool blink = false,
        float blinkFrequency = 8f)
    {
        EnemyTelegraph2D telegraph = CreateBase(effectName, BossRuntimeSprites.Circle, position, color, sortingOrder);
        if (telegraph == null)
        {
            return null;
        }

        float diameter = Mathf.Max(radius * 2f, 0.01f);
        telegraph.transform.localScale = new Vector3(diameter, diameter, 1f);
        telegraph.Initialize(effectDuration, fadeOut, true, targetToFollow, fadeIn, blink, blinkFrequency);
        return telegraph;
    }

    public static EnemyTelegraph2D CreateLine(
        string effectName,
        Sprite sprite,
        Vector2 start,
        Vector2 direction,
        float length,
        float width,
        Color color,
        int sortingOrder,
        float effectDuration)
    {
        EnemyTelegraph2D telegraph = CreateBase(effectName, BossRuntimeSprites.Square, start, color, sortingOrder);
        if (telegraph == null)
        {
            return null;
        }

        telegraph.SetLine(start, direction, length, width);
        telegraph.Initialize(effectDuration, true, false);
        return telegraph;
    }

    public static EnemyTelegraph2D CreateFlash(
        string effectName,
        Sprite sprite,
        Vector2 position,
        Vector2 scale,
        Color color,
        int sortingOrder,
        float effectDuration,
        Transform targetToFollow = null,
        bool blink = false,
        float blinkFrequency = 8f)
    {
        EnemyTelegraph2D telegraph = CreateBase(effectName, BossRuntimeSprites.Circle, position, color, sortingOrder);
        if (telegraph == null)
        {
            return null;
        }

        telegraph.transform.localScale = new Vector3(Mathf.Max(scale.x, 0.01f), Mathf.Max(scale.y, 0.01f), 1f);
        telegraph.Initialize(effectDuration, true, true, targetToFollow, false, blink, blinkFrequency);
        return telegraph;
    }

    public static EnemyTelegraph2D CreateArc(
        string effectName,
        Sprite sprite,
        Vector2 start,
        Vector2 end,
        Vector2 scale,
        float arcHeight,
        Color color,
        int sortingOrder,
        float effectDuration)
    {
        EnemyTelegraph2D telegraph = CreateBase(effectName, BossRuntimeSprites.Circle, start, color, sortingOrder);
        if (telegraph == null)
        {
            return null;
        }

        telegraph.transform.localScale = new Vector3(Mathf.Max(scale.x, 0.01f), Mathf.Max(scale.y, 0.01f), 1f);
        telegraph.Initialize(effectDuration, true, true);
        telegraph.SetArc(start, end, arcHeight);
        return telegraph;
    }

    private static EnemyTelegraph2D CreateBase(string effectName, Sprite sprite, Vector2 position, Color color, int sortingOrder)
    {
        if (sprite == null)
        {
            return null;
        }

        GameObject effectObject = new GameObject(effectName);
        effectObject.transform.position = position;

        SpriteRenderer renderer = effectObject.AddComponent<SpriteRenderer>();
        renderer.sprite = sprite;
        renderer.color = color;
        renderer.sortingOrder = sortingOrder;

        return effectObject.AddComponent<EnemyTelegraph2D>();
    }
}
