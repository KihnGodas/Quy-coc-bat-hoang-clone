using UnityEngine;

[RequireComponent(typeof(BossBase))]
public sealed class TutorialBossVisual : MonoBehaviour
{
    [SerializeField] private BossBase boss;
    [SerializeField] private Transform visualRoot;
    [SerializeField] private SpriteRenderer haloRenderer;
    [SerializeField] private SpriteRenderer sealRenderer;
    [SerializeField] private SpriteRenderer coreRenderer;

    private void Awake()
    {
        boss = GetComponent<BossBase>();
        BuildVisual();
    }

    private void Update()
    {
        float pulse = 1f + Mathf.Sin(Time.time * 3.2f) * 0.06f;
        float fastPulse = 1f + Mathf.Sin(Time.time * 8.5f) * 0.1f;

        if (haloRenderer != null)
        {
            haloRenderer.transform.localScale = new Vector3(3.35f, 3.35f, 1f) * pulse;
            haloRenderer.color = new Color(0.55f, 0.84f, 1f, 0.18f + Mathf.Sin(Time.time * 2.4f) * 0.04f);
        }

        if (sealRenderer != null)
        {
            sealRenderer.transform.Rotate(0f, 0f, 22f * Time.deltaTime);
        }

        if (coreRenderer != null)
        {
            coreRenderer.transform.localScale = new Vector3(0.32f, 0.32f, 1f) * fastPulse;
        }
    }

    private void BuildVisual()
    {
        ClearOldVisual();
        visualRoot = new GameObject("TutorialBossVisual").transform;
        visualRoot.SetParent(transform, false);

        haloRenderer = CreatePart("HeavenHalo", BossRuntimeSprites.Circle, Vector2.zero, new Vector2(3.35f, 3.35f), new Color(0.55f, 0.84f, 1f, 0.2f), 1);
        sealRenderer = CreatePart("DaoSeal", BossRuntimeSprites.Square, Vector2.zero, new Vector2(1.72f, 1.72f), new Color(0.34f, 0.72f, 1f, 0.28f), 2);
        CreatePart("OuterRobe", BossRuntimeSprites.Circle, new Vector2(0f, -0.1f), new Vector2(1.22f, 1.8f), new Color(0.82f, 0.93f, 1f, 1f), 3);
        CreatePart("InnerRobe", BossRuntimeSprites.Circle, new Vector2(0f, -0.02f), new Vector2(0.72f, 1.28f), new Color(0.98f, 0.98f, 0.9f, 1f), 4);
        CreatePart("Face", BossRuntimeSprites.Circle, new Vector2(0f, 0.58f), new Vector2(0.45f, 0.45f), new Color(1f, 0.92f, 0.72f, 1f), 5);
        coreRenderer = CreatePart("DaoCore", BossRuntimeSprites.Circle, new Vector2(0f, 0.08f), new Vector2(0.32f, 0.32f), new Color(0.65f, 0.92f, 1f, 1f), 7);
        CreatePart("LeftSleeve", BossRuntimeSprites.Square, new Vector2(-0.5f, 0.04f), new Vector2(0.22f, 1.08f), new Color(0.55f, 0.8f, 1f, 1f), 5).transform.rotation = Quaternion.Euler(0f, 0f, -20f);
        CreatePart("RightSleeve", BossRuntimeSprites.Square, new Vector2(0.5f, 0.04f), new Vector2(0.22f, 1.08f), new Color(0.55f, 0.8f, 1f, 1f), 5).transform.rotation = Quaternion.Euler(0f, 0f, 20f);

        for (int i = 0; i < 6; i++)
        {
            float angle = i * 60f;
            Vector2 offset = Quaternion.Euler(0f, 0f, angle) * Vector2.up * 0.95f;
            SpriteRenderer sword = CreatePart($"FloatingSword_{i}", BossRuntimeSprites.Square, offset, new Vector2(0.08f, 0.58f), new Color(0.72f, 0.9f, 1f, 0.88f), 6);
            sword.transform.localRotation = Quaternion.Euler(0f, 0f, angle);
        }

        CreatePart("Crown", BossRuntimeSprites.Square, new Vector2(0f, 0.92f), new Vector2(0.46f, 0.12f), new Color(1f, 0.88f, 0.38f, 1f), 6);
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
            if (child.name == "TutorialBossVisual")
            {
                Destroy(child.gameObject);
            }
        }
    }
}
