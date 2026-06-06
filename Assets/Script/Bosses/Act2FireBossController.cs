using System.Collections;
using UnityEngine;

[RequireComponent(typeof(BossBase))]
public sealed class Act2FireBossController : MonoBehaviour
{
    [SerializeField] private BossBase boss;
    [SerializeField] private Transform target;
    [SerializeField, Min(0.1f)] private float warningTime = 0.75f;
    [SerializeField, Min(1f)] private float fireRingRadius = 5.2f;
    [SerializeField, Min(4)] private int meteorCount = 15;
    [SerializeField, Min(0.1f)] private float meteorWarningTime = 0.82f;
    [SerializeField, Min(1f)] private float dashDistance = 8.5f;
    [SerializeField, Min(0.1f)] private float dashDuration = 0.48f;
    [SerializeField, Min(0.1f)] private float dashWarningTime = 0.72f;
    [SerializeField, Min(1f)] private float burningZoneRadius = 1.45f;
    [SerializeField, Min(1)] private int burningZoneCount = 7;
    [SerializeField, Min(0.2f)] private float burningZoneLifetime = 4.5f;
    [SerializeField, Min(0.2f)] private float emberPulseInterval = 1.4f;
    [SerializeField, Min(0f)] private float emberPulseSpeed = 7.5f;

    [Header("Movement")]
    [SerializeField, Min(0f)] private float movementSpeed = 3.8f;
    [SerializeField, Min(0f)] private float enragedMovementSpeed = 5.2f;
    [SerializeField, Min(0.5f)] private float preferredMinDistance = 6.2f;
    [SerializeField, Min(0.5f)] private float preferredMaxDistance = 9f;
    [SerializeField, Min(0f)] private float orbitWeight = 0.75f;
    [SerializeField, Min(1f)] private float arenaClampRadius = 13f;

    private string currentSkillName = "Ready";
    private bool isDashing;
    private bool movementLocked;
    private Vector2 lastTargetPosition;
    private Vector2 targetVelocity;

    public string CurrentSkillName => currentSkillName;
    public bool IsEnraged => boss != null && boss.HealthPercent <= 0.45f;

    private void Awake()
    {
        boss = GetComponent<BossBase>();
    }

    private void Start()
    {
        ResolveTarget();
        if (target != null)
        {
            lastTargetPosition = target.position;
        }

        StartCoroutine(BossAttackLoop());
    }

    private void Update()
    {
        if (boss == null || boss.Health == null || boss.Health.IsDead)
        {
            return;
        }

        ResolveTarget();
        UpdateTargetVelocity();

        if (!isDashing && !movementLocked)
        {
            MoveTactically();
        }
    }

    private IEnumerator BossAttackLoop()
    {
        yield return new WaitForSeconds(1.25f);
        StartCoroutine(EmberPulseLoop());

        while (boss != null && boss.Health != null && !boss.Health.IsDead)
        {
            yield return FireRingRoutine();
            yield return new WaitForSeconds(IsEnraged ? 0.28f : 0.55f);
            yield return MeteorRainRoutine();
            yield return new WaitForSeconds(IsEnraged ? 0.25f : 0.5f);
            yield return FlameDashRoutine();
            yield return new WaitForSeconds(IsEnraged ? 0.28f : 0.5f);
            yield return BurningZoneRoutine();
            yield return new WaitForSeconds(IsEnraged ? 0.35f : 0.65f);
        }
    }

    private IEnumerator FireRingRoutine()
    {
        currentSkillName = "Fire Ring";
        movementLocked = true;
        Vector2 center = transform.position;
        BossSkillTelegraph2D.CreateCircle("FireRingWarning", center, fireRingRadius, new Color(1f, 0.16f, 0.02f, 0.34f), warningTime, 7, true);
        yield return new WaitForSeconds(warningTime);

        int flameCount = IsEnraged ? 24 : 18;
        float damage = boss.BaseDamage * (IsEnraged ? 1.55f : 1.25f);
        SpawnFireRing(center, fireRingRadius, flameCount, damage, new Color(1f, 0.28f, 0.03f, 0.92f));
        yield return new WaitForSeconds(0.22f);
        SpawnFireRing(center, fireRingRadius * 0.62f, IsEnraged ? 18 : 14, boss.BaseDamage * (IsEnraged ? 1.2f : 0.95f), new Color(1f, 0.16f, 0.02f, 0.86f));

        if (IsEnraged)
        {
            yield return new WaitForSeconds(0.28f);
            SpawnFireRing(center, fireRingRadius * 0.82f, 20, boss.BaseDamage * 1.1f, new Color(1f, 0.08f, 0.02f, 0.92f), Mathf.PI / 20f);
        }

        movementLocked = false;
        currentSkillName = "Ready";
    }

