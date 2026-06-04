using UnityEngine;

public sealed class SkillIconHUD : MonoBehaviour
{
    [SerializeField] private PlayerDash2D playerDash;
    [SerializeField] private WeaponController weaponController;
    [SerializeField] private PlayerSpellController spellController;
    [SerializeField] private PlayerUltimateController playerUltimate;
    [SerializeField] private PlayerCultivationState cultivationState;

    [Header("Layout")]
    [SerializeField] private float iconSize = 50f;
    [SerializeField] private float iconGap = 8f;
    [SerializeField] private float barBottomOffset = 8f;
    [SerializeField] private int iconFontSize = 13;
    [SerializeField] private int cooldownFontSize = 16;
    [SerializeField] private int keyFontSize = 10;

    [Header("Colors")]
    [SerializeField] private Color dashColor = new Color(0.3f, 0.7f, 1f, 1f);
    [SerializeField] private Color weaponSkillColor = new Color(1f, 0.6f, 0.1f, 1f);
    [SerializeField] private Color ultColor = new Color(1f, 0.2f, 0.2f, 1f);
    [SerializeField] private Color readyBorderColor = new Color(1f, 1f, 1f, 0.85f);
    [SerializeField] private Color cooldownBorderColor = new Color(0.6f, 0.6f, 0.6f, 0.7f);
    [SerializeField] private Color lockedColor = new Color(0.25f, 0.25f, 0.25f, 0.9f);
    [SerializeField] private Color cooldownOverlayColor = new Color(0f, 0f, 0f, 0.6f);
    [SerializeField] private Color textColor = Color.white;
    [SerializeField] private Color cooldownTextColor = Color.yellow;

    private Texture2D whiteTexture;
    private GUIStyle iconStyle;
    private GUIStyle cooldownStyle;
    private GUIStyle keyStyle;
    private float nextResolveTime;
    private const float RESOLVE_INTERVAL = 0.5f;

    private void Awake()
    {
        ResolveReferences();
        InitializeWhiteTexture();
    }

    private void Update()
    {
        if (!ReferencesResolved() && Time.time >= nextResolveTime)
        {
            nextResolveTime = Time.time + RESOLVE_INTERVAL;
            ResolveReferences();
        }
    }

    private void OnDestroy()
    {
        if (whiteTexture != null)
        {
            Destroy(whiteTexture);
        }
    }

    private bool ReferencesResolved()
    {
        return playerDash != null
            && weaponController != null
            && spellController != null
            && playerUltimate != null
            && cultivationState != null;
    }

    private void OnGUI()
    {
        EnsureStyles();
        float totalWidth = iconSize * 4f + iconGap * 3f;
        float startX = (Screen.width - totalWidth) * 0.5f;
        float y = Screen.height - barBottomOffset - iconSize;

        float x = startX;

        DrawDashIcon(x, y);
        x += iconSize + iconGap;

        DrawWeaponSkillIcon(x, y);
        x += iconSize + iconGap;

        DrawSpellIcon(x, y);
        x += iconSize + iconGap;

        DrawUltimateIcon(x, y);
    }

    private void DrawDashIcon(float x, float y)
    {
        bool ready = playerDash != null && playerDash.IsDashReady;
        float cdRemaining = playerDash != null ? playerDash.DashCooldownRemaining : 0f;
        float cdTotal = playerDash != null ? playerDash.DashCooldownTotal : 1f;

        DrawIconBase(x, y, dashColor, ready, "Dash", "Space", cdRemaining, cdTotal, true);
    }

    private void DrawWeaponSkillIcon(float x, float y)
    {
        bool ready = weaponController != null && weaponController.IsSkillReady;
        float cdRemaining = weaponController != null ? weaponController.SkillCooldownRemaining : 0f;
        float cdTotal = weaponController != null ? weaponController.CurrentSkillCooldown : 1f;

        DrawIconBase(x, y, weaponSkillColor, ready, "Skill", "Q", cdRemaining, cdTotal, true);
    }

    private void DrawSpellIcon(float x, float y)
    {
        if (spellController == null) return;

        SpellData data = spellController.GetCurrentSpellData();
        if (data == null) return;

        bool ready = spellController.CanCastCurrentSpell();
        float cdRemainingRaw = spellController.GetCurrentSpellCooldownRemaining();
        float cdTotal = spellController.GetCurrentSpellTotalCooldown();

        string label = GetSpellAbbreviation(data.SpellType);
        DrawIconBase(x, y, data.VisualColor, ready, label, "E", cdRemainingRaw, cdTotal, true);
    }

    private void DrawUltimateIcon(float x, float y)
    {
        bool unlocked = cultivationState == null || cultivationState.CanUseUltimate;
        bool ready = playerUltimate != null && playerUltimate.IsUltimateReady;
        float cdRemaining = playerUltimate != null ? playerUltimate.UltimateCooldownRemaining : 0f;
        float cdTotal = playerUltimate != null ? playerUltimate.CurrentUltimateCooldown : 1f;

        DrawIconBase(x, y, ultColor, ready, "Ult", "R", cdRemaining, cdTotal, unlocked);
    }

