using UnityEngine;

public sealed class EnemyVisual2D : MonoBehaviour
{
    private const string VisualPartsRootName = "EnemyVisualParts";

    [SerializeField] private EnemyBase enemyBase;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private BoxCollider2D boxCollider;
    [SerializeField] private bool showNameLabel = true;

    private Transform visualPartsRoot;

    private void Awake()
    {
        ResolveReferences();
        ApplyVisual();
    }

    public void OnEnemyDataChanged(EnemyData enemyData)
    {
        ApplyVisual(enemyData);
    }

    public void ApplyVisual()
    {
        ResolveReferences();
        ApplyVisual(enemyBase != null ? enemyBase.Data : null);
    }

    private void ApplyVisual(EnemyData enemyData)
    {
        if (enemyData == null)
        {
            return;
        }

        if (spriteRenderer != null)
        {
            spriteRenderer.color = enemyData.VisualColor;
        }

        Vector2 visualScale = enemyData.VisualScale;
        if (visualScale.x > 0f && visualScale.y > 0f)
        {
            transform.localScale = new Vector3(visualScale.x, visualScale.y, transform.localScale.z);
        }

        if (boxCollider != null)
        {
            if (enemyData.ColliderSize.x > 0f && enemyData.ColliderSize.y > 0f)
            {
                boxCollider.size = enemyData.ColliderSize;
            }

            boxCollider.offset = enemyData.ColliderOffset;
        }

        RebuildRoleSilhouette(enemyData);
    }