    private IEnumerator MeteorRainRoutine()
    {
        currentSkillName = "Meteor Rain";
        ResolveTarget();
        if (target == null)
        {
            yield break;
        }

        Vector2 center = target.position;
        int count = IsEnraged ? meteorCount + 7 : meteorCount;
        int predictiveCount = Mathf.Min(IsEnraged ? 6 : 4, count);
        for (int i = 0; i < count; i++)
        {
            Vector2 position = i < predictiveCount
                ? GetPredictiveMeteorPosition(center, i)
                : center + Random.insideUnitCircle * (IsEnraged ? 5.2f : 4.2f);
            BossSkillTelegraph2D.CreateCircle("MeteorWarning", position, 1.15f, new Color(1f, 0.05f, 0f, 0.48f), meteorWarningTime, 8, true);
            StartCoroutine(DelayedMeteor(position));
            yield return new WaitForSeconds(0.09f);
        }

        yield return new WaitForSeconds(meteorWarningTime + 0.2f);
        currentSkillName = "Ready";
    }

    private IEnumerator DelayedMeteor(Vector2 position)
    {
        yield return new WaitForSeconds(meteorWarningTime);
        SpawnFallingMeteorVisual(position);
        SpawnCircleHazard("MeteorImpact", position, 1.15f, boss.BaseDamage * (IsEnraged ? 1.65f : 1.35f), 0.32f, new Color(1f, 0.36f, 0.02f, 0.95f), true, 0.2f);
        SpawnCircleHazard("MeteorBurningGround", position, 1.3f, boss.BaseDamage * 0.22f, 2.3f, new Color(1f, 0.18f, 0.02f, 0.45f), false, 0.35f);
    }

    private IEnumerator FlameDashRoutine()
    {
        currentSkillName = "Flame Dash";
        ResolveTarget();
        if (target == null || isDashing)
        {
            yield break;
        }

        yield return SingleFlameDashRoutine();
        if (IsEnraged)
        {
            yield return new WaitForSeconds(0.35f);
            yield return SingleFlameDashRoutine();
            yield return new WaitForSeconds(0.32f);
            yield return SingleFlameDashRoutine();
        }

        currentSkillName = "Ready";
    }

    private IEnumerator SingleFlameDashRoutine()
    {
        ResolveTarget();
        if (target == null || isDashing)
        {
            yield break;
        }

        Vector2 start = transform.position;
        Vector2 direction = ((Vector2)target.position - start).normalized;
        if (direction.sqrMagnitude < 0.0001f)
        {
            direction = Vector2.right;
        }

        Vector2 center = start + direction * (dashDistance * 0.5f);
        BossSkillTelegraph2D.CreateRectangle("FlameDashWarning", center, direction, dashDistance, 1.65f, new Color(1f, 0.08f, 0f, 0.5f), dashWarningTime, 8, true);
        yield return new WaitForSeconds(dashWarningTime);

        isDashing = true;
        float elapsed = 0f;
        float nextTrailTime = 0f;
        Vector2 end = start + direction * dashDistance;
        while (elapsed < dashDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / dashDuration);
            transform.position = Vector2.Lerp(start, end, t);

            if (Time.time >= nextTrailTime)
            {
                SpawnCircleHazard("FlameDashTrail", transform.position, 0.78f, boss.BaseDamage * 0.18f, 1.4f, new Color(1f, 0.24f, 0.02f, 0.45f), false, 0.35f);
                nextTrailTime = Time.time + 0.08f;
            }

            yield return null;
        }

