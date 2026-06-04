using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(EnemyMovement))]
public class EnemyBase : MonoBehaviour
{
    [SerializeField] private EnemyData enemyData;
    [SerializeField] private Transform target;
    [SerializeField] private EnemyMovement movement;
    [SerializeField] private Health health;
    [SerializeField] private SimpleHealth simpleHealth;
    [SerializeField] private EnemyRoleController roleController;
    [SerializeField] private EnemyStatus2D enemyStatus;

    private EnemyState state = EnemyState.Idle;
    private bool initialized;
    private bool movementLocked;

    public EnemyData Data => enemyData;
    public Transform Target => target;
    public EnemyState State => state;
    public bool IsMovementLocked => movementLocked;
    public float HealthPercent
    {
        get
        {
            if (health != null && health.MaxHP > 0f)
            {
                return Mathf.Clamp01(health.CurrentHP / health.MaxHP);
            }

            if (simpleHealth != null && simpleHealth.MaxHealth > 0f)
            {
                return Mathf.Clamp01(simpleHealth.CurrentHealth / simpleHealth.MaxHealth);
            }

            return 1f;
        }
    }

    public float Damage => (enemyData != null ? enemyData.Damage : 10f) * GetDamageMultiplier();
    public float MoveSpeed => (enemyData != null ? enemyData.MoveSpeed : 2.5f) * GetMoveSpeedMultiplier();
    public float AttackRange => enemyData != null ? enemyData.AttackRange : 1.1f;
    public float AttackCooldown => Mathf.Max((enemyData != null ? enemyData.AttackCooldown : 1f) / GetAttackRateMultiplier(), 0.01f);
    public EnemyRole Role => enemyData != null ? enemyData.EnemyRole : EnemyRole.MeleeChaser;

    protected virtual void Awake()
    {
        ResolveReferences();
        InitializeFromData();
        SubscribeHealthEvents();
    }

    protected virtual void OnDestroy()
    {
        UnsubscribeHealthEvents();
    }

    protected virtual void Update()
    {
        if (state == EnemyState.Dead)
        {
            return;
        }

        if (movementLocked || IsMovementBlockedByStatus())
        {
            SetState(EnemyState.Casting);
            return;
        }

        ResolveTargetIfNeeded();

        if (target == null)
        {
            SetState(EnemyState.Idle);
            return;
        }

        float distance = Vector2.Distance(transform.position, target.position);
        SetState(distance <= AttackRange ? EnemyState.Attack : EnemyState.Chase);
    }

    protected virtual void FixedUpdate()
    {
        if (state != EnemyState.Chase || target == null || movement == null || movementLocked || IsMovementBlockedByStatus())
        {
            return;
        }

        bool rotateToMoveDirection = enemyData != null && enemyData.RotateToMoveDirection;
        float arenaPadding = enemyData != null ? enemyData.ArenaPadding : 0.35f;
        movement.MoveTowards(target.position, MoveSpeed, rotateToMoveDirection, arenaPadding);
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    public void SetData(EnemyData newData, bool reinitialize = true)
    {
        enemyData = newData;
        ResolveReferences();

        if (reinitialize)
        {
            initialized = false;
            InitializeFromData();
        }

        BroadcastMessage("OnEnemyDataChanged", enemyData, SendMessageOptions.DontRequireReceiver);
    }

    public void SetMovementLocked(bool locked)
    {
        movementLocked = locked;
        if (movementLocked && state != EnemyState.Dead)
        {
            SetState(EnemyState.Casting);
        }
        else if (!movementLocked && state == EnemyState.Casting)
        {
            SetState(EnemyState.Idle);
        }
    }

    public void ApplyMaxHealthMultiplier(float multiplier)
    {
        if (enemyData == null || multiplier <= 0f)
        {
            return;
        }

        float scaledMaxHealth = enemyData.MaxHP * multiplier;
        if (health != null)
        {
            health.Initialize(scaledMaxHealth, false);
        }
        else if (simpleHealth != null)
        {
            simpleHealth.Initialize(scaledMaxHealth, false);
        }
    }

    protected void SetState(EnemyState newState)
    {
        if (state == EnemyState.Dead || state == newState)
        {
            return;
        }

        state = newState;
    }

    protected virtual void ResolveReferences()
    {
        if (movement == null)
        {
            movement = GetComponent<EnemyMovement>();
        }

        if (health == null)
        {
            health = GetComponent<Health>();
        }

        if (simpleHealth == null)
        {
            simpleHealth = GetComponent<SimpleHealth>();
        }

        if (roleController == null)
        {
            roleController = GetComponent<EnemyRoleController>();
        }

        if (enemyStatus == null)
        {
            enemyStatus = GetComponent<EnemyStatus2D>();
        }

        ResolveTargetIfNeeded();
    }

    private bool IsMovementBlockedByStatus()
    {
        if (enemyStatus == null)
        {
            enemyStatus = GetComponent<EnemyStatus2D>();
        }

        return enemyStatus != null && enemyStatus.BlocksMovement;
    }

    protected void ResolveTargetIfNeeded()
    {
        if (target != null)
        {
            return;
        }

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            target = playerObject.transform;
        }
    }

    private void InitializeFromData()
    {
        if (initialized || enemyData == null)
        {
            return;
        }

        if (health != null)
        {
            health.Initialize(enemyData.MaxHP);
        }
        else if (simpleHealth != null)
        {
            simpleHealth.Initialize(enemyData.MaxHP);
        }

        initialized = true;
    }

    private float GetMoveSpeedMultiplier()
    {
        return roleController != null ? roleController.MoveSpeedMultiplier : 1f;
    }

    private float GetDamageMultiplier()
    {
        return roleController != null ? roleController.DamageMultiplier : 1f;
    }

    private float GetAttackRateMultiplier()
    {
        return roleController != null ? roleController.AttackRateMultiplier : 1f;
    }

    private void SubscribeHealthEvents()
    {
        if (health != null)
        {
            health.OnDeath -= HandleDeath;
            health.OnDeath += HandleDeath;
        }

        if (simpleHealth != null)
        {
            simpleHealth.OnDeath -= HandleDeath;
            simpleHealth.OnDeath += HandleDeath;
        }
    }

    private void UnsubscribeHealthEvents()
    {
        if (health != null)
        {
            health.OnDeath -= HandleDeath;
        }

        if (simpleHealth != null)
        {
            simpleHealth.OnDeath -= HandleDeath;
        }
    }

    private void HandleDeath()
    {
        state = EnemyState.Dead;
    }
}
