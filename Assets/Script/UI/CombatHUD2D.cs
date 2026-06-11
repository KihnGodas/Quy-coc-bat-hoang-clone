using UnityEngine;

public sealed class CombatHUD2D : MonoBehaviour
{
    [SerializeField] private PlayerHealth2D playerHealth;
    [SerializeField] private PlayerCombat2D playerCombat;
    [SerializeField] private WeaponController weaponController;
    [SerializeField] private PlayerDash2D playerDash;
    [SerializeField] private PlayerExperience playerExperience;
    [SerializeField] private PlayerUltimateController playerUltimate;
    [SerializeField] private CombatManager combatManager;
    [SerializeField] private Vector2 screenOffset = new Vector2(16f, 16f);
    [SerializeField, Min(16f)] private float labelWidth = 220f;
    [SerializeField, Min(18f)] private float lineHeight = 24f;
    [SerializeField, Min(8)] private int fontSize = 18;
    [SerializeField] private Color normalTextColor = Color.white;
    [SerializeField] private Color readyTextColor = new Color(0.45f, 1f, 0.55f, 1f);
    [SerializeField] private Color cooldownTextColor = new Color(1f, 0.82f, 0.35f, 1f);

    [Header("Health Bar")]
    [SerializeField] private float healthBarWidth = 280f;
    [SerializeField] private float healthBarHeight = 22f;
    [SerializeField] private float healthBarBottomOffset = 85f;
    [SerializeField] private Color healthBarBackgroundColor = new Color(0.12f, 0.12f, 0.12f, 0.85f);
    [SerializeField] private Color healthBarBorderColor = new Color(0.4f, 0.4f, 0.4f, 1f);

    [Header("Experience Bar")]
    [SerializeField] private float expBarWidth = 200f;
    [SerializeField] private float expBarHeight = 22f;
    [SerializeField] private float expBarGap = 10f;
    [SerializeField] private Color expBarBackgroundColor = new Color(0.12f, 0.12f, 0.12f, 0.85f);
    [SerializeField] private Color expBarBorderColor = new Color(0.4f, 0.4f, 0.4f, 1f);
    [SerializeField] private Color expBarColor = new Color(0.35f, 0.55f, 1f);

    private GUIStyle labelStyle;
    private GUIStyle centerStyle;
    private Texture2D whiteTexture;

    private void Awake()
    {
        ResolveMissingReferences();
        InitializeWhiteTexture();
    }

    private void Update()
    {
        ResolveMissingReferences();
    }

    private void OnGUI()
    {
        if (DialogueManager.Instance != null && DialogueManager.Instance.IsPlaying)
            return;

        EnsureStyle();
        EnsureCenterStyle();

        DrawCenterTopInfo();

        DrawBottomBars();
    }

    private void ResolveMissingReferences()
    {
        if (playerHealth == null)
        {
            playerHealth = FindComponentInScene<PlayerHealth2D>();
        }

        if (playerCombat == null)
        {
            playerCombat = FindComponentInScene<PlayerCombat2D>();
        }

        if (weaponController == null)
        {
            weaponController = FindComponentInScene<WeaponController>();
        }

        if (playerDash == null)
        {
            playerDash = FindComponentInScene<PlayerDash2D>();
        }

        if (playerExperience == null)
        {
            playerExperience = FindComponentInScene<PlayerExperience>();
        }

        if (playerUltimate == null)
        {
            playerUltimate = FindComponentInScene<PlayerUltimateController>();
        }

        if (combatManager == null)
        {
            combatManager = FindComponentInScene<CombatManager>();
        }
    }

    private static T FindComponentInScene<T>() where T : Object
    {
#if UNITY_2023_1_OR_NEWER
        return FindFirstObjectByType<T>();
#else
        return FindObjectOfType<T>();
#endif
    }

