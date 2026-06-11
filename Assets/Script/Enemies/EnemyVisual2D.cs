using UnityEngine;

public sealed class EnemyVisual2D : MonoBehaviour
{
    private const string VisualPartsRootName = "EnemyVisualParts";
    private static float globalSizeMultiplier = 1f;
    private const float IdleAnimSpeedMultiplier = 0.3f;

    [SerializeField] private EnemyBase enemyBase;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private BoxCollider2D boxCollider;
    [SerializeField] private bool showNameLabel = true;

    private Transform visualPartsRoot;
    private EnemyData currentEnemyData;
    private Sprite[] activeWalkFrames;
    private Sprite[] activeAttackFrames;
    private Sprite[] currentAnimationFrames;
    private Transform shadowTransform;
    private Vector3 baseLocalScale = Vector3.one;
    private int walkFrameIndex;
    private float walkFrameTimer;

    public static void SetGlobalSizeMultiplier(float sizeMultiplier)
    {
        globalSizeMultiplier = Mathf.Max(0.1f, sizeMultiplier);
    }

    private void Awake()
    {
        baseLocalScale = transform.localScale;
        ResolveReferences();
        ApplyVisual();
    }

    private void OnEnable()
    {
        ApplyVisual();
    }

    public void OnEnemyDataChanged(EnemyData enemyData)
    {
        ApplyVisual(enemyData);
    }

    private void Update()
    {
        UpdateWalkAnimation();
        UpdateFacingDirection();
    }

    public void ApplyVisual()
    {
        ResolveReferences();
        ApplyVisual(enemyBase != null ? enemyBase.Data : null);
    }

    private void ApplyVisual(EnemyData enemyData)
    {
        ResolveReferences();
        ResetAnimationState();

        if (spriteRenderer == null)
        {
            return;
        }

        if (enemyData == null)
        {
            ApplyFallbackVisual();
            return;
        }

        currentEnemyData = enemyData;
        activeWalkFrames = enemyData.WalkFrames;
        activeAttackFrames = enemyData.AttackFrames;

        spriteRenderer.color = enemyData.VisualColor;

        if (boxCollider != null)
        {
            Vector2 targetColliderSize = GetEffectiveColliderSize(enemyData);
            if (targetColliderSize.x > 0f && targetColliderSize.y > 0f)
            {
                boxCollider.size = targetColliderSize;
            }

            boxCollider.offset = enemyData.ColliderOffset;
        }

        if (enemyData.HasAnyAnimation)
        {
            ClearVisualParts();
            if (!SetAnimationFrame(GetDefaultFrames(), 0))
            {
                ApplyFallbackSprite(enemyData);
            }

            CreateGroundShadow(enemyData);
            return;
        }

        if (spriteRenderer.sprite == null)
        {
            ApplyFallbackSprite(enemyData);
        }

        ApplyVisualSize();
        RebuildRoleSilhouette(enemyData);
    }

    private void UpdateWalkAnimation()
    {
        if (spriteRenderer == null || currentEnemyData == null)
        {
            return;
        }

        Sprite[] targetFrames = GetFramesForState(out bool shouldAnimate, out float frameRate);
        if (targetFrames == null || targetFrames.Length == 0)
        {
            return;
        }

        if (currentAnimationFrames != targetFrames)
        {
            currentAnimationFrames = targetFrames;
            if (!SetAnimationFrame(targetFrames, 0))
            {
                ApplyFallbackSprite(currentEnemyData);
            }

            walkFrameTimer = 0f;
        }

        if (!shouldAnimate)
        {
            if (!SetAnimationFrame(targetFrames, 0))
            {
                ApplyFallbackSprite(currentEnemyData);
            }

            walkFrameTimer = 0f;
            return;
        }

        float frameDuration = 1f / Mathf.Max(0.1f, frameRate);
        walkFrameTimer += Time.deltaTime;

        while (walkFrameTimer >= frameDuration)
        {
            walkFrameTimer -= frameDuration;
            if (!SetAnimationFrame(targetFrames, walkFrameIndex + 1))
            {
                ApplyFallbackSprite(currentEnemyData);
                return;
            }
        }
    }

    private void UpdateFacingDirection()
    {
        if (spriteRenderer == null || !HasAnyActiveAnimation()
            || enemyBase == null || enemyBase.Target == null)
        {
            return;
        }

        float deltaX = enemyBase.Target.position.x - transform.position.x;
        if (Mathf.Abs(deltaX) > 0.01f)
        {
            spriteRenderer.flipX = deltaX < 0f;
        }
    }

    private Sprite[] GetFramesForState(out bool shouldAnimate, out float frameRate)
    {
        EnemyState state = enemyBase != null ? enemyBase.State : EnemyState.Idle;
        if (state == EnemyState.Attack && activeAttackFrames != null && activeAttackFrames.Length > 0)
        {
            shouldAnimate = true;
            frameRate = currentEnemyData.AttackFrameRate;
            return activeAttackFrames;
        }

        shouldAnimate = state == EnemyState.Chase || state == EnemyState.Idle;
        frameRate = currentEnemyData.WalkFrameRate;
        if (state == EnemyState.Idle)
            frameRate *= IdleAnimSpeedMultiplier;
        return GetDefaultFrames();
    }

