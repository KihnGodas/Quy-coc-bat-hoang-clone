using System;
using UnityEngine;

public sealed class CombatManager : MonoBehaviour
{
    public enum CombatMode
    {
        NormalCombat,
        BossCombat
    }

    public enum CombatState
    {
        NotStarted,
        InDialogue,
        Running,
        ClearingEnemies,
        Victory,
        Defeat
    }

    [SerializeField] private CombatMode combatMode = CombatMode.NormalCombat;
    [SerializeField, Min(0f)] private float normalCombatDuration = 20f;
    [SerializeField] private bool autoStart = true;
    [SerializeField] private bool controlEnemySpawner = true;
    [SerializeField] private PlayerHealth2D playerHealth;
    [SerializeField] private EnemySpawner enemySpawner;
    [SerializeField] private Health bossHealth;

    private readonly CombatTimer timer = new CombatTimer();
    private CombatState state = CombatState.NotStarted;
    private CombatResult result = CombatResult.None;

    public CombatMode Mode => combatMode;
    public CombatState State => state;
    public CombatResult Result => result;
    public float Duration => timer.Duration;
    public float ElapsedTime => timer.Elapsed;
    public float RemainingTime => combatMode == CombatMode.NormalCombat ? timer.Remaining : 0f;
    public int RemainingEnemyCount => enemySpawner != null ? enemySpawner.AliveCount : 0;
    public int TotalKills => enemySpawner != null ? enemySpawner.TotalKilled : 0;
    public bool IsInDialogue => state == CombatState.InDialogue;
    public bool IsClearingEnemies => state == CombatState.ClearingEnemies;
    public bool IsRunning => state == CombatState.Running;
    public bool IsBossCombat => combatMode == CombatMode.BossCombat;

    public event Action OnCombatStarted;
    public event Action OnCombatVictory;
    public event Action OnCombatDefeat;

    private void Awake()
    {
        ResolveReferences();
        SubscribeEvents();

        if (controlEnemySpawner && enemySpawner != null)
        {
            enemySpawner.SetSpawningEnabled(false);
        }
    }

    private void Start()
    {
        ResolveReferences();

        if (autoStart)
        {
            StartCombat();
        }
    }

    private void OnDestroy()
    {
        UnsubscribeEvents();
    }

    private void Update()
    {
        if (state == CombatState.Running && combatMode == CombatMode.NormalCombat)
        {
            timer.Tick(Time.deltaTime);
            if (timer.IsComplete)
            {
                BeginClearingRemainingEnemies();
            }

            return;
        }

        if (state == CombatState.ClearingEnemies && RemainingEnemyCount <= 0)
        {
            CompleteCombat(CombatResult.Victory);
        }
    }

    public void StartDialogue()
    {
        if (state != CombatState.NotStarted) return;
        state = CombatState.InDialogue;
    }

    public void StartCombat()
    {
        if (state == CombatState.Running)
        {
            return;
        }

        ResolveReferences();
        result = CombatResult.None;
        state = CombatState.Running;

        if (combatMode == CombatMode.NormalCombat)
        {
            timer.Start(normalCombatDuration);
        }
        else
        {
            timer.Stop();
        }

        if (controlEnemySpawner && enemySpawner != null)
        {
            enemySpawner.SetSpawningEnabled(combatMode == CombatMode.NormalCombat);
        }

        OnCombatStarted?.Invoke();
    }

    public void SetCombatMode(CombatMode newCombatMode)
    {
        if (state == CombatState.Running || state == CombatState.ClearingEnemies)
        {
            return;
        }

        combatMode = newCombatMode;
    }

    public void Configure(StageData stageData)
    {
        if (stageData == null)
        {
            return;
        }

        combatMode = stageData.CombatMode;

        if (combatMode == CombatMode.NormalCombat)
        {
            normalCombatDuration = stageData.Duration;
        }
    }

    public void CompleteCombat(CombatResult combatResult)
    {
        if (state != CombatState.Running && state != CombatState.ClearingEnemies)
        {
            return;
        }

        result = combatResult;
        state = combatResult == CombatResult.Victory ? CombatState.Victory : CombatState.Defeat;
        timer.Stop();

        if (controlEnemySpawner && enemySpawner != null)
        {
            enemySpawner.SetSpawningEnabled(false);
        }

        if (combatResult == CombatResult.Victory)
        {
            GrantTranThienKhiReward();
            OnCombatVictory?.Invoke();
        }
        else if (combatResult == CombatResult.Defeat)
        {
            OnCombatDefeat?.Invoke();
        }
    }

    private void GrantTranThienKhiReward()
    {
        if (GameManager.Instance == null) return;

        TranThienKhiType reward = GetTranThienKhiForStage(GameManager.Instance.CurrentStageIndex);
        if (reward != TranThienKhiType.None)
        {
            GameManager.Instance.CollectTranThienKhi(reward);
        }
    }

    private static TranThienKhiType GetTranThienKhiForStage(int stageIndex)
    {
        return stageIndex switch
        {
            6 => TranThienKhiType.Moc,
            7 => TranThienKhiType.Hoa,
            8 => TranThienKhiType.Kim,
            9 => TranThienKhiType.Thuy,
            _ => TranThienKhiType.None
        };
    }

    public void SetBossHealth(Health newBossHealth)
    {
        if (bossHealth == newBossHealth)
        {
            return;
        }

        if (bossHealth != null)
        {
            bossHealth.OnDeath -= HandleBossDeath;
        }

        bossHealth = newBossHealth;

        if (bossHealth != null)
        {
            bossHealth.OnDeath += HandleBossDeath;
        }
    }

    private void ResolveReferences()
    {
        if (playerHealth == null)
        {
            playerHealth = FindComponentInScene<PlayerHealth2D>();
        }

        if (enemySpawner == null)
        {
            enemySpawner = FindComponentInScene<EnemySpawner>();
        }

        if (bossHealth == null)
        {
            BossBase boss = FindComponentInScene<BossBase>();
            if (boss != null)
            {
                SetBossHealth(boss.Health);
            }
        }
    }

    private void BeginClearingRemainingEnemies()
    {
        if (state != CombatState.Running)
        {
            return;
        }

        state = CombatState.ClearingEnemies;
        timer.Stop();

        if (controlEnemySpawner && enemySpawner != null)
        {
            enemySpawner.SetSpawningEnabled(false);
        }

        if (RemainingEnemyCount <= 0)
        {
            CompleteCombat(CombatResult.Victory);
        }
    }

    private void SubscribeEvents()
    {
        if (playerHealth != null)
        {
            playerHealth.OnDeath -= HandlePlayerDeath;
            playerHealth.OnDeath += HandlePlayerDeath;
        }

        if (bossHealth != null)
        {
            bossHealth.OnDeath -= HandleBossDeath;
            bossHealth.OnDeath += HandleBossDeath;
        }
    }

    private void UnsubscribeEvents()
    {
        if (playerHealth != null)
        {
            playerHealth.OnDeath -= HandlePlayerDeath;
        }

        if (bossHealth != null)
        {
            bossHealth.OnDeath -= HandleBossDeath;
        }
    }

    private void HandlePlayerDeath()
    {
        CompleteCombat(CombatResult.Defeat);
    }

    private void HandleBossDeath()
    {
        if (combatMode == CombatMode.BossCombat)
        {
            CompleteCombat(CombatResult.Victory);
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