    private void DrawIconBase(float x, float y, Color baseColor, bool isReady, string label, string keyText,
        float cdRemaining, float cdTotal, bool isUnlocked)
    {
        Rect iconRect = new Rect(x, y, iconSize, iconSize);

        float cdProgress = cdTotal > 0f ? Mathf.Clamp01(cdRemaining / cdTotal) : 0f;

        if (!isUnlocked)
        {
            GUI.color = lockedColor;
            GUI.DrawTexture(iconRect, whiteTexture);
            GUI.color = Color.white;
        }
        else if (isReady)
        {
            GUI.color = baseColor;
            GUI.DrawTexture(iconRect, whiteTexture);
            GUI.color = Color.white;

            DrawBorder(iconRect, readyBorderColor);
        }
        else
        {
            GUI.color = baseColor;
            GUI.DrawTexture(iconRect, whiteTexture);
            GUI.color = Color.white;

            DrawBorder(iconRect, cooldownBorderColor);

            if (cdProgress > 0f)
            {
                GUI.color = cooldownOverlayColor;
                float overlayHeight = iconSize * cdProgress;
                GUI.DrawTexture(new Rect(x, y, iconSize, overlayHeight), whiteTexture);
                GUI.color = Color.white;
            }
        }

        if (!isUnlocked)
        {
            iconStyle.normal.textColor = new Color(0.5f, 0.5f, 0.5f, 0.6f);
            GUI.Label(iconRect, label, iconStyle);
            iconStyle.normal.textColor = textColor;
        }
        else if (isReady)
        {
            iconStyle.normal.textColor = textColor;
            GUI.Label(iconRect, label, iconStyle);
        }
        else
        {
            iconStyle.normal.textColor = cooldownTextColor;
            GUI.Label(iconRect, label, iconStyle);

            string cdText = $"{Mathf.Max(0f, cdRemaining):0.0}";
            cooldownStyle.normal.textColor = cooldownTextColor;
            GUI.Label(iconRect, cdText, cooldownStyle);

            iconStyle.normal.textColor = textColor;
        }

        keyStyle.normal.textColor = isUnlocked ? new Color(1f, 1f, 1f, 0.8f) : new Color(0.5f, 0.5f, 0.5f, 0.5f);
        GUI.Label(new Rect(x + 2f, y + 2f, iconSize, keyFontSize + 2f), keyText, keyStyle);
    }

    private void DrawBorder(Rect rect, Color color)
    {
        GUI.color = color;
        float t = 2f;
        GUI.DrawTexture(new Rect(rect.x, rect.y, rect.width, t), whiteTexture);
        GUI.DrawTexture(new Rect(rect.x, rect.y + rect.height - t, rect.width, t), whiteTexture);
        GUI.DrawTexture(new Rect(rect.x, rect.y, t, rect.height), whiteTexture);
        GUI.DrawTexture(new Rect(rect.x + rect.width - t, rect.y, t, rect.height), whiteTexture);
        GUI.color = Color.white;
    }

    private static string GetSpellAbbreviation(SpellType type)
    {
        return type switch
        {
            SpellType.WoodVine => "Wood",
            SpellType.Fireball => "Fire",
            SpellType.EarthSpike => "Earth",
            SpellType.WaterArrows => "Water",
            SpellType.MetalBlade => "Metal",
            _ => "Spell"
        };
    }

    private void InitializeWhiteTexture()
    {
        whiteTexture = new Texture2D(1, 1, TextureFormat.RGBA32, false);
        whiteTexture.SetPixel(0, 0, Color.white);
        whiteTexture.Apply();
    }

    private void EnsureStyles()
    {
        if (iconStyle != null && iconStyle.fontSize == iconFontSize) return;

        iconStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = iconFontSize,
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.MiddleCenter
        };
        iconStyle.normal.textColor = textColor;

        cooldownStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = cooldownFontSize,
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.MiddleCenter
        };
        cooldownStyle.normal.textColor = cooldownTextColor;

        keyStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = keyFontSize,
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.UpperLeft
        };
        keyStyle.normal.textColor = new Color(1f, 1f, 1f, 0.8f);
    }

    private void ResolveReferences()
    {
        if (playerDash == null)
            playerDash = FindComponentInScene<PlayerDash2D>();

        if (weaponController == null)
            weaponController = FindComponentInScene<WeaponController>();

        if (spellController == null)
            spellController = FindComponentInScene<PlayerSpellController>();

        if (playerUltimate == null)
            playerUltimate = FindComponentInScene<PlayerUltimateController>();

        if (cultivationState == null)
            cultivationState = FindComponentInScene<PlayerCultivationState>();
    }

    private static T FindComponentInScene<T>() where T : Object
    {
#if UNITY_2023_1_OR_NEWER
        return FindFirstObjectByType<T>();
#else
        return FindObjectOfType<T>();
#endif
    }
}