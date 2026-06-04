using UnityEngine;
using UnityEngine.SceneManagement;

public sealed class CombatResultUI : MonoBehaviour
{
    [SerializeField] private CombatManager combatManager;
    [SerializeField] private EnemySpawner enemySpawner;
    [SerializeField] private PlayerExperience playerExperience;
    [SerializeField] private Vector2 windowSize = new Vector2(420f, 300f);
    [SerializeField, Min(10)] private int titleFontSize = 30;
    [SerializeField, Min(8)] private int bodyFontSize = 16;
    [SerializeField, Min(20f)] private float buttonHeight = 42f;
    [SerializeField] private string restartButtonText = "Restart";
    [SerializeField] private Color victoryColor = new Color(0.45f, 1f, 0.55f, 1f);
    [SerializeField] private Color defeatColor = new Color(1f, 0.35f, 0.3f, 1f);

    private GUIStyle titleStyle;
    private GUIStyle bodyStyle;
    private GUIStyle buttonStyle;

    private void Awake()
    {
        ResolveReferences();
    }

    private void Update()
    {
        ResolveReferences();
    }

    private void OnGUI()
    {
        if (combatManager == null || !IsCombatFinished())
        {
            return;
        }

        EnsureStyles();

        Rect windowRect = new Rect(
            (Screen.width - windowSize.x) * 0.5f,
            (Screen.height - windowSize.y) * 0.5f,
            windowSize.x,
            windowSize.y);

        GUI.Box(windowRect, GUIContent.none);

        GUILayout.BeginArea(new Rect(windowRect.x + 24f, windowRect.y + 22f, windowRect.width - 48f, windowRect.height - 44f));
        DrawTitle();
        GUILayout.Space(14f);
        DrawStats();
        GUILayout.FlexibleSpace();

        if (GUILayout.Button(restartButtonText, buttonStyle, GUILayout.Height(buttonHeight)))
        {
            RestartCurrentScene();
        }

        GUILayout.EndArea();
    }

    private bool IsCombatFinished()
    {
        return combatManager.State == CombatManager.CombatState.Victory
            || combatManager.State == CombatManager.CombatState.Defeat;
    }

    private void DrawTitle()
    {
        bool victory = combatManager.Result == CombatResult.Victory;
        titleStyle.normal.textColor = victory ? victoryColor : defeatColor;
        GUILayout.Label(victory ? "VICTORY" : "DEFEAT", titleStyle);
    }

    private void DrawStats()
    {
        GUILayout.Label($"Time: {combatManager.ElapsedTime:0.0}s", bodyStyle);

        if (enemySpawner != null)
        {
            GUILayout.Label($"Enemy defeated: {enemySpawner.DefeatedEnemyCount}", bodyStyle);
            GUILayout.Label($"Enemy remaining: {enemySpawner.AliveCount}", bodyStyle);
        }

        if (playerExperience != null)
        {
            GUILayout.Label($"Level: {playerExperience.Level}", bodyStyle);
            GUILayout.Label($"EXP: {playerExperience.CurrentExperience:0}/{playerExperience.ExperienceToNextLevel:0}", bodyStyle);
        }
    }

    private void RestartCurrentScene()
    {
        Scene activeScene = SceneManager.GetActiveScene();
        if (activeScene.buildIndex >= 0)
        {
            SceneManager.LoadScene(activeScene.buildIndex);
            return;
        }

        SceneManager.LoadScene(activeScene.name);
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

        bodyStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = bodyFontSize,
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.MiddleLeft
        };

        buttonStyle = new GUIStyle(GUI.skin.button)
        {
            fontSize = bodyFontSize,
            fontStyle = FontStyle.Bold
        };
    }

    private void ResolveReferences()
    {
        if (combatManager == null)
        {
            combatManager = FindComponentInScene<CombatManager>();
        }

        if (enemySpawner == null)
        {
            enemySpawner = FindComponentInScene<EnemySpawner>();
        }

        if (playerExperience == null)
        {
            playerExperience = FindComponentInScene<PlayerExperience>();
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
