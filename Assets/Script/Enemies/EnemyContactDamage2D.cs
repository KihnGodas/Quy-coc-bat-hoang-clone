using UnityEngine;

public sealed class EnemyContactDamage2D : MonoBehaviour
{
    [SerializeField, Min(0f)] private float contactDamage = 10f;
    [SerializeField, Min(0f)] private float attackCooldown = 1f;
    [SerializeField] private string targetTag = "Player";

    private float nextAttackTime;

    private void OnCollisionStay2D(Collision2D collision)
    {
        TryDamagePlayer(collision.collider);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        TryDamagePlayer(other);
    }

    private void TryDamagePlayer(Collider2D other)
    {
        if (Time.time < nextAttackTime)
        {
            return;
        }

        Transform root = other.transform.root;
        if (!string.IsNullOrEmpty(targetTag) && !root.CompareTag(targetTag))
        {
            return;
        }

        IDamageable damageable = other.GetComponentInParent<IDamageable>();
        if (damageable == null)
        {
            return;
        }

        Vector2 hitDirection = ((Vector2)other.transform.position - (Vector2)transform.position).normalized;
        damageable.TakeDamage(new DamageInfo(contactDamage, gameObject, false, 0f, 1f, hitDirection));
        nextAttackTime = Time.time + attackCooldown;
    }
}
