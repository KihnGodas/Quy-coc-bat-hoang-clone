using UnityEngine;

public sealed class CombatHUD2D : MonoBehaviour
{
    [SerializeField] private PlayerHealth2D playerHealth;
    [SerializeField] private PlayerCombat2D playerCombat;
    [SerializeField] private PlayerDash2D playerDash;
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

        DrawLine(FormatSkillText(), GetSkillTextColor(), y);
        y += lineHeight;

        DrawLine(FormatDashText(), GetDashTextColor(), y);
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

        if (playerDash == null)
        {
            playerDash = FindComponentInScene<PlayerDash2D>();
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

    private Color GetSkillTextColor()
    {
        return playerCombat != null && playerCombat.IsSkillReady ? readyTextColor : cooldownTextColor;
    }

    private Color GetDashTextColor()
    {
        return playerDash != null && playerDash.IsDashReady ? readyTextColor : cooldownTextColor;
    }
}
