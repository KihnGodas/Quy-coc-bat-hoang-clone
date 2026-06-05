using UnityEngine;

public sealed class ArenaBounds : MonoBehaviour
{
    [SerializeField] private Vector2 center;
    [SerializeField] private Vector2 size = new Vector2(35f, 35f);
    [SerializeField] private Color gizmoColor = new Color(0.15f, 0.85f, 1f, 0.8f);

    public static ArenaBounds Instance { get; private set; }

    public Vector2 Center => (Vector2)transform.position + center;
    public Vector2 Size => size;

    public void SetSize(Vector2 newSize)
    {
        size = new Vector2(Mathf.Max(0.1f, newSize.x), Mathf.Max(0.1f, newSize.y));
        UpdateWalls();
    }

    private void UpdateWalls()
    {
        float halfW = size.x * 0.5f;
        float halfH = size.y * 0.5f;

        SetWall("Wall_Top",    new Vector3(0,           halfH + 0.5f, 0), new Vector3(size.x + 1f, 1f, 1f));
        SetWall("Wall_Bottom", new Vector3(0,          -halfH - 0.5f, 0), new Vector3(size.x + 1f, 1f, 1f));
        SetWall("Wall_Left",   new Vector3(-halfW - 0.5f, 0,          0), new Vector3(1f, size.y + 1f, 1f));
        SetWall("Wall_Right",  new Vector3( halfW + 0.5f, 0,          0), new Vector3(1f, size.y + 1f, 1f));
    }

    private void SetWall(string name, Vector3 pos, Vector3 scale)
    {
        Transform t = transform.Find(name);
        if (t != null)
        {
            t.localPosition = pos;
            t.localScale = scale;
        }
    }
    public Vector2 Min => Center - size * 0.5f;
    public Vector2 Max => Center + size * 0.5f;

    private void Awake()
    {
        if (Instance == null || Instance == this)
        {
            Instance = this;
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    private void OnValidate()
    {
        size.x = Mathf.Max(0.1f, size.x);
        size.y = Mathf.Max(0.1f, size.y);
    }

    public Vector2 ClampPosition(Vector2 position, float padding = 0f)
    {
        Vector2 min = Min + Vector2.one * padding;
        Vector2 max = Max - Vector2.one * padding;

        if (min.x > max.x)
        {
            float midX = (min.x + max.x) * 0.5f;
            min.x = midX;
            max.x = midX;
        }

        if (min.y > max.y)
        {
            float midY = (min.y + max.y) * 0.5f;
            min.y = midY;
            max.y = midY;
        }

        return new Vector2(
            Mathf.Clamp(position.x, min.x, max.x),
            Mathf.Clamp(position.y, min.y, max.y));
    }

    public bool Contains(Vector2 position, float padding = 0f)
    {
        Vector2 min = Min + Vector2.one * padding;
        Vector2 max = Max - Vector2.one * padding;

        return position.x >= min.x
            && position.x <= max.x
            && position.y >= min.y
            && position.y <= max.y;
    }

    public bool TryGetWorldBounds(out float minX, out float maxX, out float minY, out float maxY)
    {
        Vector2 min = Min;
        Vector2 max = Max;
        minX = min.x;
        maxX = max.x;
        minY = min.y;
        maxY = max.y;
        return size.x > 0f && size.y > 0f;
    }

    public Vector2 GetRandomPointOnEdge(float padding = 0f)
    {
        Vector2 min = Min + Vector2.one * padding;
        Vector2 max = Max - Vector2.one * padding;
        int edge = Random.Range(0, 4);

        if (edge == 0)
        {
            return new Vector2(Random.Range(min.x, max.x), max.y);
        }

        if (edge == 1)
        {
            return new Vector2(Random.Range(min.x, max.x), min.y);
        }

        if (edge == 2)
        {
            return new Vector2(min.x, Random.Range(min.y, max.y));
        }

        return new Vector2(max.x, Random.Range(min.y, max.y));
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = gizmoColor;
        Gizmos.DrawWireCube(Center, size);
    }
}