    private void RebuildRoleSilhouette(EnemyData enemyData)
    {
        ClearVisualParts();

        if (spriteRenderer == null || spriteRenderer.sprite == null)
        {
            return;
        }

        string enemyName = enemyData.EnemyName;
        if (ContainsName(enemyName, "Hac"))
        {
            CreatePart("RedEyeLeft", new Vector2(-0.2f, 0.2f), new Vector2(0.18f, 0.09f), new Color(1f, 0f, 0f, 1f), 3);
            CreatePart("RedEyeRight", new Vector2(0.2f, 0.2f), new Vector2(0.18f, 0.09f), new Color(1f, 0f, 0f, 1f), 3);
            CreatePart("DarkTail", new Vector2(-0.62f, -0.08f), new Vector2(0.55f, 0.18f), new Color(0f, 0f, 0f, 1f), -1);
            CreatePart("ShadowBack", new Vector2(0f, -0.08f), new Vector2(0.95f, 0.3f), new Color(0f, 0f, 0f, 0.7f), 2);
            CreateNameLabel("Hac Lang", new Color(1f, 0.18f, 0.16f, 1f));
            return;
        }

        if (ContainsName(enemyName, "Dai"))
        {
            Color hornColor = new Color(1f, 0.88f, 0.42f, 1f);
            CreatePart("HornLeft", new Vector2(-0.48f, 0.58f), new Vector2(0.34f, 0.52f), hornColor, 2);
            CreatePart("HornRight", new Vector2(0.48f, 0.58f), new Vector2(0.34f, 0.52f), hornColor, 2);
            CreatePart("HeavyShoulderLeft", new Vector2(-0.6f, 0f), new Vector2(0.32f, 0.55f), new Color(0.45f, 0f, 0f, 1f), 1);
            CreatePart("HeavyShoulderRight", new Vector2(0.6f, 0f), new Vector2(0.32f, 0.55f), new Color(0.45f, 0f, 0f, 1f), 1);
            CreatePart("ChargeCore", new Vector2(0f, 0f), new Vector2(0.62f, 0.62f), new Color(1f, 0.35f, 0f, 1f), 3);
            CreateNameLabel("Dai Lang", new Color(1f, 0.72f, 0.2f, 1f));
            return;
        }

        if (ContainsName(enemyName, "Thiet"))
        {
            Color armorColor = new Color(0.82f, 0.9f, 0.95f, 1f);
            CreatePart("ArmorBand", new Vector2(0f, 0.08f), new Vector2(1.3f, 0.24f), armorColor, 3);
            CreatePart("ArmorPlate", new Vector2(0f, -0.24f), new Vector2(1f, 0.22f), new Color(0.18f, 0.22f, 0.27f, 1f), 3);
            CreatePart("IronSnout", new Vector2(0f, 0.36f), new Vector2(0.42f, 0.24f), new Color(0.9f, 0.95f, 1f, 1f), 3);
            CreateNameLabel("Thiet Tru", new Color(0.82f, 0.92f, 1f, 1f));
            return;
        }

        if (ContainsName(enemyName, "Oai"))
        {
            Color maneColor = new Color(1f, 0.82f, 0.18f, 1f);
            CreatePart("ManeTop", new Vector2(0f, 0.46f), new Vector2(1.05f, 0.34f), maneColor, -1);
            CreatePart("ManeLeft", new Vector2(-0.48f, 0f), new Vector2(0.28f, 0.88f), maneColor, -1);
            CreatePart("ManeRight", new Vector2(0.48f, 0f), new Vector2(0.28f, 0.88f), maneColor, -1);
            CreatePart("BerserkCore", new Vector2(0f, 0f), new Vector2(0.46f, 0.46f), new Color(1f, 0.05f, 0f, 1f), 3);
            CreateNameLabel("Oai Hung", new Color(1f, 0.82f, 0.18f, 1f));
            return;
        }

        if (ContainsName(enemyName, "Kim"))
        {
            Color handColor = new Color(1f, 0.93f, 0.36f, 1f);
            CreatePart("ThrowHandLeft", new Vector2(-0.58f, -0.02f), new Vector2(0.38f, 0.38f), handColor, 2);
            CreatePart("ThrowHandRight", new Vector2(0.58f, -0.02f), new Vector2(0.38f, 0.38f), handColor, 2);
            CreatePart("StonePack", new Vector2(0f, 0.52f), new Vector2(0.48f, 0.48f), new Color(0.36f, 0.36f, 0.34f, 1f), 3);
            CreatePart("GoldBelt", new Vector2(0f, -0.28f), new Vector2(0.95f, 0.16f), new Color(1f, 0.85f, 0.08f, 1f), 3);
            CreateNameLabel("Kim Vien", new Color(1f, 0.9f, 0.22f, 1f));
            return;
        }

        if (ContainsName(enemyName, "Xa"))
        {
            CreatePart("SnakeHead", new Vector2(0f, 0.52f), new Vector2(0.55f, 0.34f), new Color(0.75f, 1f, 0.12f, 1f), 3);
            CreatePart("SnakeTailOne", new Vector2(0f, -0.56f), new Vector2(0.34f, 0.58f), new Color(0.04f, 0.58f, 0.1f, 1f), -1);
            CreatePart("SnakeTailTwo", new Vector2(0.2f, -0.9f), new Vector2(0.26f, 0.42f), new Color(0.03f, 0.42f, 0.08f, 1f), -1);
            CreatePart("VenomMark", new Vector2(0f, 0f), new Vector2(0.28f, 0.72f), new Color(0.86f, 1f, 0.05f, 1f), 3);
            CreateNameLabel("Xa Yeu", new Color(0.75f, 1f, 0.12f, 1f));
            return;
        }

        if (ContainsName(enemyName, "Ung"))
        {
            Color wingColor = new Color(0.38f, 0.85f, 1f, 1f);
            CreatePart("WingLeft", new Vector2(-0.78f, 0.04f), new Vector2(0.75f, 0.28f), wingColor, -1);
            CreatePart("WingRight", new Vector2(0.78f, 0.04f), new Vector2(0.75f, 0.28f), wingColor, -1);
            CreatePart("Beak", new Vector2(0f, 0.46f), new Vector2(0.24f, 0.3f), new Color(1f, 0.78f, 0f, 1f), 3);
            CreatePart("SkyCore", new Vector2(0f, -0.1f), new Vector2(0.38f, 0.38f), new Color(0.08f, 0.35f, 1f, 1f), 3);
            CreateNameLabel("Ung Yeu", new Color(0.38f, 0.85f, 1f, 1f));
            return;
        }

        if (ContainsName(enemyName, "Moc"))
        {
            Color branchColor = new Color(0.46f, 0.28f, 0.13f, 1f);
            CreatePart("BranchLeft", new Vector2(-0.44f, 0.48f), new Vector2(0.22f, 0.68f), branchColor, 2);
            CreatePart("BranchRight", new Vector2(0.44f, 0.48f), new Vector2(0.22f, 0.68f), branchColor, 2);
            CreatePart("LeafTop", new Vector2(0f, 0.82f), new Vector2(0.55f, 0.3f), new Color(0.2f, 1f, 0.22f, 1f), 3);
            CreatePart("RootCore", new Vector2(0f, -0.44f), new Vector2(0.92f, 0.26f), new Color(0.1f, 1f, 0.22f, 1f), 3);
            CreateNameLabel("Moc Yeu", new Color(0.28f, 1f, 0.35f, 1f));
            return;
        }

        CreatePart("KhoiMark", new Vector2(0f, 0.28f), new Vector2(0.36f, 0.14f), new Color(1f, 1f, 1f, 1f), 3);
        CreatePart("KhoiFangLeft", new Vector2(-0.2f, -0.35f), new Vector2(0.12f, 0.22f), new Color(1f, 1f, 1f, 1f), 3);
        CreatePart("KhoiFangRight", new Vector2(0.2f, -0.35f), new Vector2(0.12f, 0.22f), new Color(1f, 1f, 1f, 1f), 3);
        CreateNameLabel("Khoi Lang", Color.white);
    }

