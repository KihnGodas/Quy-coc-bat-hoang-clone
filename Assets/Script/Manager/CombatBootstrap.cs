using UnityEngine;

public sealed class CombatBootstrap : MonoBehaviour
{
    [SerializeField] private ArenaBounds arenaBounds;
    [SerializeField] private Transform player;
    [SerializeField] private EnemySpawner enemySpawner;
    [SerializeField] private CombatDifficultyScaler combatDifficultyScaler;
    [SerializeField] private BossController bossController;
    [SerializeField] private CombatManager combatManager;
    [SerializeField] private CombatHUD2D combatHud;
    [SerializeField] private BossHealthUI bossHealthUi;
    [SerializeField] private CombatResultUI combatResultUi;
    [SerializeField] private DebugCombatUI debugCombatUI;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private bool ensureBossController = true;
    [SerializeField] private bool ensureBossHealthUi = true;
    [SerializeField] private bool ensureCombatDifficultyScaler = true;
    [SerializeField] private bool ensureCombatResultUi = true;
    [SerializeField] private bool ensurePlayerExperience = true;
    [SerializeField] private bool ensurePlayerCultivationState = true;
    [SerializeField] private bool ensurePlayerSpellController = true;
    [SerializeField] private bool ensurePlayerUltimateController = true;
    [SerializeField] private bool logBootstrap = true;

    public ArenaBounds ArenaBounds => arenaBounds;
    public Transform Player => player;
    public EnemySpawner EnemySpawner => enemySpawner;
    public CombatManager CombatManager => combatManager;

    private void Awake()
    {
        ResolveReferences();
    }

    private void Start()
    {
        ResolveReferences();
        EnsureRuntimePlayerComponents();
        EnsureRuntimeCombatComponents();

        if (logBootstrap)
        {
            Debug.Log($"Combat bootstrap ready. Arena: {arenaBounds != null}, Player: {player != null}, Spawner: {enemySpawner != null}");
        }
    }

    public void ResolveReferences()
    {
        if (arenaBounds == null)
        {
            arenaBounds = FindComponentInScene<ArenaBounds>();
        }

        if (player == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null)
            {
                player = playerObject.transform;
            }
        }

        if (enemySpawner == null)
        {
            enemySpawner = FindComponentInScene<EnemySpawner>();
        }

        if (combatManager == null)
        {
            combatManager = FindComponentInScene<CombatManager>();
        }

        if (bossController == null)
        {
            bossController = FindComponentInScene<BossController>();
        }

        if (combatDifficultyScaler == null)
        {
            combatDifficultyScaler = FindComponentInScene<CombatDifficultyScaler>();
        }

        if (combatHud == null)
        {
            combatHud = FindComponentInScene<CombatHUD2D>();
        }

        if (bossHealthUi == null)
        {
            bossHealthUi = FindComponentInScene<BossHealthUI>();
        }

        if (combatResultUi == null)
        {
            combatResultUi = FindComponentInScene<CombatResultUI>();
        }

        if (debugCombatUI == null)
        {
            debugCombatUI = FindComponentInScene<DebugCombatUI>();
        }

        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
    }

    private void EnsureRuntimePlayerComponents()
    {
        if (player == null)
        {
            return;
        }

        if (ensurePlayerExperience && player.GetComponent<PlayerExperience>() == null)
        {
            player.gameObject.AddComponent<PlayerExperience>();
        }

        if (ensurePlayerSpellController && player.GetComponent<PlayerSpellController>() == null)
        {
            player.gameObject.AddComponent<PlayerSpellController>();
        }

        if (ensurePlayerCultivationState && player.GetComponent<PlayerCultivationState>() == null)
        {
            player.gameObject.AddComponent<PlayerCultivationState>();
        }

        if (ensurePlayerUltimateController && player.GetComponent<PlayerUltimateController>() == null)
        {
            player.gameObject.AddComponent<PlayerUltimateController>();
        }
    }

    private void EnsureRuntimeCombatComponents()
    {
        if (ensureBossController && bossController == null)
        {
            bossController = gameObject.AddComponent<BossController>();
        }

        if (ensureBossHealthUi && bossHealthUi == null)
        {
            bossHealthUi = gameObject.AddComponent<BossHealthUI>();
        }

        if (ensureCombatDifficultyScaler && combatDifficultyScaler == null)
        {
            combatDifficultyScaler = gameObject.AddComponent<CombatDifficultyScaler>();
        }

        if (!ensureCombatResultUi)
        {
            return;
        }

        if (combatResultUi != null)
        {
            return;
        }

        combatResultUi = gameObject.AddComponent<CombatResultUI>();
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