    private void EnsureStyle()
    {
        if (labelStyle != null && labelStyle.fontSize == fontSize)
        {
            return;
        }

        labelStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = fontSize,
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.UpperLeft
        };
    }

    private void EnsureCenterStyle()
    {
        if (centerStyle != null && centerStyle.fontSize == fontSize)
        {
            return;
        }

        centerStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = fontSize,
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.UpperCenter
        };
    }

    private void DrawCenterTopInfo()
    {
        if (combatManager == null)
        {
            centerStyle.normal.textColor = normalTextColor;
            DrawCenteredLine("Missing CombatManager");
            return;
        }

        centerStyle.normal.textColor = GetCombatTimeTextColor();

        if (combatManager.Mode == CombatManager.CombatMode.BossCombat)
        {
            DrawCenteredLine($"BOSS  |  Kills: {combatManager.TotalKills}");
            return;
        }

        switch (combatManager.State)
        {
            case CombatManager.CombatState.NotStarted:
                DrawCenteredLine($"Starting: {combatManager.Duration:0.0}s");
                break;

            case CombatManager.CombatState.Running:
                DrawCenteredLine($"{combatManager.RemainingTime:0.0}s  |  Kills: {combatManager.TotalKills}");
                break;

            case CombatManager.CombatState.ClearingEnemies:
                DrawCenteredLine($"Clear: {combatManager.RemainingEnemyCount} remaining");
                break;

            default:
                DrawCenteredLine($"Combat: {combatManager.State}");
                break;
        }
    }

    private void DrawCenteredLine(string text)
    {
        GUI.Label(new Rect(0f, screenOffset.y, Screen.width, lineHeight), text, centerStyle);
    }

    private void DrawLine(string text, Color color, float y)
    {
        labelStyle.normal.textColor = color;
        GUI.Label(new Rect(screenOffset.x, y, labelWidth, lineHeight), text, labelStyle);
    }

    private void DrawBottomBars()
    {
        float labelHeight = 18f;
        float spacing = 4f;

        float totalWidth = healthBarWidth + expBarGap + expBarWidth;
        float startX = (Screen.width - totalWidth) * 0.5f;
        float barY = Screen.height - healthBarHeight - healthBarBottomOffset;
        float labelY = barY - spacing - labelHeight;

        // Đã thêm (float) vào trước playerHealth.CurrentHealth
        DrawSingleBar(
            startX, barY, labelY, healthBarWidth, healthBarHeight, labelHeight,
            playerHealth != null ? Mathf.Clamp01((float)playerHealth.CurrentHealth / playerHealth.MaxHealth) : 0f,
            Color.red, healthBarBackgroundColor, healthBarBorderColor,
            "Health",
            playerHealth != null
                ? $"{Mathf.CeilToInt(playerHealth.CurrentHealth)} / {Mathf.CeilToInt(playerHealth.MaxHealth)}"
                : ""
        );

        float expX = startX + healthBarWidth + expBarGap;
        
        // Đã thêm (float) vào trước playerExperience.CurrentExperience
        DrawSingleBar(
            expX, barY, labelY, expBarWidth, expBarHeight, labelHeight,
            playerExperience != null ? Mathf.Clamp01((float)playerExperience.CurrentExperience / playerExperience.ExperienceToNextLevel) : 0f,
            expBarColor, expBarBackgroundColor, expBarBorderColor,
            "EXP",
            playerExperience != null
                ? $"{Mathf.CeilToInt(playerExperience.CurrentExperience)} / {Mathf.CeilToInt(playerExperience.ExperienceToNextLevel)}"
                : ""
        );
    }
    private void DrawSingleBar(float x, float barY, float labelY, float barWidth, float barHeight, float labelHeight,
        float fillPercent, Color fillColor, Color bgColor, Color borderColor, string label, string numberText)
    {
        Rect barRect = new Rect(x, barY, barWidth, barHeight);

        GUI.color = bgColor;
        GUI.DrawTexture(barRect, whiteTexture);

        GUI.color = fillColor;
        float fillWidth = barWidth * fillPercent;
        if (fillWidth > 0f)
        {
            GUI.DrawTexture(new Rect(x, barY, fillWidth, barHeight), whiteTexture);
        }

        GUI.color = borderColor;
        float t = 1f;
        GUI.DrawTexture(new Rect(x, barY, barWidth, t), whiteTexture);
        GUI.DrawTexture(new Rect(x, barY + barHeight - t, barWidth, t), whiteTexture);
        GUI.DrawTexture(new Rect(x, barY, t, barHeight), whiteTexture);
        GUI.DrawTexture(new Rect(x + barWidth - t, barY, t, barHeight), whiteTexture);

        GUI.color = Color.white;
        GUIStyle lblStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = Mathf.RoundToInt(14f),
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.LowerCenter,
            normal = { textColor = Color.white }
        };
        GUI.Label(new Rect(x, labelY, barWidth, labelHeight), label, lblStyle);

        GUIStyle numStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = Mathf.RoundToInt(barHeight * 0.5f),
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.MiddleCenter,
            normal = { textColor = Color.white }
        };
        GUI.Label(barRect, numberText, numStyle);

        GUI.color = Color.white;
    }

    private void InitializeWhiteTexture()
    {
        whiteTexture = new Texture2D(1, 1, TextureFormat.RGBA32, false);
        whiteTexture.SetPixel(0, 0, Color.white);
        whiteTexture.Apply();
    }

    private string FormatSkillText()
    {
        if (weaponController != null)
        {
            return weaponController.IsSkillReady
                ? $"{weaponController.CurrentWeapon} Skill Ready"
                : $"{weaponController.CurrentWeapon} Skill CD: {weaponController.SkillCooldownRemaining:0.0}s";
        }

        if (playerCombat == null)
        {
            return "Skill: Missing PlayerCombat2D";
        }

        return playerCombat.IsSkillReady
            ? "Skill Ready"
            : $"Skill CD: {playerCombat.SkillCooldownRemaining:0.0}s";
    }

    private string FormatDashText()
    {
        if (playerDash == null)
        {
            return "Dash: Missing PlayerDash2D";
        }

        return playerDash.IsDashReady
            ? "Dash Ready"
            : $"Dash CD: {playerDash.DashCooldownRemaining:0.0}s";
    }

    private string FormatActionText()
    {
        return $"{FormatSkillText()} | {FormatDashText()}";
    }

    private string FormatProgressionText()
    {
        if (playerExperience == null)
        {
            return "Lv: Missing";
        }

        CultivationRealm realm = CultivationRealm.LuyenKhi;
        if (playerExperience != null)
        {
            PlayerCultivationState cultivationState = playerExperience.GetComponent<PlayerCultivationState>();
            if (cultivationState != null)
            {
                realm = cultivationState.PlayerRealm;
            }
        }

        return $"Lv {playerExperience.Level} | {realm} | EXP {playerExperience.CurrentExperience:0}/{playerExperience.ExperienceToNextLevel:0}";
    }

    private string FormatUltimateText()
    {
        if (playerUltimate == null)
        {
            return "Ult: Missing";
        }

        if (!playerUltimate.IsUltimateUnlocked)
        {
            return "Ult: Locked";
        }

        return playerUltimate.IsUltimateReady
            ? $"Ult: {playerUltimate.CurrentUltimateType} Ready"
            : $"Ult: {playerUltimate.UltimateCooldownRemaining:0.0}s";
    }

    private Color GetSkillTextColor()
    {
        if (weaponController != null)
        {
            return weaponController.IsSkillReady ? readyTextColor : cooldownTextColor;
        }

        return playerCombat != null && playerCombat.IsSkillReady ? readyTextColor : cooldownTextColor;
    }

    private Color GetDashTextColor()
    {
        return playerDash != null && playerDash.IsDashReady ? readyTextColor : cooldownTextColor;
    }

    private Color GetActionTextColor()
    {
        bool skillReady = weaponController != null
            ? weaponController.IsSkillReady
            : playerCombat != null && playerCombat.IsSkillReady;

        bool dashReady = playerDash != null && playerDash.IsDashReady;
        return skillReady && dashReady ? readyTextColor : cooldownTextColor;
    }

    private Color GetCombatTimeTextColor()
    {
        return combatManager != null
            && (combatManager.State == CombatManager.CombatState.Running
                || combatManager.State == CombatManager.CombatState.ClearingEnemies)
            ? normalTextColor
            : cooldownTextColor;
    }

    private Color GetUltimateTextColor()
    {
        return playerUltimate != null && playerUltimate.IsUltimateReady ? readyTextColor : cooldownTextColor;
    }

    private Color GetProgressionTextColor()
    {
        return playerExperience != null && playerExperience.CanUseUltimate ? readyTextColor : normalTextColor;
    }
}
