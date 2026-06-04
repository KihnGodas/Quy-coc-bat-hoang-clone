using UnityEngine;

public sealed class DebugCombatUI : MonoBehaviour
{
    [SerializeField] private PlayerHealth2D playerHealth2D;
    [SerializeField] private Health playerHealth;
    [SerializeField] private Health bossHealth;
    [SerializeField] private EnemySpawner enemySpawner;
    [SerializeField] private CombatManager combatManager;
    [SerializeField] private bool showDebugUI;
    [SerializeField] private string enemyTag = "Enemy";
    [SerializeField] private Vector2 screenOffset = new Vector2(16f, 108f);
    [SerializeField, Min(16f)] private float labelWidth = 260f;
    [SerializeField, Min(18f)] private float lineHeight = 22f;
    [SerializeField, Min(8)] private int fontSize = 16;
    [SerializeField] private Color textColor = Color.white;

    private GUIStyle labelStyle;
    private float startTime;

    private void Awake()
    {
        startTime = Time.time;
        ResolveReferences();
    }

    private void Update()
    {
        ResolveReferences();
    }

    private void OnGUI()
    {
        if (!showDebugUI)
        {
            return;
        }

        EnsureStyle();

        float y = screenOffset.y;
        DrawLine(GetPlayerHealthText(), y);
        y += lineHeight;

        DrawLine($"Timer: {Time.time - startTime:0.0}s", y);
        y += lineHeight;

        DrawLine(GetCombatStateText(), y);
        y += lineHeight;

        DrawLine($"Enemy Count: {GetEnemyCount()}", y);
        y += lineHeight;

        if (bossHealth != null)
        {
            DrawLine($"Boss HP: {Mathf.CeilToInt(bossHealth.CurrentHP)} / {Mathf.CeilToInt(bossHealth.MaxHP)}", y);
        }
        else
        {
            DrawLine("Boss HP: None", y);
        }
    }

    private void ResolveReferences()
    {
        if (playerHealth2D == null)
        {
            playerHealth2D = FindComponentInScene<PlayerHealth2D>();
        }

        if (playerHealth == null)
        {
            playerHealth = FindComponentInScene<Health>();
        }

        if (enemySpawner == null)
        {
            enemySpawner = FindComponentInScene<EnemySpawner>();
        }

        if (combatManager == null)
        {
            combatManager = FindComponentInScene<CombatManager>();
        }
    }

    private string GetPlayerHealthText()
    {
        if (playerHealth2D != null)
        {
            return $"Player HP: {Mathf.CeilToInt(playerHealth2D.CurrentHealth)} / {Mathf.CeilToInt(playerHealth2D.MaxHealth)}";
        }

        if (playerHealth != null)
        {
            return $"Player HP: {Mathf.CeilToInt(playerHealth.CurrentHP)} / {Mathf.CeilToInt(playerHealth.MaxHP)}";
        }

        return "Player HP: Missing";
    }

    private string GetCombatStateText()
    {
        if (combatManager == null)
        {
            return "Combat: Missing";
        }

        if (combatManager.Mode == CombatManager.CombatMode.NormalCombat)
        {
            if (combatManager.State == CombatManager.CombatState.ClearingEnemies)
            {
                return $"Combat: {combatManager.State} | Enemies Left: {combatManager.RemainingEnemyCount}";
            }

            return $"Combat: {combatManager.State} | Time Left: {combatManager.RemainingTime:0.0}s";
        }

        return $"Combat: {combatManager.State} | Mode: Boss";
    }

    private int GetEnemyCount()
    {
        if (enemySpawner != null)
        {
            return enemySpawner.AliveCount;
        }

        try
        {
            return GameObject.FindGameObjectsWithTag(enemyTag).Length;
        }
        catch (UnityException)
        {
            return 0;
        }
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

    private void DrawLine(string text, float y)
    {
        labelStyle.normal.textColor = textColor;
        GUI.Label(new Rect(screenOffset.x, y, labelWidth, lineHeight), text, labelStyle);
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
