using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public sealed class BossHazard2D : MonoBehaviour
{
    [SerializeField, Min(0f)] private float damage;
    [SerializeField, Min(0.01f)] private float lifetime = 1f;
    [SerializeField] private Vector2 velocity;
    [SerializeField] private GameObject source;
    [SerializeField] private bool damageOnce = true;
    [SerializeField, Min(0.05f)] private float tickInterval = 0.35f;
    [SerializeField] private bool rootPlayer;
    [SerializeField, Min(0f)] private float rootDuration;

    private readonly HashSet<Transform> damagedTargets = new HashSet<Transform>();
    private readonly Dictionary<Transform, float> nextTickTimes = new Dictionary<Transform, float>();
    private float endTime;

    private void Awake()
    {
        Collider2D hitbox = GetComponent<Collider2D>();
        hitbox.isTrigger = true;
        endTime = Time.time + lifetime;
    }

    private void Update()
    {
        transform.position += (Vector3)(velocity * Time.deltaTime);
        if (Time.time >= endTime)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        TryHit(other);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        TryHit(other);
    }

    public void Initialize(float hazardDamage, float hazardLifetime, Vector2 hazardVelocity, GameObject hazardSource, bool shouldDamageOnce, float hazardTickInterval, bool shouldRootPlayer = false, float playerRootDuration = 0f)
    {
        damage = Mathf.Max(0f, hazardDamage);
        lifetime = Mathf.Max(0.01f, hazardLifetime);
        velocity = hazardVelocity;
        source = hazardSource;
        damageOnce = shouldDamageOnce;
        tickInterval = Mathf.Max(0.05f, hazardTickInterval);
        rootPlayer = shouldRootPlayer;
        rootDuration = Mathf.Max(0f, playerRootDuration);
        endTime = Time.time + lifetime;
    }

    private void TryHit(Collider2D other)
    {
        if (other == null || !other.transform.root.CompareTag("Player"))
        {
            return;
        }

        Transform root = other.transform.root;
        if (damageOnce && damagedTargets.Contains(root))
        {
            return;
        }

        if (!damageOnce && nextTickTimes.TryGetValue(root, out float nextTime) && Time.time < nextTime)
        {
            return;
        }

        IDamageable damageable = other.GetComponentInParent<IDamageable>();
        if (damageable != null && damage > 0f)
        {
            Vector2 hitDirection = ((Vector2)other.transform.position - (Vector2)transform.position).normalized;
            damageable.TakeDamage(new DamageInfo(damage, source, false, 0f, 1f, hitDirection));
        }

        if (rootPlayer && rootDuration > 0f)
        {
            PlayerStatus2D status = other.GetComponentInParent<PlayerStatus2D>();
            if (status != null)
            {
                status.ApplyRoot(rootDuration);
            }
        }

        damagedTargets.Add(root);
        nextTickTimes[root] = Time.time + tickInterval;
    }
}
