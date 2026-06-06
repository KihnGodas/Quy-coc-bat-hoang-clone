using UnityEngine;

public sealed class BossHealthUI : MonoBehaviour
{
    [SerializeField] private BossController bossController;
    [SerializeField] private CombatManager combatManager;
    [SerializeField] private Vector2 screenOffset = new Vector2(16f, 124f);
    [SerializeField, Min(120f)] private float width = 420f;
    [SerializeField, Min(14f)] private float height = 22f;
    [SerializeField, Min(8)] private int fontSize = 15;
    [SerializeField] private Color frameColor = new Color(0f, 0f, 0f, 0.75f);
    [SerializeField] private Color fillColor = new Color(0.2f, 0.9f, 0.35f, 0.95f);
    [SerializeField] private Color lowFillColor = new Color(1f, 0.25f, 0.15f, 0.95f);
    [SerializeField] private Color textColor = Color.white;

    private GUIStyle labelStyle;
    private Texture2D whiteTexture;

    private void Update()
    {
        ResolveReferences();
    }

    private void OnGUI()
    {
        ResolveReferences();
        if (combatManager == null || combatManager.Mode != CombatManager.CombatMode.BossCombat)
        {
            return;
        }

        BossBase boss = bossController != null ? bossController.ActiveBoss : null;
        if (boss == null || boss.Health == null || boss.Health.IsDead)
        {
            return;
        }

        EnsureResources();
        float hp01 = boss.MaxHP > 0f ? Mathf.Clamp01(boss.CurrentHP / boss.MaxHP) : 0f;
        Rect frameRect = new Rect(screenOffset.x, screenOffset.y, width, height);
        Rect fillRect = new Rect(screenOffset.x + 2f, screenOffset.y + 2f, (width - 4f) * hp01, height - 4f);

        DrawRect(frameRect, frameColor);
        DrawRect(fillRect, Color.Lerp(lowFillColor, fillColor, hp01));

        labelStyle.normal.textColor = textColor;
        GUI.Label(frameRect, $"{boss.BossName}: {Mathf.CeilToInt(boss.CurrentHP)} / {Mathf.CeilToInt(boss.MaxHP)}", labelStyle);

        Act1WoodBossController woodBoss = boss.GetComponent<Act1WoodBossController>();
        if (woodBoss != null)
        {
            Rect stateRect = new Rect(screenOffset.x, screenOffset.y + height + 4f, width, height);
            labelStyle.normal.textColor = woodBoss.IsShieldActive
                ? new Color(0.55f, 1f, 0.65f, 1f)
                : new Color(1f, 0.85f, 0.25f, 1f);
            string shieldText = woodBoss.IsShieldActive ? "Shield: ACTIVE" : "Shield: OPEN";
            GUI.Label(stateRect, $"{shieldText} | Roots Left: {woodBoss.RemainingRootCount}", labelStyle);
            return;
        }

        Act2FireBossController fireBoss = boss.GetComponent<Act2FireBossController>();
        if (fireBoss != null)
        {
            Rect stateRect = new Rect(screenOffset.x, screenOffset.y + height + 4f, width, height);
            labelStyle.normal.textColor = fireBoss.IsEnraged
                ? new Color(1f, 0.25f, 0.08f, 1f)
                : new Color(1f, 0.72f, 0.22f, 1f);
            string stateText = fireBoss.IsEnraged ? "State: ENRAGED" : "State: NORMAL";
            GUI.Label(stateRect, $"{stateText} | Skill: {fireBoss.CurrentSkillName}", labelStyle);
        }
    }

    private void EnsureResources()
    {
        if (whiteTexture == null)
        {
            whiteTexture = Texture2D.whiteTexture;
        }

        if (labelStyle == null || labelStyle.fontSize != fontSize)
        {
            labelStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = fontSize,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter
            };
        }
    }

    private void DrawRect(Rect rect, Color color)
    {
        Color previous = GUI.color;
        GUI.color = color;
        GUI.DrawTexture(rect, whiteTexture);
        GUI.color = previous;
    }

    private void ResolveReferences()
    {
        if (bossController == null)
        {
            bossController = FindComponentInScene<BossController>();
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
}
