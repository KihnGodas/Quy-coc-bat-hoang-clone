using UnityEngine;

public abstract class EnemyAttackBase : MonoBehaviour
{
    [SerializeField] protected EnemyBase enemyBase;
    [SerializeField] protected EnemyStatus2D enemyStatus;
    [SerializeField] protected string targetTag = "Player";

    protected float nextAttackTime;

    protected virtual void Awake()
    {
        ResolveReferences();
    }

    public bool CanAttack => enemyBase != null
        && enemyBase.State != EnemyState.Dead
        && (enemyStatus == null || !enemyStatus.BlocksAttack)
        && Time.time >= nextAttackTime;

    public virtual void ResolveReferences()
    {
        if (enemyBase == null)
        {
            enemyBase = GetComponent<EnemyBase>();
        }

        if (enemyStatus == null)
        {
            enemyStatus = GetComponent<EnemyStatus2D>();
        }
    }

    protected bool IsValidTarget(Collider2D other)
    {
        if (other == null)
        {
            return false;
        }

        Transform root = other.transform.root;
        return string.IsNullOrEmpty(targetTag) || root.CompareTag(targetTag);
    }

    protected void MarkAttackUsed()
    {
        float cooldown = enemyBase != null ? enemyBase.AttackCooldown : 0f;
        nextAttackTime = Time.time + cooldown;
    }
}
