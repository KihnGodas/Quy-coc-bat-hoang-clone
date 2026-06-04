using UnityEngine;

public sealed class EnemyRoleController : MonoBehaviour
{
    [SerializeField] private EnemyBase enemyBase;

    private bool lowHealthPhaseActive;

    public float MoveSpeedMultiplier
    {
        get
        {
            UpdateLowHealthPhase();
            EnemyData data = Data;
            if (data == null)
            {
                return 1f;
            }

            if ((data.EnemyRole == EnemyRole.SpeedChaser || data.EnemyRole == EnemyRole.Berserker)
                && lowHealthPhaseActive)
            {
                return data.LowHealthMoveSpeedMultiplier;
            }

            return 1f;
        }
    }

    public float DamageMultiplier
    {
        get
        {
            UpdateLowHealthPhase();
            EnemyData data = Data;
            if (data == null)
            {
                return 1f;
            }

            if (data.EnemyRole == EnemyRole.Berserker && lowHealthPhaseActive)
            {
                return data.LowHealthDamageMultiplier;
            }

            return 1f;
        }
    }

    public float AttackRateMultiplier
    {
        get
        {
            UpdateLowHealthPhase();
            EnemyData data = Data;
            if (data == null)
            {
                return 1f;
            }

            if (data.EnemyRole == EnemyRole.Berserker && lowHealthPhaseActive)
            {
                return data.LowHealthAttackRateMultiplier;
            }

            return 1f;
        }
    }

    private EnemyData Data => enemyBase != null ? enemyBase.Data : null;

    private bool IsLowHealth
    {
        get
        {
            EnemyData data = Data;
            return enemyBase != null
                && data != null
                && enemyBase.HealthPercent <= data.LowHealthThreshold;
        }
    }

    private void Awake()
    {
        ResolveReferences();
    }

    private void Update()
    {
        UpdateLowHealthPhase();
    }

    public void OnEnemyDataChanged(EnemyData enemyData)
    {
        lowHealthPhaseActive = false;
        ResolveReferences();
    }

    private void ResolveReferences()
    {
        if (enemyBase == null)
        {
            enemyBase = GetComponent<EnemyBase>();
        }
    }

    private void UpdateLowHealthPhase()
    {
        if (lowHealthPhaseActive || !IsLowHealth)
        {
            return;
        }

        EnemyData data = Data;
        if (data == null)
        {
            return;
        }

        if (data.EnemyRole != EnemyRole.SpeedChaser && data.EnemyRole != EnemyRole.Berserker)
        {
            return;
        }

        lowHealthPhaseActive = true;
        if (data.EnemyRole == EnemyRole.Berserker && data.LowHealthMaxHealthMultiplier > 1f)
        {
            enemyBase.ApplyMaxHealthMultiplier(data.LowHealthMaxHealthMultiplier);
        }
    }
}
