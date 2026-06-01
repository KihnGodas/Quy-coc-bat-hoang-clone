using UnityEngine;

public sealed class EnemyContactDamage2D : MonoBehaviour
{
    [SerializeField, Min(0f)] private float contactDamage = 10f;
    [SerializeField, Min(0f)] private float attackCooldown = 1f;

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

        PlayerHealth2D playerHealth = other.GetComponentInParent<PlayerHealth2D>();
        if (playerHealth == null)
        {
            return;
        }

        playerHealth.TakeDamage(contactDamage);
        nextAttackTime = Time.time + attackCooldown;
    }
}
