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

    private GUIStyle labelStyle;

    private void Awake()
    {
        ResolveMissingReferences();
    }

    private void Update()
    {
        ResolveMissingReferences();
    }

    private void OnGUI()
    {
        EnsureStyle();

        float y = screenOffset.y;
        DrawLine(FormatHealthText(), normalTextColor, y);
        y += lineHeight;

        DrawLine(FormatCombatTimeText(), GetCombatTimeTextColor(), y);
        y += lineHeight;

        DrawLine(FormatProgressionText(), GetProgressionTextColor(), y);
        y += lineHeight;

        DrawLine(FormatActionText(), GetActionTextColor(), y);
        y += lineHeight;

        DrawLine(FormatUltimateText(), GetUltimateTextColor(), y);
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

    private void DrawLine(string text, Color color, float y)
    {
        labelStyle.normal.textColor = color;
        GUI.Label(new Rect(screenOffset.x, y, labelWidth, lineHeight), text, labelStyle);
    }

    private string FormatHealthText()
    {
        if (playerHealth == null)
        {
            return "HP: Missing PlayerHealth2D";
        }

        int currentHealth = Mathf.CeilToInt(playerHealth.CurrentHealth);
        int maxHealth = Mathf.CeilToInt(playerHealth.MaxHealth);
        return $"HP: {currentHealth} / {maxHealth}";
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

    private string FormatCombatTimeText()
    {
        if (combatManager == null)
        {
            return "Time Left: Missing CombatManager";
        }

        if (combatManager.Mode != CombatManager.CombatMode.NormalCombat)
        {
            return $"Boss Stage: {combatManager.State}";
        }

        if (combatManager.State == CombatManager.CombatState.ClearingEnemies)
        {
            return $"Clear Enemies: {combatManager.RemainingEnemyCount}";
        }

        if (combatManager.State == CombatManager.CombatState.Running)
        {
            return $"Time Left: {combatManager.RemainingTime:0.0}s";
        }

        if (combatManager.State == CombatManager.CombatState.NotStarted)
        {
            return $"Time Left: {combatManager.Duration:0.0}s";
        }

        return $"Combat: {combatManager.State}";
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
