using UnityEngine;

public sealed class BoomerangProjectile2D : Projectile2D
{
    [SerializeField, Min(0f)] private float returnDelay = 0.45f;
    [SerializeField] private bool returnToOwner = true;
    [SerializeField] private Transform returnTarget;

    private float flightTimer;

    public void ConfigureReturn(float delay, Transform target, bool shouldReturnToOwner = true)
    {
        returnDelay = Mathf.Max(0f, delay);
        returnToOwner = shouldReturnToOwner;
        returnTarget = target;
    }

    public override void Launch(Vector2 direction, Transform owner)
    {
        base.Launch(direction, owner);
        flightTimer = 0f;

        if (returnToOwner && returnTarget == null)
        {
            returnTarget = owner;
        }
    }

    public override void Init(Vector2 direction, float damageAmount, float projectileSpeed, float projectileLifetime, GameObject source)
    {
        base.Init(direction, damageAmount, projectileSpeed, projectileLifetime, source);

        if (returnToOwner && returnTarget == null && source != null)
        {
            returnTarget = source.transform;
        }
    }

    protected override void FixedUpdate()
    {
        flightTimer += Time.fixedDeltaTime;

        if (flightTimer >= returnDelay && returnTarget != null)
        {
            Vector2 toTarget = (Vector2)returnTarget.position - body.position;
            if (toTarget.sqrMagnitude > 0.0001f)
            {
                moveDirection = toTarget.normalized;
                transform.right = moveDirection;
            }
        }

        base.FixedUpdate();
    }
}
