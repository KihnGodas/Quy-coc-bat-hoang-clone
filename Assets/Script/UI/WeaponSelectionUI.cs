using System.Collections.Generic;
using UnityEngine;

public sealed class WeaponSelectionUI : MonoBehaviour
{
    public MainMenuUI mainMenuUI;
    public Texture2D swordIcon;
    public Texture2D spearIcon;
    public Texture2D axeIcon;
    public Texture2D flyingSwordIcon;
    public Texture2D swordBg;
    public Texture2D spearBg;
    public Texture2D axeBg;
    public Texture2D flyingSwordBg;

    [Header("Card Settings")]
    [SerializeField] private float cardWidth = 180f;
    [SerializeField] private float cardHeight = 130f;
    [SerializeField] private float cardSpacing = 25f;
    [SerializeField] private float cardY = 110f;
    [SerializeField] private float nameLabelYOffset = 5f;
    [SerializeField] private float nameLabelHeight = 28f;

    [Header("Stats Box Settings")]
    [SerializeField] private float statsYOffset = 30f;
    [SerializeField] private float statsBoxWidth = 560f;
    [SerializeField] private float statsBoxHeight = 240f;
    [SerializeField] private float statsBoxPadding = 15f;
    [SerializeField] private float statsHeaderHeight = 35f;
    [SerializeField] private float statsHeaderYOffset = 40f;
    [SerializeField] private float statsRowHeight = 28f;
    [SerializeField] private float statsRowSpacing = 32f;
    [Range(0f, 1f)]
    [SerializeField] private float statsLabelWidthRatio = 0.4f;

    [Header("Button Settings")]
    [SerializeField] private float buttonYOffset = 90f;
    [SerializeField] private float buttonWidth = 220f;
    [SerializeField] private float buttonHeight = 55f;

    [Header("Title Settings")]
    [SerializeField] private float titleY = 25f;
    [SerializeField] private float titleWidth = 500f;
    [SerializeField] private float titleHeight = 60f;

    [Header("Colors")]
    [SerializeField] private Color weaponNameColor = Color.white;

    private Texture2D layer1Texture;
    private Texture2D layer2Texture;
    private List<WeaponData> weapons;
    private int selectedIndex = -1;
    private readonly Dictionary<WeaponType, Texture2D> iconTextures = new Dictionary<WeaponType, Texture2D>();
    private readonly Dictionary<WeaponType, Texture2D> bgTextures = new Dictionary<WeaponType, Texture2D>();
    private Texture2D statsBoxBg;
    private bool isShown;

    private float scaleFactor;
    private float lastStyleScaleFactor;
    private const float REF_WIDTH = 1920f;
    private const float REF_HEIGHT = 1080f;

    private GUIStyle titleStyle;
    private GUIStyle weaponNameStyle;
    private GUIStyle statsLabelStyle;
    private GUIStyle statsValueStyle;
    private GUIStyle buttonStyle;
    private GUIStyle statsHeaderStyle;

    private void Awake()
    {
        layer1Texture = Resources.Load<Texture2D>("WeaponSelect/layer1");
        layer2Texture = Resources.Load<Texture2D>("WeaponSelect/layer2");
        weapons = WeaponController.CreateDefaultWeaponData();
        CacheIconTextures();
        CacheBgTextures();
        CreateStatsBoxBg();
    }

    private void UpdateScaleFactor()
    {
        float scaleX = Screen.width / REF_WIDTH;
        float scaleY = Screen.height / REF_HEIGHT;
        scaleFactor = Mathf.Min(scaleX, scaleY);
    }

    private void CreateStatsBoxBg()
    {
        statsBoxBg = new Texture2D(1, 1);
        statsBoxBg.SetPixel(0, 0, new Color(0f, 0f, 0f, 0.7f));
        statsBoxBg.Apply();
    }

    private void CacheIconTextures()
    {
        foreach (WeaponType type in System.Enum.GetValues(typeof(WeaponType)))
        {
            Texture2D tex = GetPublicIcon(type);
            if (tex == null)
            {
                int size = 72;
                tex = new Texture2D(size, size);
                Color color = GetWeaponColor(type);
                for (int y = 0; y < size; y++)
                    for (int x = 0; x < size; x++)
                        tex.SetPixel(x, y, color);
                tex.Apply();
            }
            iconTextures[type] = tex;
        }
    }

    private void CacheBgTextures()
    {
        foreach (WeaponType type in System.Enum.GetValues(typeof(WeaponType)))
        {
            Texture2D tex = GetPublicBg(type);
            if (tex != null)
                bgTextures[type] = tex;
        }
    }

