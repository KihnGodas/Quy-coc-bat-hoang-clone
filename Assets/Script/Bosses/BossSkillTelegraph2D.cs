using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public sealed class BossSkillTelegraph2D : MonoBehaviour
{
    [SerializeField, Min(0.01f)] private float duration = 0.5f;
    [SerializeField] private bool fadeOut = true;
    [SerializeField] private bool pulse = true;
    [SerializeField] private bool blink;
    [SerializeField, Min(0.1f)] private float blinkFrequency = 8f;

    private SpriteRenderer spriteRenderer;
    private Color startColor;
    private Vector3 startScale;
    private float startTime;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        startColor = spriteRenderer.color;
        startScale = transform.localScale;
        startTime = Time.time;
    }

    private void Update()
    {
        float t = Mathf.Clamp01((Time.time - startTime) / duration);
        Color color = startColor;

        if (fadeOut)
        {
            color.a = Mathf.Lerp(startColor.a, 0f, t);
        }

        if (blink)
        {
            color.a *= Mathf.Lerp(0.25f, 1f, Mathf.PingPong(Time.time * blinkFrequency, 1f));
        }

        spriteRenderer.color = color;

        if (pulse)
        {
            float scale = 1f + Mathf.Sin(t * Mathf.PI) * 0.12f;
            transform.localScale = startScale * scale;
        }

        if (t >= 1f)
        {
            Destroy(gameObject);
        }
    }

    public void Initialize(float effectDuration, bool shouldFadeOut, bool shouldPulse, bool shouldBlink)
    {
        duration = Mathf.Max(effectDuration, 0.01f);
        fadeOut = shouldFadeOut;
        pulse = shouldPulse;
        blink = shouldBlink;
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        startColor = spriteRenderer.color;
        startScale = transform.localScale;
        startTime = Time.time;
    }

    public static BossSkillTelegraph2D CreateCircle(string name, Vector2 position, float radius, Color color, float duration, int sortingOrder = 7, bool blink = false)
    {
        BossSkillTelegraph2D telegraph = CreateBase(name, BossRuntimeSprites.Circle, position, color, sortingOrder);
        telegraph.transform.localScale = new Vector3(radius * 2f, radius * 2f, 1f);
        telegraph.Initialize(duration, true, true, blink);
        return telegraph;
    }

    public static BossSkillTelegraph2D CreateRectangle(string name, Vector2 center, Vector2 direction, float length, float width, Color color, float duration, int sortingOrder = 7, bool blink = false)
    {
        direction = direction.sqrMagnitude > 0.0001f ? direction.normalized : Vector2.right;
        BossSkillTelegraph2D telegraph = CreateBase(name, BossRuntimeSprites.Square, center, color, sortingOrder);
        telegraph.transform.right = direction;
        telegraph.transform.localScale = new Vector3(Mathf.Max(length, 0.01f), Mathf.Max(width, 0.01f), 1f);
        telegraph.Initialize(duration, true, false, blink);
        return telegraph;
    }

    private static BossSkillTelegraph2D CreateBase(string name, Sprite sprite, Vector2 position, Color color, int sortingOrder)
    {
        GameObject obj = new GameObject(name);
        obj.transform.position = position;
        SpriteRenderer renderer = obj.AddComponent<SpriteRenderer>();
        renderer.sprite = sprite;
        renderer.color = color;
        renderer.sortingOrder = sortingOrder;
        return obj.AddComponent<BossSkillTelegraph2D>();
    }
}
