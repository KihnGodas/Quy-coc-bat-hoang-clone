using UnityEngine;

public sealed class DamageDealer : MonoBehaviour
{
    [SerializeField, Min(0f)] private float damage = 10f;
    [SerializeField] private GameObject sourceOverride;
    [SerializeField] private LayerMask targetLayers = ~0;
    [SerializeField] private bool damageOnTriggerEnter = true;
    [SerializeField] private bool damageOnCollisionEnter = true;
    [SerializeField] private bool destroyAfterDamage;
    [SerializeField] private bool canCrit;
    [SerializeField, Range(0f, 1f)] private float critChance;
    [SerializeField, Min(1f)] private float critMultiplier = 2f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (damageOnTriggerEnter)
        {
            TryDealDamage(other);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (damageOnCollisionEnter)
        {
            TryDealDamage(collision.collider);
        }
    }

    public bool TryDealDamage(Collider2D other)
    {
        if (other == null || !IsInTargetLayer(other.gameObject.layer))
        {
            return false;
        }

        IDamageable damageable = other.GetComponentInParent<IDamageable>();
        if (damageable == null)
        {
            return false;
        }

        Vector2 hitDirection = ((Vector2)other.transform.position - (Vector2)transform.position).normalized;
        DamageInfo damageInfo = new DamageInfo(
            damage,
            GetDamageSource(),
            canCrit,
            critChance,
            critMultiplier,
            hitDirection);

        damageable.TakeDamage(damageInfo);

        if (destroyAfterDamage)
        {
            Destroy(gameObject);
        }

        return true;
    }

    private GameObject GetDamageSource()
    {
        return sourceOverride != null ? sourceOverride : gameObject;
    }

    private bool IsInTargetLayer(int layer)
    {
        return (targetLayers.value & (1 << layer)) != 0;
    }
}
