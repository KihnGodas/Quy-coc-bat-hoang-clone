using UnityEngine;

[RequireComponent(typeof(BossBase))]
public sealed class Act2FireBossVisual : MonoBehaviour
{
    [SerializeField] private BossBase boss;
    [SerializeField] private Transform visualRoot;
    [SerializeField] private SpriteRenderer auraRenderer;
    [SerializeField] private SpriteRenderer coreRenderer;
    [SerializeField] private SpriteRenderer crownRenderer;

    private void Awake()
    {
        boss = GetComponent<BossBase>();
        BuildVisual();
    }

    private void Update()
    {
        float hp01 = boss != null ? boss.HealthPercent : 1f;
        float pulse = 1f + Mathf.Sin(Time.time * 7f) * 0.08f;
        float enrage = hp01 <= 0.45f ? 1f : 0f;

        if (auraRenderer != null)
        {
            auraRenderer.color = Color.Lerp(new Color(1f, 0.25f, 0.04f, 0.18f), new Color(1f, 0.02f, 0.02f, 0.35f), enrage);
            auraRenderer.transform.localScale = new Vector3(3.1f, 3.1f, 1f) * pulse;
        }

        if (coreRenderer != null)
        {
            coreRenderer.color = Color.Lerp(new Color(1f, 0.86f, 0.15f, 1f), new Color(1f, 0.1f, 0.02f, 1f), enrage);
            coreRenderer.transform.localScale = new Vector3(0.42f, 0.42f, 1f) * (1f + Mathf.Sin(Time.time * 12f) * 0.16f);
        }

        if (crownRenderer != null)
        {
            crownRenderer.transform.localRotation = Quaternion.Euler(0f, 0f, Mathf.Sin(Time.time * 4.5f) * 9f);
        }
    }

    private void BuildVisual()
    {
        ClearOldVisual();
        visualRoot = new GameObject("Act2FireBossVisual").transform;
        visualRoot.SetParent(transform, false);

        auraRenderer = CreatePart("FireAura", BossRuntimeSprites.Circle, Vector2.zero, new Vector2(3.1f, 3.1f), new Color(1f, 0.25f, 0.04f, 0.18f), 1);
        CreatePart("AshShadow", BossRuntimeSprites.Circle, new Vector2(0f, -0.23f), new Vector2(1.5f, 1.95f), new Color(0.24f, 0.05f, 0.04f, 1f), 2);
        CreatePart("Mantle", BossRuntimeSprites.Circle, new Vector2(0f, -0.03f), new Vector2(1.15f, 1.6f), new Color(0.72f, 0.11f, 0.04f, 1f), 3);
        CreatePart("Face", BossRuntimeSprites.Circle, new Vector2(0f, 0.56f), new Vector2(0.5f, 0.5f), new Color(1f, 0.64f, 0.22f, 1f), 5);
        coreRenderer = CreatePart("BlazingCore", BossRuntimeSprites.Circle, new Vector2(0f, 0.02f), new Vector2(0.42f, 0.42f), new Color(1f, 0.86f, 0.15f, 1f), 6);
        CreatePart("LeftFlameSleeve", BossRuntimeSprites.Square, new Vector2(-0.52f, 0.02f), new Vector2(0.25f, 1.12f), new Color(0.95f, 0.23f, 0.03f, 1f), 4).transform.rotation = Quaternion.Euler(0f, 0f, -24f);
        CreatePart("RightFlameSleeve", BossRuntimeSprites.Square, new Vector2(0.52f, 0.02f), new Vector2(0.25f, 1.12f), new Color(0.95f, 0.23f, 0.03f, 1f), 4).transform.rotation = Quaternion.Euler(0f, 0f, 24f);
        crownRenderer = CreatePart("FlameCrownCenter", BossRuntimeSprites.Square, new Vector2(0f, 0.95f), new Vector2(0.22f, 0.92f), new Color(1f, 0.44f, 0.02f, 1f), 5);

        for (int i = 0; i < 8; i++)
        {
            float angle = i * 45f;
            Vector2 offset = Quaternion.Euler(0f, 0f, angle) * Vector2.up * 0.92f;
            SpriteRenderer ember = CreatePart($"OrbitEmber_{i}", BossRuntimeSprites.Circle, offset, new Vector2(0.14f, 0.14f), new Color(1f, 0.58f, 0.06f, 0.9f), 6);
            ember.transform.localRotation = Quaternion.Euler(0f, 0f, angle);
        }
    }

    private SpriteRenderer CreatePart(string partName, Sprite sprite, Vector2 localPosition, Vector2 localScale, Color color, int sortingOrder)
    {
        GameObject part = new GameObject(partName);
        part.transform.SetParent(visualRoot, false);
        part.transform.localPosition = localPosition;
        part.transform.localScale = new Vector3(localScale.x, localScale.y, 1f);

        SpriteRenderer renderer = part.AddComponent<SpriteRenderer>();
        renderer.sprite = sprite;
        renderer.color = color;
        renderer.sortingOrder = sortingOrder;
        return renderer;
    }

    private void ClearOldVisual()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Transform child = transform.GetChild(i);
            if (child.name == "Act2FireBossVisual")
            {
                Destroy(child.gameObject);
            }
        }
    }
}
