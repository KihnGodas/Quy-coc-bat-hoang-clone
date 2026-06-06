using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public sealed class EnemyChargeAttack : MonoBehaviour
{
    [SerializeField] private EnemyBase enemyBase;
    [SerializeField] private Rigidbody2D body;
    [SerializeField] private ArenaBounds arenaBounds;

    private bool isCharging;
    private bool dashDamageActive;
    private bool hitDuringCharge;
    private float nextChargeTime;

    private void Awake()
    {
        ResolveReferences();
    }

    private void Update()
    {
        if (enemyBase == null || enemyBase.Data == null || enemyBase.Role != EnemyRole.Charger)
        {
            return;
        }

        if (enemyBase.Target == null || enemyBase.State == EnemyState.Dead || isCharging)
        {
            return;
        }

        if (Time.time < nextChargeTime)
        {
            return;
        }

        float distance = Vector2.Distance(transform.position, enemyBase.Target.position);
        if (distance <= enemyBase.Data.ChargeRange)
        {
            StartCoroutine(ChargeRoutine());
        }
    }

    private IEnumerator ChargeRoutine()
    {
        EnemyData data = enemyBase.Data;
        if (data == null || enemyBase.Target == null)
        {
            yield break;
        }

        isCharging = true;
        dashDamageActive = false;
        hitDuringCharge = false;
        enemyBase.SetMovementLocked(true);
        nextChargeTime = Time.time + data.ChargeCooldown;
        SpriteRenderer spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        Sprite warningSprite = spriteRenderer != null ? spriteRenderer.sprite : null;
        int sortingOrder = spriteRenderer != null ? spriteRenderer.sortingOrder + 3 : 3;
        EnemyTelegraph2D.CreateCircle(
            "DaiLangChargeCircle",
            warningSprite,
            transform.position,
            0.72f,
            new Color(1f, 0.45f, 0.08f, 0.38f),
            sortingOrder,
            Mathf.Max(data.ChargeWindup, 0.1f),
            transform,
            false);
        EnemyTelegraph2D chargeLine = null;

        if (data.ChargeWindup > 0f)
        {
            float windupElapsed = 0f;
            while (windupElapsed < data.ChargeWindup && enemyBase.Target != null && enemyBase.State != EnemyState.Dead)
            {
                windupElapsed += Time.deltaTime;
                Vector2 start = transform.position;
                Vector2 directionToTarget = ((Vector2)enemyBase.Target.position - start).normalized;
                if (directionToTarget.sqrMagnitude < 0.0001f)
                {
                    directionToTarget = Vector2.right;
                }

                if (chargeLine == null)
                {
                    chargeLine = EnemyTelegraph2D.CreateLine(
                        "DaiLangDashDirection",
                        warningSprite,
                        start,
                        directionToTarget,
                        data.ChargeDistance,
                        0.11f,
                        new Color(1f, 0.64f, 0.1f, 0.48f),
                        sortingOrder,
                        Mathf.Max(data.ChargeWindup, 0.1f));
                }
                else
                {
                    chargeLine.SetLine(start, directionToTarget, data.ChargeDistance, 0.16f);
                }

                yield return null;
            }
        }

        if (enemyBase.Target == null || enemyBase.State == EnemyState.Dead)
        {
            enemyBase.SetMovementLocked(false);
            dashDamageActive = false;
            isCharging = false;
            yield break;
        }

        Vector2 startPosition = body.position;
        Vector2 direction = ((Vector2)enemyBase.Target.position - startPosition).normalized;
        if (direction.sqrMagnitude < 0.0001f)
        {
            direction = Vector2.right;
        }

        EnemyTelegraph2D.CreateFlash(
            "DaiLangChargeFlash",
            warningSprite,
            transform.position,
            new Vector2(1.05f, 1.05f),
            new Color(1f, 0.86f, 0.22f, 0.52f),
            sortingOrder + 1,
            0.15f,
            transform);
        dashDamageActive = true;

        float speed = Mathf.Max(data.ChargeSpeed, 0.01f);
        float distance = data.ChargeDistance;
        float travelTime = distance / speed;
        float elapsed = 0f;

        while (elapsed < travelTime && enemyBase.State != EnemyState.Dead && !hitDuringCharge)
        {
            elapsed += Time.fixedDeltaTime;
            Vector2 nextPosition = body.position + direction * speed * Time.fixedDeltaTime;

            FindArenaBoundsIfNeeded();
            if (arenaBounds != null)
            {
                nextPosition = arenaBounds.ClampPosition(nextPosition, data.ArenaPadding);
            }

            body.MovePosition(nextPosition);
            yield return new WaitForFixedUpdate();
        }

        enemyBase.SetMovementLocked(false);
        dashDamageActive = false;
        isCharging = false;
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        TryChargeDamage(collision.collider);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        TryChargeDamage(other);
    }

    private void TryChargeDamage(Collider2D other)
    {
        if (!dashDamageActive || hitDuringCharge || other == null || enemyBase == null || enemyBase.Data == null)
        {
            return;
        }

        Transform root = other.transform.root;
        if (!root.CompareTag("Player"))
        {
            return;
        }

        IDamageable damageable = other.GetComponentInParent<IDamageable>();
        if (damageable == null)
        {
            return;
        }

        Vector2 hitDirection = ((Vector2)other.transform.position - (Vector2)transform.position).normalized;
        float damage = enemyBase.Damage * enemyBase.Data.ChargeDamageMultiplier;
        damageable.TakeDamage(new DamageInfo(damage, gameObject, false, 0f, 1f, hitDirection));
        hitDuringCharge = true;
    }

    private void OnDisable()
    {
        if (enemyBase != null)
        {
            enemyBase.SetMovementLocked(false);
        }

        dashDamageActive = false;
        isCharging = false;
    }

    private void ResolveReferences()
    {
        if (enemyBase == null)
        {
            enemyBase = GetComponent<EnemyBase>();
        }

        if (body == null)
        {
            body = GetComponent<Rigidbody2D>();
        }

        FindArenaBoundsIfNeeded();
    }

    private void FindArenaBoundsIfNeeded()
    {
        if (arenaBounds != null)
        {
            return;
        }

        arenaBounds = ArenaBounds.Instance;

        if (arenaBounds == null)
        {
            arenaBounds = FindComponentInScene<ArenaBounds>();
        }
    }

    private static T FindComponentInScene<T>() where T : Object
    {
#if UNITY_2023_1_OR_NEWER
        return FindFirstObjectByType<T>();
#else
        return FindObjectOfType<T>();
#endif
    }
}
