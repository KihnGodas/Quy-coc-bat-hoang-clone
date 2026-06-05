using UnityEngine;

public sealed class MainMenuUI : MonoBehaviour
{
    [SerializeField] private Vector2 windowSize = new Vector2(360f, 480f);
    [SerializeField, Min(10)] private int titleFontSize = 36;
    [SerializeField, Min(8)] private int stageFontSize = 18;
    [SerializeField, Min(8)] private int lockFontSize = 14;
    [SerializeField, Min(20f)] private float buttonHeight = 44f;
    [SerializeField] private Color lockedColor = new Color(0.5f, 0.5f, 0.5f, 1f);
    [SerializeField] private Color unlockedColor = Color.white;
    [SerializeField] private Color bossStageColor = new Color(1f, 0.7f, 0.2f, 1f);

    private GUIStyle titleStyle;
    private GUIStyle stageStyle;
    private GUIStyle lockStyle;
    private GUIStyle buttonStyle;

    private void OnGUI()
    {
        if (GameManager.Instance == null)
        {
            return;
        }

        if (GameManager.Instance.StageProgression == null)
        {
            GUI.Box(new Rect(0, 0, Screen.width, Screen.height), GUIContent.none);
            GUILayout.BeginArea(new Rect(0, 0, Screen.width, Screen.height));
            GUILayout.FlexibleSpace();
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("StageProgression not found!\nRun Tools > Create All Stage Assets in Unity Editor.", 
                new GUIStyle(GUI.skin.label) { fontSize = 20, fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleCenter });
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.EndArea();
            return;
        }

        EnsureStyles();

        StageProgression progression = GameManager.Instance.StageProgression;

        Rect windowRect = new Rect(
            (Screen.width - windowSize.x) * 0.5f,
            (Screen.height - windowSize.y) * 0.5f,
            windowSize.x,
            windowSize.y);

        GUI.Box(windowRect, GUIContent.none);

        GUILayout.BeginArea(new Rect(windowRect.x + 20f, windowRect.y + 16f, windowRect.width - 40f, windowRect.height - 32f));

        titleStyle.normal.textColor = bossStageColor;
        GUILayout.Label("Quỷ Cốc Bát Hoang", titleStyle);
        GUILayout.Space(20f);

        for (int i = 0; i < progression.TotalStages; i++)
        {
            StageData stage = progression.GetStage(i);
            if (stage == null)
            {
                continue;
            }

            bool unlocked = GameManager.Instance.IsStageUnlocked(i);
            bool isBoss = stage.CombatMode == CombatManager.CombatMode.BossCombat;

            Color originalColor = GUI.color;
            string label = unlocked
                ? $"{(isBoss ? "👑 " : "")}Stage {stage.StageNumber}: {stage.StageName}"
                : $"Stage {stage.StageNumber}: ???";

            if (unlocked)
            {
                GUI.color = isBoss ? bossStageColor : unlockedColor;
            }
            else
            {
                GUI.color = lockedColor;
            }

            if (unlocked)
            {
                if (GUILayout.Button(label, buttonStyle, GUILayout.Height(buttonHeight)))
                {
                    GameManager.Instance.TryLoadStage(i);
                }
            }
            else
            {
                GUILayout.Label(label, lockStyle, GUILayout.Height(buttonHeight));
            }

            GUI.color = originalColor;
        }

        GUILayout.FlexibleSpace();
        GUILayout.EndArea();
    }

    private void EnsureStyles()
    {
        if (titleStyle != null && titleStyle.fontSize == titleFontSize)
        {
            return;
        }

        titleStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = titleFontSize,
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.MiddleCenter
        };

        stageStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = stageFontSize,
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.MiddleLeft
        };

        lockStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = lockFontSize,
            fontStyle = FontStyle.Italic,
            alignment = TextAnchor.MiddleCenter
        };

        buttonStyle = new GUIStyle(GUI.skin.button)
        {
            fontSize = stageFontSize,
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.MiddleLeft
        };
    }
}
