using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public sealed class Projectile2D : MonoBehaviour
{
    [SerializeField, Min(0f)] private float speed = 12f;
    [SerializeField, Min(0.1f)] private float lifetime = 2f;
    [SerializeField, Min(0f)] private float damage = 1f;
    [SerializeField] private bool destroyOnHit = true;

    private Rigidbody2D body;
    private Vector2 moveDirection = Vector2.right;
    private Transform ownerRoot;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        body.bodyType = RigidbodyType2D.Kinematic;
        body.gravityScale = 0f;

        Collider2D projectileCollider = GetComponent<Collider2D>();
        projectileCollider.isTrigger = true;

        Destroy(gameObject, lifetime);
    }

    public void Launch(Vector2 direction, Transform owner)
    {
        if (direction.sqrMagnitude > 0.0001f)
        {
            moveDirection = direction.normalized;
            transform.right = moveDirection;
        }

        ownerRoot = owner;
    }

    private void FixedUpdate()
    {
        Vector2 nextPosition = body.position + moveDirection * speed * Time.fixedDeltaTime;
        body.MovePosition(nextPosition);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (ownerRoot != null && other.transform.root == ownerRoot)
        {
            return;
        }

        GameObject hitObject = other.attachedRigidbody != null
            ? other.attachedRigidbody.gameObject
            : other.gameObject;

        SimpleHealth health = other.GetComponentInParent<SimpleHealth>();
        if (health != null)
        {
            health.TakeDamage(damage);
        }
        else
        {
            hitObject.SendMessage("TakeDamage", damage, SendMessageOptions.DontRequireReceiver);
        }

        if (destroyOnHit)
        {
            Destroy(gameObject);
        }
    }
}
