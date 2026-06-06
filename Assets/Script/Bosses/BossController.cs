using UnityEngine;

public sealed class BossController : MonoBehaviour
{
    public enum PrototypeBossType
    {
        Act1Wood,
        Act2Fire,
        Simple
    }

    [SerializeField] private CombatManager combatManager;
    [SerializeField] private Transform player;
    [SerializeField] private BossBase bossPrefab;
    [SerializeField] private BossBase activeBoss;
    [SerializeField] private bool spawnPrototypeBoss = true;
    [SerializeField] private bool useAct1WoodBossPrototype = true;
    [SerializeField] private PrototypeBossType prototypeBossType = PrototypeBossType.Act1Wood;
    [SerializeField] private bool useBossTypeDefaultStats = true;
    [SerializeField] private string prototypeBossName = "Boss Prototype - Co Thu Ton Gia";
    [SerializeField, Min(1f)] private float prototypeBossHP = 4500f;
    [SerializeField, Min(0f)] private float prototypeBossDamage = 60f;
    [SerializeField] private Vector2 spawnPosition = Vector2.zero;
    [SerializeField] private Color prototypeBossColor = new Color(0.18f, 0.95f, 0.35f, 1f);
    [SerializeField] private Vector2 prototypeBossScale = new Vector2(2.2f, 2.2f);

    public BossBase ActiveBoss => activeBoss;
    public bool HasActiveBoss => activeBoss != null && activeBoss.Health != null && !activeBoss.Health.IsDead;

    private void Awake()
    {
        ResolveReferences();
    }

    private void OnEnable()
    {
        ResolveReferences();
        if (combatManager != null)
        {
            combatManager.OnCombatStarted -= HandleCombatStarted;
            combatManager.OnCombatStarted += HandleCombatStarted;
        }
    }

    private void Start()
    {
        ResolveReferences();
        if (combatManager != null && combatManager.IsRunning && combatManager.Mode == CombatManager.CombatMode.BossCombat)
        {
            EnsureBossForBossCombat();
        }
    }

    private void OnDisable()
    {
        if (combatManager != null)
        {
            combatManager.OnCombatStarted -= HandleCombatStarted;
        }
    }

    public void ConfigureFromStage(StageData stageData)
    {
        if (stageData == null)
        {
            return;
        }

        if (stageData.CombatMode != CombatManager.CombatMode.BossCombat)
        {
            enabled = false;
            return;
        }

        enabled = true;
        spawnPrototypeBoss = false;

        if (stageData.BossPrefab != null)
        {
            bossPrefab = stageData.BossPrefab;
        }

        prototypeBossName = stageData.BossName;
        prototypeBossHP = stageData.BossHP;
        prototypeBossDamage = stageData.BossDamage;
        spawnPosition = stageData.BossSpawnPosition;
        prototypeBossScale = stageData.BossScale;
    }

    public void EnsureBossForBossCombat()
    {
        ResolveReferences();

        if (combatManager == null || combatManager.Mode != CombatManager.CombatMode.BossCombat)
        {
            return;
        }

        if (HasActiveBoss)
        {
            combatManager.SetBossHealth(activeBoss.Health);
            return;
        }

        if (!spawnPrototypeBoss)
        {
            return;
        }

        PrototypeStats stats = GetPrototypeStats();
        activeBoss = bossPrefab != null
            ? Instantiate(bossPrefab, spawnPosition, Quaternion.identity)
            : CreatePrototypeBoss(stats);

        activeBoss.Initialize(stats.Name, stats.HP, stats.Damage, player);
        combatManager.SetBossHealth(activeBoss.Health);
    }

    private BossBase CreatePrototypeBoss(PrototypeStats stats)
    {
        GameObject bossObject = new GameObject(stats.Name);
        bossObject.transform.position = spawnPosition;
        bossObject.layer = LayerMask.NameToLayer("Enemy");
        bossObject.tag = "Enemy";

        bossObject.transform.localScale = new Vector3(stats.Scale.x, stats.Scale.y, 1f);

        CircleCollider2D collider = bossObject.AddComponent<CircleCollider2D>();
        collider.radius = 0.55f;
        collider.isTrigger = true;

        bossObject.AddComponent<Rigidbody2D>();
        Health health = bossObject.AddComponent<Health>();
        health.Initialize(stats.HP, true);
        BossBase boss = bossObject.AddComponent<BossBase>();

        PrototypeBossType selectedType = ResolvePrototypeBossType();
        if (selectedType == PrototypeBossType.Act1Wood)
        {
            bossObject.AddComponent<Act1WoodBossVisual>();
            bossObject.AddComponent<Act1WoodBossController>();
        }
        else if (selectedType == PrototypeBossType.Act2Fire)
        {
            bossObject.AddComponent<Act2FireBossVisual>();
            bossObject.AddComponent<Act2FireBossController>();
        }
        else
        {
            SpriteRenderer spriteRenderer = bossObject.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = BossRuntimeSprites.Circle;
            spriteRenderer.color = stats.Color;
            spriteRenderer.sortingOrder = 2;
            bossObject.AddComponent<BossVisual2D>();
        }

        return boss;
    }

    private PrototypeStats GetPrototypeStats()
    {
        if (!useBossTypeDefaultStats)
        {
            return new PrototypeStats(prototypeBossName, prototypeBossHP, prototypeBossDamage, prototypeBossColor, prototypeBossScale);
        }

        switch (ResolvePrototypeBossType())
        {
            case PrototypeBossType.Act2Fire:
                return new PrototypeStats(
                    "Boss Act II - Huyet Diem Ton Gia",
                    9000f,
                    95f,
                    new Color(1f, 0.28f, 0.08f, 1f),
                    new Vector2(2.25f, 2.25f));
            case PrototypeBossType.Simple:
                return new PrototypeStats(prototypeBossName, prototypeBossHP, prototypeBossDamage, prototypeBossColor, prototypeBossScale);
            default:
                return new PrototypeStats(
                    "Boss Act I - Co Thu Ton Gia",
                    4500f,
                    60f,
                    new Color(0.18f, 0.95f, 0.35f, 1f),
                    new Vector2(2.2f, 2.2f));
        }
    }

    private PrototypeBossType ResolvePrototypeBossType()
    {
        if (prototypeBossType == PrototypeBossType.Act1Wood && !useAct1WoodBossPrototype)
        {
            return PrototypeBossType.Simple;
        }

        return prototypeBossType;
    }

    private void HandleCombatStarted()
    {
        EnsureBossForBossCombat();
    }

    private void ResolveReferences()
    {
        if (combatManager == null)
        {
            combatManager = FindComponentInScene<CombatManager>();
        }

        if (player == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null)
            {
                player = playerObject.transform;
            }
        }

        if (activeBoss == null)
        {
            activeBoss = FindComponentInScene<BossBase>();
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

    private readonly struct PrototypeStats
    {
        public PrototypeStats(string name, float hp, float damage, Color color, Vector2 scale)
        {
            Name = name;
            HP = hp;
            Damage = damage;
            Color = color;
            Scale = scale;
        }

        public string Name { get; }
        public float HP { get; }
        public float Damage { get; }
        public Color Color { get; }
        public Vector2 Scale { get; }
    }
}