        SpawnRectangleHazard("FlameDashHitbox", center, direction, dashDistance, 1.65f, boss.BaseDamage * (IsEnraged ? 1.9f : 1.55f), 0.2f, new Color(1f, 0.2f, 0.02f, 0.88f));
        SpawnCircleHazard("FlameDashEndBurst", end, IsEnraged ? 1.8f : 1.45f, boss.BaseDamage * (IsEnraged ? 1.35f : 1f), 0.32f, new Color(1f, 0.34f, 0.02f, 0.9f), true, 0.2f);
        isDashing = false;
    }

    private IEnumerator BurningZoneRoutine()
    {
        currentSkillName = "Burning Zone";
        ResolveTarget();
        if (target == null)
        {
            yield break;
        }

        Vector2 anchor = target.position;
        int count = IsEnraged ? burningZoneCount + 3 : burningZoneCount;
        for (int i = 0; i < count; i++)
        {
            Vector2 position = anchor + Random.insideUnitCircle * (IsEnraged ? 5.2f : 4.6f);
            BossSkillTelegraph2D.CreateCircle("BurningZoneWarning", position, burningZoneRadius, new Color(1f, 0.36f, 0.02f, 0.36f), 0.55f, 7, true);
            StartCoroutine(DelayedBurningZone(position));
            yield return new WaitForSeconds(0.18f);
        }

        yield return new WaitForSeconds(1.1f);
        currentSkillName = "Ready";
    }

    private IEnumerator DelayedBurningZone(Vector2 position)
    {
        yield return new WaitForSeconds(0.55f);
        SpawnCircleHazard("BurningZone", position, burningZoneRadius, boss.BaseDamage * (IsEnraged ? 0.36f : 0.3f), burningZoneLifetime, new Color(1f, 0.15f, 0.02f, 0.5f), false, 0.45f);
    }

    private IEnumerator EmberPulseLoop()
    {
        float angleOffset = 0f;
        while (boss != null && boss.Health != null && !boss.Health.IsDead)
        {
            yield return new WaitForSeconds(IsEnraged ? emberPulseInterval * 0.72f : emberPulseInterval);
            int count = IsEnraged ? 8 : 6;
            float damage = boss.BaseDamage * (IsEnraged ? 0.38f : 0.3f);
            Color color = IsEnraged ? new Color(1f, 0.06f, 0.02f, 0.92f) : new Color(1f, 0.46f, 0.05f, 0.85f);

            for (int i = 0; i < count; i++)
            {
                float angle = angleOffset + i * 360f / count;
                Vector2 direction = Quaternion.Euler(0f, 0f, angle) * Vector2.right;
                SpawnProjectileHazard("EmberPulse", transform.position, direction, emberPulseSpeed, damage, color);
            }

            angleOffset += IsEnraged ? 22.5f : 15f;
        }
    }

    private void MoveTactically()
    {
        if (target == null)
        {
            return;
        }

        Vector2 position = transform.position;
        Vector2 toTarget = (Vector2)target.position - position;
        float distance = toTarget.magnitude;
        if (distance <= 0.001f)
        {
            return;
        }

        Vector2 towardTarget = toTarget / distance;
        Vector2 orbitDirection = new Vector2(-towardTarget.y, towardTarget.x);
        if (Mathf.Sin(Time.time * 0.35f) < 0f)
        {
            orbitDirection = -orbitDirection;
        }

        Vector2 desiredDirection;
        if (distance > preferredMaxDistance)
        {
            desiredDirection = Vector2.Lerp(towardTarget, orbitDirection, orbitWeight * 0.45f).normalized;
        }
        else if (distance < preferredMinDistance)
        {
            desiredDirection = Vector2.Lerp(-towardTarget, orbitDirection, orbitWeight * 0.55f).normalized;
        }
        else
        {
            Vector2 orbitBlend = Vector2.Lerp(towardTarget, orbitDirection, orbitWeight).normalized;
            desiredDirection = Vector2.Lerp(orbitBlend, targetVelocity.normalized, targetVelocity.sqrMagnitude > 0.01f ? 0.18f : 0f).normalized;
        }

        float speed = IsEnraged ? enragedMovementSpeed : movementSpeed;
        Vector2 nextPosition = position + desiredDirection * speed * Time.deltaTime;
        if (arenaClampRadius > 0f && nextPosition.magnitude > arenaClampRadius)
        {
            nextPosition = nextPosition.normalized * arenaClampRadius;
        }

        transform.position = nextPosition;
    }

    private void UpdateTargetVelocity()
    {
        if (target == null)
        {
            targetVelocity = Vector2.zero;
            return;
        }

        Vector2 currentPosition = target.position;
        if (Time.deltaTime > 0f)
        {
            targetVelocity = (currentPosition - lastTargetPosition) / Time.deltaTime;
        }

        lastTargetPosition = currentPosition;
    }

    private Vector2 GetPredictiveMeteorPosition(Vector2 center, int index)
    {
        Vector2 direction = targetVelocity.sqrMagnitude > 0.01f ? targetVelocity.normalized : Random.insideUnitCircle.normalized;
        if (direction.sqrMagnitude < 0.01f)
        {
            direction = Vector2.right;
        }

        Vector2 side = new Vector2(-direction.y, direction.x);
        float forwardDistance = 1.8f + index * 1.35f;
        float sideOffset = index % 2 == 0 ? 0.9f : -0.9f;
        return center + direction * forwardDistance + side * sideOffset;
    }

    private void SpawnProjectileHazard(string name, Vector2 position, Vector2 direction, float speed, float damage, Color color)
    {
        GameObject obj = CreateVisualHazard(name, position, BossRuntimeSprites.Circle, color, new Vector2(0.32f, 0.32f), 6);
        CircleCollider2D collider = obj.AddComponent<CircleCollider2D>();
        collider.radius = 0.5f;
        BossHazard2D hazard = obj.AddComponent<BossHazard2D>();
        hazard.Initialize(damage, 3.2f, direction.normalized * speed, gameObject, true, 0.2f);
    }

    private void SpawnFireRing(Vector2 center, float radius, int flameCount, float damage, Color color, float angleOffset = 0f)
    {
        for (int i = 0; i < flameCount; i++)
        {
            float angle = angleOffset + i * Mathf.PI * 2f / flameCount;
            Vector2 position = center + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;
            SpawnCircleHazard("EnragedFireRingFlame", position, 0.76f, damage, 0.4f, color, true, 0.2f);
        }
    }

    private void SpawnFallingMeteorVisual(Vector2 position)
    {
        GameObject meteor = CreateVisualHazard("FallingMeteorFlash", position + Vector2.up * 0.5f, BossRuntimeSprites.Circle, new Color(1f, 0.7f, 0.04f, 0.95f), new Vector2(0.68f, 0.68f), 9);
        Destroy(meteor, 0.28f);
    }

    private void SpawnCircleHazard(string name, Vector2 position, float radius, float damage, float lifetime, Color color, bool damageOnce, float tickInterval)
    {
        GameObject obj = CreateVisualHazard(name, position, BossRuntimeSprites.Circle, color, new Vector2(radius * 2f, radius * 2f), 6);
        CircleCollider2D collider = obj.AddComponent<CircleCollider2D>();
        collider.radius = 0.5f;
        BossHazard2D hazard = obj.AddComponent<BossHazard2D>();
        hazard.Initialize(damage, lifetime, Vector2.zero, gameObject, damageOnce, tickInterval);
    }

    private void SpawnRectangleHazard(string name, Vector2 center, Vector2 direction, float length, float width, float damage, float lifetime, Color color)
    {
        GameObject obj = CreateVisualHazard(name, center, BossRuntimeSprites.Square, color, new Vector2(length, width), 6);
        obj.transform.right = direction.sqrMagnitude > 0.0001f ? direction.normalized : Vector2.right;
        BoxCollider2D collider = obj.AddComponent<BoxCollider2D>();
        collider.size = Vector2.one;
        BossHazard2D hazard = obj.AddComponent<BossHazard2D>();
        hazard.Initialize(damage, lifetime, Vector2.zero, gameObject, true, 0.2f);
    }

    private GameObject CreateVisualHazard(string name, Vector2 position, Sprite sprite, Color color, Vector2 scale, int sortingOrder)
    {
        GameObject obj = new GameObject(name);
        obj.transform.position = position;
        obj.transform.localScale = new Vector3(scale.x, scale.y, 1f);
        SpriteRenderer renderer = obj.AddComponent<SpriteRenderer>();
        renderer.sprite = sprite;
        renderer.color = color;
        renderer.sortingOrder = sortingOrder;
        return obj;
    }

    private void ResolveTarget()
    {
        if (target == null && boss != null)
        {
            target = boss.Target;
        }

        if (target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                target = player.transform;
                boss.SetTarget(target);
            }
        }
    }
}
