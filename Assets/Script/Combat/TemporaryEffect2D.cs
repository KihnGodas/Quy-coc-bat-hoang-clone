using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public sealed class TemporaryEffect2D : MonoBehaviour
{
    [SerializeField, Min(0.01f)] private float lifetime = 0.25f;
    [SerializeField] private bool fadeOut = true;
    [SerializeField] private bool scaleOut = true;

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
        float progress = Mathf.Clamp01((Time.time - startTime) / lifetime);

        if (fadeOut)
        {
            Color color = startColor;
            color.a = Mathf.Lerp(startColor.a, 0f, progress);
            spriteRenderer.color = color;
        }

        if (scaleOut)
        {
            transform.localScale = Vector3.Lerp(startScale, startScale * 1.4f, progress);
        }

        if (progress >= 1f)
        {
            Destroy(gameObject);
        }
    }

    public void Initialize(float effectLifetime, bool shouldFadeOut, bool shouldScaleOut)
    {
        lifetime = Mathf.Max(effectLifetime, 0.01f);
        fadeOut = shouldFadeOut;
        scaleOut = shouldScaleOut;
    }

    public static TemporaryEffect2D CreateSpriteEffect(
        string effectName,
        Sprite sprite,
        Vector3 position,
        Quaternion rotation,
        Vector3 scale,
        Color color,
        int sortingOrder,
        float effectLifetime)
    {
        if (sprite == null)
        {
            return null;
        }

        GameObject effectObject = new GameObject(effectName);
        effectObject.transform.position = position;
        effectObject.transform.rotation = rotation;
        effectObject.transform.localScale = scale;

        SpriteRenderer renderer = effectObject.AddComponent<SpriteRenderer>();
        renderer.sprite = sprite;
        renderer.color = color;
        renderer.sortingOrder = sortingOrder;

        TemporaryEffect2D effect = effectObject.AddComponent<TemporaryEffect2D>();
        effect.Initialize(effectLifetime, true, true);
        return effect;
    }
}