    private Texture2D GetPublicBg(WeaponType type)
    {
        switch (type)
        {
            case WeaponType.Sword: return swordBg;
            case WeaponType.Spear: return spearBg;
            case WeaponType.Axe: return axeBg;
            case WeaponType.FlyingSword: return flyingSwordBg;
            default: return null;
        }
    }

    private Texture2D GetPublicIcon(WeaponType type)
    {
        switch (type)
        {
            case WeaponType.Sword: return swordIcon;
            case WeaponType.Spear: return spearIcon;
            case WeaponType.Axe: return axeIcon;
            case WeaponType.FlyingSword: return flyingSwordIcon;
            default: return null;
        }
    }

    public void Show()
    {
        isShown = true;
        selectedIndex = -1;
        enabled = true;
        gameObject.SetActive(true);
    }

    private void OnGUI()
    {
        if (!isShown) return;

        if (layer1Texture != null)
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), layer1Texture);

        if (layer2Texture != null)
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), layer2Texture);

        UpdateScaleFactor();

        EnsureStyles();

        float centerX = Screen.width * 0.5f;

        GUI.Label(new Rect(centerX - titleWidth * 0.5f * scaleFactor, titleY * scaleFactor, titleWidth * scaleFactor, titleHeight * scaleFactor), "CHỌN VŨ KHÍ", titleStyle);

        float cw = cardWidth * scaleFactor;
        float ch = cardHeight * scaleFactor;
        float totalWidth = weapons.Count * cw + (weapons.Count - 1) * cardSpacing * scaleFactor;
        float startX = centerX - totalWidth * 0.5f;
        float cy = cardY * scaleFactor;

        for (int i = 0; i < weapons.Count; i++)
        {
            Rect cardRect = new Rect(startX + i * (cw + cardSpacing * scaleFactor), cy, cw, ch);
            DrawWeaponCard(cardRect, weapons[i], i == selectedIndex);

            GUI.Label(new Rect(cardRect.x, cardRect.y + ch + nameLabelYOffset * scaleFactor, cardRect.width, nameLabelHeight * scaleFactor),
                GetWeaponName(weapons[i].WeaponType), weaponNameStyle);
        }

        if (selectedIndex >= 0 && selectedIndex < weapons.Count)
        {
            WeaponData weapon = weapons[selectedIndex];
            float statsY = cy + ch + statsYOffset * scaleFactor;
            float statsX = centerX - statsBoxWidth * 0.5f * scaleFactor;
            float statsWidth = statsBoxWidth * scaleFactor;

            GUI.DrawTexture(new Rect(statsX - statsBoxPadding * scaleFactor, statsY - statsBoxPadding * scaleFactor, statsWidth + statsBoxPadding * 2f * scaleFactor, statsBoxHeight * scaleFactor), statsBoxBg);

            GUI.Label(new Rect(statsX, statsY, statsWidth, statsHeaderHeight * scaleFactor), GetWeaponName(weapon.WeaponType).ToUpper(), statsHeaderStyle);
            statsY += statsHeaderYOffset * scaleFactor;

            DrawStatRow("Sát thương", weapon.NormalDamageMultiplier.ToString("F1"), ref statsY, statsX, statsWidth);
            DrawStatRow("Chí mạng", (weapon.CritChance * 100f).ToString("F0") + "%", ref statsY, statsX, statsWidth);
            DrawStatRow("Tầm đánh", weapon.NormalRange.ToString("F1"), ref statsY, statsX, statsWidth);
            DrawStatRow("Hồi chiêu", weapon.NormalAttackCooldown.ToString("F1") + "s", ref statsY, statsX, statsWidth);
            DrawStatRow("Dạng đánh thường", GetAttackShapeName(weapon.NormalAttackShape), ref statsY, statsX, statsWidth);
            DrawStatRow("Dạng chiêu thức", GetAttackShapeName(weapon.SkillAttackShape), ref statsY, statsX, statsWidth);
        }

        float btnY = Screen.height - buttonYOffset * scaleFactor;
        GUI.enabled = selectedIndex >= 0;
        if (GUI.Button(new Rect(centerX - buttonWidth * 0.5f * scaleFactor, btnY, buttonWidth * scaleFactor, buttonHeight * scaleFactor), "Xác nhận", buttonStyle))
        {
            ConfirmSelection();
        }
        GUI.enabled = true;
    }

    private void DrawStatRow(string label, string value, ref float y, float x, float width)
    {
        GUI.Label(new Rect(x, y, width * statsLabelWidthRatio, statsRowHeight * scaleFactor), label, statsLabelStyle);
        GUI.Label(new Rect(x + width * statsLabelWidthRatio, y, width * (1f - statsLabelWidthRatio), statsRowHeight * scaleFactor), value, statsValueStyle);
        y += statsRowSpacing * scaleFactor;
    }

    private void DrawWeaponCard(Rect rect, WeaponData weapon, bool isSelected)
    {
        Color borderColor = isSelected ? Color.yellow : new Color(0.4f, 0.4f, 0.4f, 0.7f);
        float borderWidth = (isSelected ? 4f : 2f) * scaleFactor;

        GUI.color = borderColor;
        GUI.DrawTexture(new Rect(
            rect.x - borderWidth, rect.y - borderWidth,
            rect.width + borderWidth * 2f, rect.height + borderWidth * 2f), Texture2D.whiteTexture);
        GUI.color = Color.white;

        Texture2D bgTex;
        if (bgTextures.TryGetValue(weapon.WeaponType, out bgTex))
            GUI.DrawTexture(rect, bgTex);

        Texture2D iconTex;
        if (iconTextures.TryGetValue(weapon.WeaponType, out iconTex))
            GUI.DrawTexture(rect, iconTex);

        if (Event.current.type == EventType.MouseDown && rect.Contains(Event.current.mousePosition))
        {
            selectedIndex = weapons.IndexOf(weapon);
        }
    }

    private static string GetWeaponName(WeaponType type)
    {
        switch (type)
        {
            case WeaponType.Sword: return "Kiếm";
            case WeaponType.Spear: return "Thương";
            case WeaponType.Axe: return "Rìu";
            case WeaponType.FlyingSword: return "Phi Kiếm";
            default: return type.ToString();
        }
    }

    private static string GetAttackShapeName(AttackShape shape)
    {
        switch (shape)
        {
            case AttackShape.Cone: return "Hình nón";
            case AttackShape.Rectangle: return "Hình chữ nhật";
            case AttackShape.Circle: return "Hình tròn";
            case AttackShape.Projectile: return "Đạn";
            case AttackShape.ProjectileSpread: return "Đạn lan";
            default: return shape.ToString();
        }
    }

    private static Color GetWeaponColor(WeaponType type)
    {
        switch (type)
        {
            case WeaponType.Sword: return new Color(0.4f, 0.7f, 1f);
            case WeaponType.Spear: return new Color(1f, 0.55f, 0.15f);
            case WeaponType.Axe: return new Color(1f, 0.25f, 0.25f);
            case WeaponType.FlyingSword: return new Color(0.65f, 0.25f, 1f);
            default: return Color.gray;
        }
    }

    private void ConfirmSelection()
    {
        if (selectedIndex >= 0 && selectedIndex < weapons.Count)
        {
            GameManager.Instance.SelectedWeapon = weapons[selectedIndex].WeaponType;
        }
        isShown = false;
        gameObject.SetActive(false);
        mainMenuUI.ShowStageSelect();
    }

    private void EnsureStyles()
    {
        if (titleStyle != null && Mathf.Approximately(lastStyleScaleFactor, scaleFactor))
            return;

        lastStyleScaleFactor = scaleFactor;

        titleStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = Mathf.RoundToInt(40 * scaleFactor),
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.MiddleCenter
        };
        titleStyle.normal.textColor = Color.white;

        weaponNameStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = Mathf.RoundToInt(20 * scaleFactor),
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.MiddleCenter
        };
        weaponNameStyle.normal.textColor = weaponNameColor;

        statsLabelStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = Mathf.RoundToInt(20 * scaleFactor),
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.MiddleLeft
        };
        statsLabelStyle.normal.textColor = new Color(0.8f, 0.8f, 0.8f);

        statsValueStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = Mathf.RoundToInt(20 * scaleFactor),
            alignment = TextAnchor.MiddleLeft
        };
        statsValueStyle.normal.textColor = Color.white;

        statsHeaderStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = Mathf.RoundToInt(24 * scaleFactor),
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.MiddleLeft
        };
        statsHeaderStyle.normal.textColor = Color.yellow;

        buttonStyle = new GUIStyle(GUI.skin.button)
        {
            fontSize = Mathf.RoundToInt(26 * scaleFactor),
            fontStyle = FontStyle.Bold
        };
    }
}
