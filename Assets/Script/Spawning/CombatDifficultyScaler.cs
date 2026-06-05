using UnityEngine;

public sealed class CombatDifficultyScaler : MonoBehaviour
{
    [SerializeField] private EnemySpawner enemySpawner;
    [SerializeField] private CombatManager combatManager;
    [SerializeField] private bool scalingEnabled = true;
    [SerializeField, Min(0.1f)] private float earlySpawnInterval = 2.2f;
    [SerializeField, Min(0.1f)] private float lateSpawnInterval = 0.85f;
    [SerializeField, Min(1)] private int earlyMaxAliveEnemies = 7;
    [SerializeField, Min(1)] private int lateMaxAliveEnemies = 14;
    [SerializeField] private float currentDifficulty01;
    [SerializeField] private float currentSpawnInterval;
    [SerializeField] private int currentMaxAliveEnemies;

    public bool ScalingEnabled
    {
        get => scalingEnabled;
        set => scalingEnabled = value;
    }

    public void ConfigureFromStage(StageData stageData)
    {
        if (stageData == null)
        {
            return;
        }

        scalingEnabled = stageData.EnableDifficultyScaling;
        earlySpawnInterval = stageData.EarlySpawnInterval;
        lateSpawnInterval = stageData.LateSpawnInterval;
        earlyMaxAliveEnemies = stageData.EarlyMaxAlive;
        lateMaxAliveEnemies = stageData.LateMaxAlive;
    }

    private void Start()
    {
        ResolveReferences();
        ApplyScaling();
    }

    private void Update()
    {
        ApplyScaling();
    }

    private void ApplyScaling()
    {
        ResolveReferences();

        if (enemySpawner == null)
        {
            return;
        }

        if (!scalingEnabled || combatManager == null || combatManager.Duration <= 0f)
        {
            currentDifficulty01 = 0f;
            currentSpawnInterval = 0f;
            currentMaxAliveEnemies = 0;
            enemySpawner.ClearDifficultySpawnSettings();
            return;
        }

        currentDifficulty01 = Mathf.Clamp01(combatManager.ElapsedTime / combatManager.Duration);
        currentSpawnInterval = Mathf.Lerp(earlySpawnInterval, lateSpawnInterval, currentDifficulty01);
        currentMaxAliveEnemies = Mathf.RoundToInt(Mathf.Lerp(earlyMaxAliveEnemies, lateMaxAliveEnemies, currentDifficulty01));

        enemySpawner.SetDifficultySpawnSettings(currentSpawnInterval, currentMaxAliveEnemies);
    }

    private void ResolveReferences()
    {
        if (enemySpawner == null)
        {
            enemySpawner = FindComponentInScene<EnemySpawner>();
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
