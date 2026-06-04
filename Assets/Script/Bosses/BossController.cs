using UnityEngine;

public sealed class BossController : MonoBehaviour
{
    [SerializeField] private CombatManager combatManager;
    [SerializeField] private Transform player;
    [SerializeField] private BossBase bossPrefab;
    [SerializeField] private BossBase activeBoss;
    [SerializeField] private bool spawnPrototypeBoss = true;
    [SerializeField] private bool useAct1WoodBossPrototype = true;
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

        activeBoss = bossPrefab != null
            ? Instantiate(bossPrefab, spawnPosition, Quaternion.identity)
            : CreatePrototypeBoss();

        activeBoss.Initialize(prototypeBossName, prototypeBossHP, prototypeBossDamage, player);
        combatManager.SetBossHealth(activeBoss.Health);
    }

    private BossBase CreatePrototypeBoss()
    {
        GameObject bossObject = new GameObject(prototypeBossName);
        bossObject.transform.position = spawnPosition;
        bossObject.layer = LayerMask.NameToLayer("Enemy");
        bossObject.tag = "Enemy";

        bossObject.transform.localScale = new Vector3(prototypeBossScale.x, prototypeBossScale.y, 1f);

        CircleCollider2D collider = bossObject.AddComponent<CircleCollider2D>();
        collider.radius = 0.55f;
        collider.isTrigger = true;

        bossObject.AddComponent<Rigidbody2D>();
        Health health = bossObject.AddComponent<Health>();
        health.Initialize(prototypeBossHP, true);
        BossBase boss = bossObject.AddComponent<BossBase>();
        if (useAct1WoodBossPrototype)
        {
            bossObject.AddComponent<Act1WoodBossVisual>();
            bossObject.AddComponent<Act1WoodBossController>();
        }
        else
        {
            SpriteRenderer spriteRenderer = bossObject.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = BossRuntimeSprites.Circle;
            spriteRenderer.color = prototypeBossColor;
            spriteRenderer.sortingOrder = 2;
            bossObject.AddComponent<BossVisual2D>();
        }

        return boss;
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
}