    private void ClearVisualParts()
    {
        Transform existingRoot = transform.Find(VisualPartsRootName);
        if (existingRoot == null)
        {
            visualPartsRoot = null;
            return;
        }

        for (int i = existingRoot.childCount - 1; i >= 0; i--)
        {
            Transform child = existingRoot.GetChild(i);
            if (Application.isPlaying)
            {
                Destroy(child.gameObject);
            }
            else
            {
                DestroyImmediate(child.gameObject);
            }
        }

        visualPartsRoot = existingRoot;
    }

    private void CreatePart(string partName, Vector2 localPosition, Vector2 localScale, Color color, int sortingOffset)
    {
        EnsureVisualPartsRoot();

        GameObject partObject = new GameObject(partName);
        partObject.transform.SetParent(visualPartsRoot, false);
        partObject.transform.localPosition = new Vector3(localPosition.x, localPosition.y, 0f);
        partObject.transform.localScale = new Vector3(localScale.x, localScale.y, 1f);

        SpriteRenderer partRenderer = partObject.AddComponent<SpriteRenderer>();
        partRenderer.sprite = spriteRenderer.sprite;
        partRenderer.sharedMaterial = spriteRenderer.sharedMaterial;
        partRenderer.color = color;
        partRenderer.sortingLayerID = spriteRenderer.sortingLayerID;
        partRenderer.sortingOrder = spriteRenderer.sortingOrder + sortingOffset;
    }

    private void CreateNameLabel(string label, Color color)
    {
        if (!showNameLabel)
        {
            return;
        }

        EnsureVisualPartsRoot();

        GameObject labelObject = new GameObject("NameLabel");
        labelObject.transform.SetParent(visualPartsRoot, false);
        labelObject.transform.localPosition = new Vector3(0f, 1.05f, 0f);
        labelObject.transform.localScale = Vector3.one * 0.12f;

        TextMesh textMesh = labelObject.AddComponent<TextMesh>();
        textMesh.text = label;
        textMesh.anchor = TextAnchor.MiddleCenter;
        textMesh.alignment = TextAlignment.Center;
        textMesh.fontSize = 20;
        textMesh.color = color;

        MeshRenderer meshRenderer = labelObject.GetComponent<MeshRenderer>();
        if (meshRenderer != null)
        {
            meshRenderer.sortingLayerID = spriteRenderer.sortingLayerID;
            meshRenderer.sortingOrder = spriteRenderer.sortingOrder + 10;
        }
    }

    private void EnsureVisualPartsRoot()
    {
        if (visualPartsRoot != null)
        {
            return;
        }

        Transform existingRoot = transform.Find(VisualPartsRootName);
        if (existingRoot != null)
        {
            visualPartsRoot = existingRoot;
            return;
        }

        GameObject rootObject = new GameObject(VisualPartsRootName);
        rootObject.transform.SetParent(transform, false);
        rootObject.transform.localPosition = Vector3.zero;
        rootObject.transform.localScale = Vector3.one;
        visualPartsRoot = rootObject.transform;
    }

    private static bool ContainsName(string source, string keyword)
    {
        return !string.IsNullOrEmpty(source)
            && source.IndexOf(keyword, System.StringComparison.OrdinalIgnoreCase) >= 0;
    }

    private void ResolveReferences()
    {
        if (enemyBase == null)
        {
            enemyBase = GetComponent<EnemyBase>();
        }

        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }

        if (boxCollider == null)
        {
            boxCollider = GetComponent<BoxCollider2D>();
        }
    }
}
