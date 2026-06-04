using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class Projectile2D : MonoBehaviour
{
    [SerializeField, Min(0f)] protected float speed = 12f;
    [SerializeField, Min(0.1f)] protected float lifetime = 1.5f;
    [SerializeField, Min(0f)] protected float damage = 10f;
    [SerializeField] protected bool destroyOnHit = true;
    [SerializeField] protected LayerMask targetLayers = ~0;
    [SerializeField] protected bool canCrit;
    [SerializeField, Range(0f, 1f)] protected float critChance;
    [SerializeField, Min(1f)] protected float critMultiplier = 2f;
    [SerializeField] protected bool pierceTargets;
    [SerializeField] protected bool appliesPoison;
    [SerializeField, Min(0f)] protected float poisonDuration;
    [SerializeField, Min(0f)] protected float poisonTickDamage;
    [SerializeField, Min(0.1f)] protected float poisonTickInterval = 1f;
    [SerializeField] protected Color poisonTintColor = new Color(0.25f, 1f, 0.15f, 1f);
    [SerializeField] protected bool spawnImpactEffect;
    [SerializeField] protected Color impactEffectColor = new Color(1f, 1f, 1f, 0.8f);
    [SerializeField, Min(0.01f)] protected float impactEffectScale = 1f;
    [SerializeField, Min(0.01f)] protected float impactEffectLifetime = 0.18f;

    protected Rigidbody2D body;
    protected Vector2 moveDirection = Vector2.right;
    protected Transform ownerRoot;
    protected GameObject sourceObject;
    protected float lifeTimer;
    private readonly System.Collections.Generic.HashSet<Transform> damagedRoots = new System.Collections.Generic.HashSet<Transform>();

    protected virtual void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        body.bodyType = RigidbodyType2D.Kinematic;
        body.gravityScale = 0f;

        Collider2D projectileCollider = GetComponent<Collider2D>();
        projectileCollider.isTrigger = true;
    }

    protected virtual void OnValidate()
    {
        Rigidbody2D projectileBody = GetComponent<Rigidbody2D>();
        if (projectileBody != null)
        {
            projectileBody.bodyType = RigidbodyType2D.Kinematic;
            projectileBody.gravityScale = 0f;
        }

        Collider2D projectileCollider = GetComponent<Collider2D>();
        if (projectileCollider != null)
        {
            projectileCollider.isTrigger = true;
        }
    }

    public virtual void Launch(Vector2 direction, Transform owner)
    {
        if (direction.sqrMagnitude > 0.0001f)
        {
            moveDirection = direction.normalized;
            transform.right = moveDirection;
        }

        ownerRoot = owner;
        sourceObject = owner != null ? owner.gameObject : gameObject;
        lifeTimer = 0f;
    }

    public virtual void Init(Vector2 direction, float damageAmount, float projectileSpeed, float projectileLifetime, GameObject source)
    {
        damage = Mathf.Max(0f, damageAmount);
        speed = Mathf.Max(0f, projectileSpeed);
        lifetime = Mathf.Max(0.01f, projectileLifetime);
        sourceObject = source != null ? source : gameObject;
        Launch(direction, sourceObject.transform);
    }

    public virtual void SetTargetLayers(LayerMask layers)
    {
        targetLayers = layers;
    }

    public virtual void ConfigureCrit(bool projectileCanCrit, float projectileCritChance, float projectileCritMultiplier)
    {
        canCrit = projectileCanCrit;
        critChance = Mathf.Clamp01(projectileCritChance);
        critMultiplier = Mathf.Max(1f, projectileCritMultiplier);
    }

    public virtual void ConfigurePierce(bool shouldPierceTargets)
    {
        pierceTargets = shouldPierceTargets;
    }

    public virtual void ConfigurePoison(float duration, float tickDamage, float tickInterval, Color tintColor)
    {
        appliesPoison = duration > 0f && tickDamage > 0f;
        poisonDuration = Mathf.Max(duration, 0f);
        poisonTickDamage = Mathf.Max(tickDamage, 0f);
        poisonTickInterval = Mathf.Max(tickInterval, 0.1f);
        poisonTintColor = tintColor;
    }

    public virtual void ConfigureImpactEffect(Color effectColor, float effectScale, float effectLifetime)
    {
        spawnImpactEffect = true;
        impactEffectColor = effectColor;
        impactEffectScale = Mathf.Max(0.01f, effectScale);
        impactEffectLifetime = Mathf.Max(0.01f, effectLifetime);
    }

    protected virtual void Update()
    {
        lifeTimer += Time.deltaTime;

        if (lifeTimer >= lifetime)
        {
            Destroy(gameObject);
        }
    }

    protected virtual void FixedUpdate()
    {
        Vector2 nextPosition = body.position + moveDirection * speed * Time.fixedDeltaTime;
        body.MovePosition(nextPosition);
    }

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (ownerRoot != null && other.transform.root == ownerRoot)
        {
            return;
        }

        if (other.GetComponentInParent<ExperienceOrb>() != null)
        {
            return;
        }

        if (!IsInTargetLayer(other.gameObject.layer))
        {
            return;
        }

        if (pierceTargets && damagedRoots.Contains(other.transform.root))
        {
            return;
        }

        GameObject hitObject = other.attachedRigidbody != null
            ? other.attachedRigidbody.gameObject
            : other.gameObject;

        IDamageable damageable = other.GetComponentInParent<IDamageable>();
        if (damageable != null)
        {
            if (pierceTargets)
            {
                damagedRoots.Add(other.transform.root);
            }

            damageable.TakeDamage(new DamageInfo(
                damage,
                sourceObject != null ? sourceObject : gameObject,
                canCrit,
                critChance,
                critMultiplier,
                moveDirection));

            ApplyPoisonIfNeeded(other);
            SpawnImpactEffectIfNeeded(other);
        }
        else
        {
            hitObject.SendMessage("TakeDamage", damage, SendMessageOptions.DontRequireReceiver);
            SpawnImpactEffectIfNeeded(other);
        }

        if (destroyOnHit && !pierceTargets)
        {
            Destroy(gameObject);
        }
    }

    protected bool IsInTargetLayer(int layer)
    {
        return (targetLayers.value & (1 << layer)) != 0;
    }

    private void ApplyPoisonIfNeeded(Collider2D other)
    {
        if (!appliesPoison || other == null)
        {
            return;
        }

        PlayerStatus2D status = other.GetComponentInParent<PlayerStatus2D>();
        if (status == null && other.transform.root.CompareTag("Player"))
        {
            status = other.transform.root.gameObject.AddComponent<PlayerStatus2D>();
        }

        if (status == null)
        {
            return;
        }

        status.ApplyPoison(
            poisonDuration,
            poisonTickDamage,
            poisonTickInterval,
            sourceObject != null ? sourceObject : gameObject,
            poisonTintColor);
    }

    private void SpawnImpactEffectIfNeeded(Collider2D other)
    {
        if (!spawnImpactEffect)
        {
            return;
        }

        SpriteRenderer renderer = GetComponentInChildren<SpriteRenderer>();
        if (renderer == null || renderer.sprite == null)
        {
            return;
        }

        Vector3 position = other != null ? other.ClosestPoint(transform.position) : transform.position;
        TemporaryEffect2D.CreateSpriteEffect(
            "ProjectileImpactEffect",
            renderer.sprite,
            position,
            Quaternion.identity,
            Vector3.one * impactEffectScale,
            impactEffectColor,
            renderer.sortingOrder + 2,
            impactEffectLifetime);
    }
}
