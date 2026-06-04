using UnityEngine;

[RequireComponent(typeof(BossBase))]
public sealed class Act1WoodBossVisual : MonoBehaviour
{
    [SerializeField] private BossBase boss;
    [SerializeField] private Transform visualRoot;
    [SerializeField] private SpriteRenderer shieldRenderer;
    [SerializeField] private Color shieldActiveColor = new Color(0.25f, 1f, 0.45f, 0.28f);
    [SerializeField] private Color shieldBrokenColor = new Color(0.25f, 1f, 0.45f, 0.06f);

    private void Awake()
    {
        boss = GetComponent<BossBase>();
        BuildVisual();
    }

    private void Update()
    {
        if (shieldRenderer == null || boss == null)
        {
            return;
        }

        shieldRenderer.color = boss.IsInvulnerable ? shieldActiveColor : shieldBrokenColor;
        float shieldPulse = 1f + Mathf.Sin(Time.time * 3.5f) * (boss.IsInvulnerable ? 0.08f : 0.03f);
        shieldRenderer.transform.localScale = new Vector3(3.25f, 3.25f, 1f) * shieldPulse;
    }

    private void BuildVisual()
    {
        ClearOldVisual();
        visualRoot = new GameObject("Act1WoodBossVisual").transform;
        visualRoot.SetParent(transform, false);

        shieldRenderer = CreatePart("ShieldAura", BossRuntimeSprites.Circle, Vector2.zero, new Vector2(3.25f, 3.25f), shieldActiveColor, 1);
        CreatePart("RobeBack", BossRuntimeSprites.Circle, new Vector2(0f, -0.1f), new Vector2(1.35f, 1.85f), new Color(0.36f, 0.78f, 0.64f, 1f), 2);
        CreatePart("Body", BossRuntimeSprites.Circle, new Vector2(0f, 0f), new Vector2(0.72f, 1.18f), new Color(0.78f, 0.96f, 0.86f, 1f), 3);
        CreatePart("Face", BossRuntimeSprites.Circle, new Vector2(0f, 0.57f), new Vector2(0.48f, 0.48f), new Color(0.86f, 1f, 0.88f, 1f), 4);
        CreatePart("ChestCore", BossRuntimeSprites.Circle, new Vector2(0f, 0.04f), new Vector2(0.34f, 0.34f), new Color(0.25f, 1f, 0.35f, 0.9f), 5);
        CreatePart("LeftSleeve", BossRuntimeSprites.Square, new Vector2(-0.47f, 0.06f), new Vector2(0.24f, 1.05f), new Color(0.2f, 0.55f, 0.38f, 1f), 4).transform.rotation = Quaternion.Euler(0f, 0f, -18f);
        CreatePart("RightSleeve", BossRuntimeSprites.Square, new Vector2(0.47f, 0.06f), new Vector2(0.24f, 1.05f), new Color(0.2f, 0.55f, 0.38f, 1f), 4).transform.rotation = Quaternion.Euler(0f, 0f, 18f);

        for (int i = 0; i < 7; i++)
        {
            float angle = Mathf.Lerp(-70f, 70f, i / 6f);
            SpriteRenderer root = CreatePart($"CrownRoot_{i}", BossRuntimeSprites.Square, new Vector2(0f, 0.86f), new Vector2(0.08f, 0.72f), new Color(0.29f, 0.19f, 0.09f, 1f), 3);
            root.transform.localRotation = Quaternion.Euler(0f, 0f, angle);
            root.transform.localPosition += root.transform.up * 0.22f;
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
            if (child.name == "Act1WoodBossVisual")
            {
                Destroy(child.gameObject);
            }
        }
    }
}
