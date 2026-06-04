using UnityEngine;

public sealed class EnemyMeleeAttack : EnemyAttackBase
{
    private void OnCollisionStay2D(Collision2D collision)
    {
        TryDamage(collision.collider);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        TryDamage(other);
    }

    private void TryDamage(Collider2D other)
    {
        ResolveReferences();

        if (!CanAttack || !IsValidTarget(other) || !CanRoleUseMeleeContact())
        {
            return;
        }

        IDamageable damageable = other.GetComponentInParent<IDamageable>();
        if (damageable == null)
        {
            return;
        }

        Vector2 hitDirection = ((Vector2)other.transform.position - (Vector2)transform.position).normalized;
        damageable.TakeDamage(new DamageInfo(enemyBase.Damage, gameObject, false, 0f, 1f, hitDirection));
        MarkAttackUsed();
    }

    private bool CanRoleUseMeleeContact()
    {
        if (enemyBase == null)
        {
            return false;
        }

        EnemyRole role = enemyBase.Role;
        return role == EnemyRole.MeleeChaser
            || role == EnemyRole.SpeedChaser
            || role == EnemyRole.Tank
            || role == EnemyRole.Berserker
            || role == EnemyRole.HybridThrower;
    }
}
