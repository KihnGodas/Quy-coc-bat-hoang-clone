using UnityEngine;

[CreateAssetMenu(fileName = "StageData", menuName = "Combat/Stage Data")]
public sealed class StageData : ScriptableObject
{
    [SerializeField] private string stageName = "Stage";
    [SerializeField] private int stageNumber = 1;
    [SerializeField, TextArea] private string description = "";
    [SerializeField] private Sprite stageIcon;

    [Header("Combat Type")]
    [SerializeField] private CombatManager.CombatMode combatMode = CombatManager.CombatMode.NormalCombat;

    [Header("Normal Mode")]
    [SerializeField] private float duration = 30f;
    [SerializeField] private EnemySpawnEntry[] spawnTable;
    [SerializeField, Min(0.1f)] private float spawnInterval = 2f;
    [SerializeField, Min(1)] private int maxAliveEnemies = 10;
    [SerializeField] private bool enableDifficultyScaling = true;
    [SerializeField] private float earlySpawnInterval = 2.2f;
    [SerializeField] private float lateSpawnInterval = 0.85f;
    [SerializeField] private int earlyMaxAlive = 7;
    [SerializeField] private int lateMaxAlive = 14;

    [Header("Boss Mode")]
    [SerializeField] private BossBase bossPrefab;
    [SerializeField] private string bossName = "Boss";
    [SerializeField, Min(1f)] private float bossHP = 4500f;
    [SerializeField, Min(0f)] private float bossDamage = 60f;
    [SerializeField] private Vector2 bossSpawnPosition = Vector2.zero;
    [SerializeField] private Vector2 bossScale = Vector2.one;

    [Header("Arena")]
    [SerializeField] private Vector2 arenaSize = new Vector2(20f, 14f);

    [Header("Unlock & Rewards")]
    [SerializeField] private int requiredStageNumber;
    [SerializeField] private int requiredPlayerLevel;
    [SerializeField] private float experienceReward;

    public string StageName => stageName;
    public int StageNumber => stageNumber;
    public string Description => description;
    public Sprite StageIcon => stageIcon;
    public CombatManager.CombatMode CombatMode => combatMode;
    public float Duration => duration;
    public EnemySpawnEntry[] SpawnTable => spawnTable;
    public float SpawnInterval => spawnInterval;
    public int MaxAliveEnemies => maxAliveEnemies;
    public bool EnableDifficultyScaling => enableDifficultyScaling;
    public float EarlySpawnInterval => earlySpawnInterval;
    public float LateSpawnInterval => lateSpawnInterval;
    public int EarlyMaxAlive => earlyMaxAlive;
    public int LateMaxAlive => lateMaxAlive;
    public BossBase BossPrefab => bossPrefab;
    public string BossName => bossName;
    public float BossHP => bossHP;
    public float BossDamage => bossDamage;
    public Vector2 BossSpawnPosition => bossSpawnPosition;
    public Vector2 BossScale => bossScale;
    public Vector2 ArenaSize => arenaSize;
    public int RequiredStageNumber => requiredStageNumber;
    public int RequiredPlayerLevel => requiredPlayerLevel;
    public float ExperienceReward => experienceReward;
}
