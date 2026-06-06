using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public sealed class BossLineTelegraph2D : MonoBehaviour
{
    private static Material lineMaterial;

    [SerializeField, Min(0.01f)] private float duration = 1f;
    [SerializeField, Min(0.01f)] private float width = 0.12f;
    [SerializeField] private bool blink = true;

    private LineRenderer lineRenderer;
    private Color startColor;
    private float startTime;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        startColor = lineRenderer.startColor;
        startTime = Time.time;
    }

    private void Update()
    {
        float t = Mathf.Clamp01((Time.time - startTime) / duration);
        Color color = startColor;
        color.a = Mathf.Lerp(startColor.a, 0f, t);

        if (blink)
        {
            color.a *= Mathf.Lerp(0.35f, 1f, Mathf.PingPong(Time.time * 10f, 1f));
        }

        lineRenderer.startColor = color;
        lineRenderer.endColor = color;

        if (t >= 1f)
        {
            Destroy(gameObject);
        }
    }

    public void Initialize(Vector2 start, Vector2 end, Color color, float telegraphDuration, float lineWidth, int sortingOrder)
    {
        duration = Mathf.Max(0.01f, telegraphDuration);
        width = Mathf.Max(0.01f, lineWidth);

        if (lineRenderer == null)
        {
            lineRenderer = GetComponent<LineRenderer>();
        }

        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, end);
        lineRenderer.startWidth = width;
        lineRenderer.endWidth = width;
        lineRenderer.startColor = color;
        lineRenderer.endColor = color;
        lineRenderer.material = GetLineMaterial();
        lineRenderer.sortingOrder = sortingOrder;
        lineRenderer.useWorldSpace = true;

        startColor = color;
        startTime = Time.time;
    }

    public static BossLineTelegraph2D Create(string name, Vector2 start, Vector2 end, Color color, float duration, float width = 0.12f, int sortingOrder = 9)
    {
        GameObject obj = new GameObject(name);
        BossLineTelegraph2D telegraph = obj.AddComponent<BossLineTelegraph2D>();
        telegraph.Initialize(start, end, color, duration, width, sortingOrder);
        return telegraph;
    }

    private static Material GetLineMaterial()
    {
        if (lineMaterial != null)
        {
            return lineMaterial;
        }

        Shader shader = Shader.Find("Sprites/Default");
        if (shader == null)
        {
            shader = Shader.Find("Hidden/Internal-Colored");
        }

        lineMaterial = new Material(shader)
        {
            name = "BossLineTelegraphRuntimeMaterial"
        };
        return lineMaterial;
    }
}