    private Sprite[] GetDefaultFrames()
    {
        if (activeWalkFrames != null && activeWalkFrames.Length > 0)
        {
            return activeWalkFrames;
        }

        return activeAttackFrames;
    }

    private bool HasAnyActiveAnimation()
    {
        return (activeWalkFrames != null && activeWalkFrames.Length > 0)
            || (activeAttackFrames != null && activeAttackFrames.Length > 0);
    }

    private bool SetAnimationFrame(Sprite[] frames, int frameIndex)
    {
        if (spriteRenderer == null || frames == null || frames.Length == 0)
        {
            return false;
        }

        walkFrameIndex = frameIndex % frames.Length;
        Sprite frame = frames[walkFrameIndex];
        if (frame == null)
        {
            return false;
        }

        spriteRenderer.sprite = frame;
        ApplyVisualSize();
        UpdateGroundShadowSize();
        return true;
    }

    private void ApplyVisualSize()
    {
        if (currentEnemyData == null || spriteRenderer == null || spriteRenderer.sprite == null)
        {
            return;
        }

        Vector2 targetSize = GetEffectiveVisualSize(currentEnemyData);
        if (targetSize.x <= 0f || targetSize.y <= 0f)
        {
            return;
        }

        Vector2 spriteSize = spriteRenderer.sprite.bounds.size;
        if (spriteSize.x <= 0f || spriteSize.y <= 0f)
        {
            return;
        }

        float uniformScale = Mathf.Min(targetSize.x / spriteSize.x, targetSize.y / spriteSize.y);
        transform.localScale = new Vector3(uniformScale, uniformScale, baseLocalScale.z);
    }

    private void CreateGroundShadow(EnemyData enemyData)
    {
        EnsureVisualPartsRoot();

        GameObject shadowObject = new GameObject("EnemyGroundShadow");
        shadowTransform = shadowObject.transform;
        shadowTransform.SetParent(visualPartsRoot, false);

        SpriteRenderer shadowRenderer = shadowObject.AddComponent<SpriteRenderer>();
        shadowRenderer.sprite = BossRuntimeSprites.Circle;
        shadowRenderer.color = new Color(0f, 0f, 0f, 0.22f);
        shadowRenderer.sortingLayerID = spriteRenderer != null ? spriteRenderer.sortingLayerID : 0;
        shadowRenderer.sortingOrder = spriteRenderer != null ? spriteRenderer.sortingOrder - 2 : -2;

        UpdateGroundShadowSize();
    }

    private void UpdateGroundShadowSize()
    {
        if (shadowTransform == null || currentEnemyData == null)
        {
            return;
        }

        float parentScale = Mathf.Max(Mathf.Abs(transform.localScale.x), 0.01f);
        Vector2 colliderSize = GetEffectiveColliderSize(currentEnemyData);
        shadowTransform.localPosition = new Vector3(0f, -0.42f / parentScale, 0f);
        shadowTransform.localScale = new Vector3(
            Mathf.Max(colliderSize.x * 0.86f, 0.45f) / parentScale,
            Mathf.Max(colliderSize.y * 0.18f, 0.1f) / parentScale,
            1f);
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
            shadowTransform = null;
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
        shadowTransform = null;
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

    private void ResetAnimationState()
    {
        currentAnimationFrames = null;
        walkFrameIndex = 0;
        walkFrameTimer = 0f;
        activeWalkFrames = null;
        activeAttackFrames = null;
    }

    private void ApplyFallbackVisual()
    {
        currentEnemyData = null;
        ClearVisualParts();
        spriteRenderer.color = Color.white;
        spriteRenderer.flipX = false;

        if (spriteRenderer.sprite == null)
        {
            spriteRenderer.sprite = BossRuntimeSprites.Circle;
        }

        transform.localScale = new Vector3(0.9f, 0.9f, baseLocalScale.z);
    }

    private void ApplyFallbackSprite(EnemyData enemyData)
    {
        if (spriteRenderer == null)
        {
            return;
        }

        spriteRenderer.sprite = BossRuntimeSprites.Circle;
        spriteRenderer.color = enemyData != null ? enemyData.VisualColor : Color.white;
        ApplyVisualSize();
        UpdateGroundShadowSize();
    }

    private static Vector2 GetEffectiveVisualSize(EnemyData enemyData)
    {
        return enemyData != null
            ? enemyData.VisualScale * enemyData.VisualFrameScale * globalSizeMultiplier
            : Vector2.one * globalSizeMultiplier;
    }

    private static Vector2 GetEffectiveColliderSize(EnemyData enemyData)
    {
        return enemyData != null
            ? enemyData.ColliderSize * globalSizeMultiplier
            : Vector2.one * globalSizeMultiplier;
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
