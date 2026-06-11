using System;
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
    [SerializeField] private bool ensureDialogueSystem = true;
    [SerializeField] private bool ensureBossController = true;
    [SerializeField] private bool ensureBossHealthUi = true;
    [SerializeField] private bool ensureCombatDifficultyScaler = true;
    [SerializeField] private bool ensureCombatResultUi = true;
    [SerializeField] private bool ensurePlayerExperience = true;
    [SerializeField] private bool ensurePlayerAnimatorSetup = true;
    [SerializeField] private bool ensureStageBackground = true;
    [SerializeField] private int backgroundSortingOrder = -10;
    [SerializeField] private string backgroundResourcePath = "Combat/bg";
    [SerializeField] private bool ensurePlayerCultivationState = true;
    [SerializeField] private bool ensurePlayerSpellController = true;
    [SerializeField] private bool ensurePlayerUltimateController = true;
    [SerializeField] private bool ensureTranThienKhiHud = true;
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
        ApplyStageConfiguration();
        EnsureStageBackground();
        EnsureRuntimePlayerComponents();
        RestorePlayerProgression();
        EnsureRuntimeCombatComponents();
        EnsureDialogueInfrastructure();

        if (logBootstrap)
        {
            Debug.Log($"Combat bootstrap ready. Arena: {arenaBounds != null}, Player: {player != null}, Spawner: {enemySpawner != null}");
        }

        SpawnNPCDialogueTriggers();
        PlayEntranceDialogue();
    }

    private void ApplyStageConfiguration()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogWarning("GameManager.Instance not found, skipping stage configuration.");
            return;
        }

        StageData stageData = GameManager.Instance.CurrentStage;
        if (stageData == null)
        {
            Debug.LogWarning("No current stage data found, skipping stage configuration.");
            return;
        }

        if (combatManager != null)
        {
            combatManager.Configure(stageData);
        }

        if (enemySpawner != null)
        {
            enemySpawner.ConfigureFromStage(stageData);
        }

        if (combatDifficultyScaler != null)
        {
            combatDifficultyScaler.ConfigureFromStage(stageData);
        }

        if (bossController != null)
        {
            bossController.ConfigureFromStage(stageData);
        }

        if (arenaBounds != null)
        {
            arenaBounds.SetSize(stageData.ArenaSize);
        }

        if (logBootstrap)
        {
            Debug.Log($"Stage configured: {stageData.StageName} ({stageData.CombatMode})");
        }
    }

    private void EnsureStageBackground()
    {
        if (!ensureStageBackground) return;

        if (arenaBounds == null) return;

        Sprite bgSprite = Resources.Load<Sprite>(backgroundResourcePath);
        if (bgSprite == null)
        {
            Debug.LogWarning($"Stage background sprite not found at: {backgroundResourcePath}");
            return;
        }

        GameObject bgGO = new GameObject("StageBackground", typeof(SpriteRenderer));
        bgGO.transform.SetParent(arenaBounds.transform.parent);

        SpriteRenderer sr = bgGO.GetComponent<SpriteRenderer>();
        sr.sprite = bgSprite;
        sr.sortingOrder = backgroundSortingOrder;

        Vector2 arenaCenter = arenaBounds.Center;
        Vector2 arenaSize = arenaBounds.Size;
        bgGO.transform.position = new Vector3(arenaCenter.x, arenaCenter.y, 0f);

        float spriteWidth = bgSprite.bounds.size.x;
        float spriteHeight = bgSprite.bounds.size.y;
        float scaleX = arenaSize.x / spriteWidth;
        float scaleY = arenaSize.y / spriteHeight;
        bgGO.transform.localScale = new Vector3(scaleX, scaleY, 1f);

        if (logBootstrap)
            Debug.Log($"Stage background created: {bgSprite.name}, scale=({scaleX:F2}, {scaleY:F2})");
    }

    private void RestorePlayerProgression()
    {
        if (GameManager.Instance == null)
        {
            return;
        }

        PlayerExperience exp = player != null
            ? player.GetComponent<PlayerExperience>()
            : FindComponentInScene<PlayerExperience>();

        GameManager.Instance.RestorePlayerProgression(exp);
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

        if (ensurePlayerAnimatorSetup)
        {
            SetupPlayerAnimator();
        }
    }

    private void SetupPlayerAnimator()
    {
        if (player == null) return;

        Animator animator = player.GetComponent<Animator>();
        if (animator == null)
        {
            animator = player.gameObject.AddComponent<Animator>();
        }

        string controllerPath = GameManager.Instance != null &&
                                GameManager.Instance.SelectedCharacter == CharacterType.Female
            ? "Animations/PlayerFemale"
            : "Animations/PlayerMale";

        RuntimeAnimatorController controller = Resources.Load<RuntimeAnimatorController>(controllerPath);
        if (controller != null)
        {
            animator.runtimeAnimatorController = controller;
        }

        SpriteRenderer sr = player.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.color = Color.white;
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

        if (ensureTranThienKhiHud && GetComponent<TranThienKhiHUD>() == null)
        {
            gameObject.AddComponent<TranThienKhiHUD>();
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

    private void EnsureDialogueInfrastructure()
    {
        if (!ensureDialogueSystem) return;

        if (FindComponentInScene<DialogueManager>() == null)
        {
            GameObject dialogueGO = new GameObject("DialogueSystem");
            dialogueGO.transform.SetParent(transform);
            dialogueGO.AddComponent<DialogueManager>();
            dialogueGO.AddComponent<DialogueUI>();
            if (logBootstrap)
                Debug.Log("CombatBootstrap: created DialogueSystem runtime.");
        }
    }

    private void PlayEntranceDialogue()
    {
        if (GameManager.Instance == null) return;
        StageData stageData = GameManager.Instance.CurrentStage;
        if (stageData == null) return;
        DialogueSO entranceDialogue = stageData.EntranceDialogue;
        if (entranceDialogue == null) return;

        DialogueManager dm = DialogueManager.Instance;
        if (dm == null) return;

        if (combatManager != null)
            combatManager.StartDialogue();

        if (player != null)
        {
            PlayerMovement2D movement = player.GetComponent<PlayerMovement2D>();
            if (movement != null)
                movement.CanMove = false;
        }

        System.Action onEnd = null;
        onEnd = () =>
        {
            dm.OnDialogueEnd -= onEnd;

            if (player != null)
            {
                PlayerMovement2D movement = player.GetComponent<PlayerMovement2D>();
                if (movement != null)
                    movement.CanMove = true;
            }

            if (combatManager != null)
            {
                combatManager.StartCombat();
            }
        };
        dm.OnDialogueEnd += onEnd;

        dm.PlayDialogue(entranceDialogue);
    }

    private void SpawnNPCDialogueTriggers()
    {
        if (GameManager.Instance == null) return;
        StageData stage = GameManager.Instance.CurrentStage;
        if (stage?.NPCSpawns == null) return;

        foreach (StageData.NPCSpawnEntry entry in stage.NPCSpawns)
        {
            if (entry.dialogue == null) continue;

            GameObject go = new GameObject($"NPCTrigger_{entry.dialogue.name}");
            go.transform.position = entry.spawnPosition;
            go.transform.SetParent(transform);

            if (entry.npcSprite != null)
            {
                SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
                sr.sprite = entry.npcSprite;
                sr.sortingOrder = 1;
            }

            CircleCollider2D col = go.AddComponent<CircleCollider2D>();
            col.isTrigger = true;
            col.radius = entry.triggerRadius > 0f ? entry.triggerRadius : 3f;

            DialogueProximityTrigger trigger = go.AddComponent<DialogueProximityTrigger>();
            trigger.SetDialogue(entry.dialogue);

            if (logBootstrap)
                Debug.Log($"Spawned NPC trigger: {entry.dialogue.name} at {entry.spawnPosition}");
        }
    }

    private static T FindComponentInScene<T>() where T : UnityEngine.Object
    {
#if UNITY_2023_1_OR_NEWER
        return FindFirstObjectByType<T>();
#else
        return FindObjectOfType<T>();
#endif
    }
}
