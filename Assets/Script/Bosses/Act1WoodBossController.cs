using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BossBase))]
public sealed class Act1WoodBossController : MonoBehaviour
{
    [Header("Core")]
    [SerializeField] private BossBase boss;
    [SerializeField] private Transform target;
    [SerializeField, Min(1)] private int rootCount = 8;
    [SerializeField, Min(1f)] private float rootHP = 700f;
    [SerializeField, Min(0f)] private float shieldOpenDuration = 8f;
    [SerializeField, Min(3f)] private float rootSpawnRadius = 7.5f;

    [Header("Rotating Bullets")]
    [SerializeField, Min(0.5f)] private float rotatingBulletDuration = 3f;
    [SerializeField, Min(0.05f)] private float rotatingBulletInterval = 0.28f;
    [SerializeField, Min(0f)] private float rotatingBulletSpeed = 8f;
    [SerializeField, Min(0f)] private float enragedBulletSpeed = 10f;

    [Header("Root Skills")]
    [SerializeField, Min(0.1f)] private float warningTime = 0.5f;
    [SerializeField, Min(0f)] private float rootTrapCooldownDelay = 0.8f;
    [SerializeField, Min(0f)] private float groundRootStepDelay = 0.8f;
    [SerializeField, Min(0f)] private float sweepWarningTime = 1f;

    private readonly List<BossRootNode> roots = new List<BossRootNode>();
    private float shieldOpenUntil;
    private bool shieldPermanentlyBroken;

    public int RemainingRootCount
    {
        get
        {
            CleanupRoots();
            return roots.Count;
        }
    }

    public bool IsShieldActive => boss != null && boss.IsInvulnerable;

    private void Awake()
    {
        boss = GetComponent<BossBase>();
    }

    private void Start()
    {
        ResolveTarget();
        BuildRoots();
        SetShieldActive(true);
        StartCoroutine(BossAttackLoop());
    }

    private void Update()
    {
        if (boss == null || boss.Health == null || boss.Health.IsDead)
        {
            return;
        }

        if (!shieldPermanentlyBroken && RemainingRootCount > 0 && Time.time > shieldOpenUntil)
        {
            SetShieldActive(true);
        }
    }

    public void NotifyRootDestroyed(BossRootNode root)
    {
        roots.Remove(root);
        if (RemainingRootCount <= 0)
        {
            shieldPermanentlyBroken = true;
            SetShieldActive(false);
            CreateShieldBreakFlash(false);
            return;
        }

        shieldOpenUntil = Time.time + shieldOpenDuration;
        SetShieldActive(false);
        CreateShieldBreakFlash(true);
    }

    private IEnumerator BossAttackLoop()
    {
        yield return new WaitForSeconds(1.2f);

        while (boss != null && boss.Health != null && !boss.Health.IsDead)
        {
            yield return RotatingBulletRoutine();
            yield return new WaitForSeconds(0.8f);
            yield return RootTrapRoutine();
            yield return new WaitForSeconds(0.65f);
            yield return GroundRootRoutine();
            yield return new WaitForSeconds(0.65f);
            yield return GiantRootSweepRoutine();
            yield return new WaitForSeconds(0.8f);
        }
    }

    private IEnumerator RotatingBulletRoutine()
    {
        float startTime = Time.time;
        float angle = 0f;
        bool enraged = boss.HealthPercent <= 0.4f;
        float speed = enraged ? enragedBulletSpeed : rotatingBulletSpeed;
        float damage = boss.BaseDamage * (enraged ? 2f : 1.5f);
        Color color = enraged ? new Color(0.85f, 0.05f, 0.15f, 0.95f) : new Color(0.18f, 1f, 0.42f, 0.95f);

        while (Time.time - startTime < rotatingBulletDuration)
        {
            for (int i = 0; i < 8; i++)
            {
                float shotAngle = angle + i * 45f;
                Vector2 direction = Quaternion.Euler(0f, 0f, shotAngle) * Vector2.right;
                SpawnProjectileHazard(transform.position, direction, speed, damage, color);
            }

            angle += enraged ? 18f : 11f;
            yield return new WaitForSeconds(rotatingBulletInterval);
        }
    }

    private IEnumerator RootTrapRoutine()
    {
        ResolveTarget();
        if (target == null)
        {
            yield break;
        }

        Vector2 center = target.position;
        for (int i = 0; i < 5; i++)
        {
            Vector2 offset = Random.insideUnitCircle * 3.2f;
            Vector2 trapPosition = center + offset;
            BossSkillTelegraph2D.CreateCircle("BossRootTrapWarning", trapPosition, 1.25f, new Color(1f, 0.05f, 0.05f, 0.52f), warningTime, 7, true);
            StartCoroutine(ActivateRootTrap(trapPosition));
            yield return new WaitForSeconds(0.12f);
        }

        yield return new WaitForSeconds(rootTrapCooldownDelay);
    }

    private IEnumerator ActivateRootTrap(Vector2 position)
    {
        yield return new WaitForSeconds(warningTime);
        SpawnCircleHazard("BossRootTrap", position, 1.25f, boss.BaseDamage * 0.5f, 2.2f, new Color(0.36f, 0.22f, 0.08f, 0.85f), true, 1f);
        yield return new WaitForSeconds(1.3f);

        Vector2[] directions = { new Vector2(1f, 1f), new Vector2(-1f, 1f), new Vector2(1f, -1f), new Vector2(-1f, -1f) };
        for (int i = 0; i < directions.Length; i++)
        {
            Vector2 dir = directions[i].normalized;
            Vector2 rectCenter = position + dir * 1.45f;
            BossSkillTelegraph2D.CreateRectangle("BossDiagonalRootWarning", rectCenter, dir, 3.2f, 0.55f, new Color(1f, 0f, 0f, 0.45f), 0.32f, 8, true);
            SpawnRectangleHazard("BossDiagonalRootSlam", rectCenter, dir, 3.2f, 0.55f, boss.BaseDamage * 2f, 0.28f, new Color(0.4f, 0.24f, 0.08f, 0.92f));
        }
    }

    private IEnumerator GroundRootRoutine()
    {
        for (int i = 0; i < 8; i++)
        {
            ResolveTarget();
            if (target == null)
            {
                yield break;
            }

            Vector2 strikePosition = target.position;
            BossSkillTelegraph2D.CreateCircle("BossGroundRootWarning", strikePosition, 1.05f, new Color(1f, 0f, 0f, 0.55f), warningTime, 7, true);
            StartCoroutine(DelayedCircleStrike(strikePosition, 1.05f, boss.BaseDamage, warningTime, "BossGroundRootStrike"));
            yield return new WaitForSeconds(groundRootStepDelay);
        }
    }

    private IEnumerator DelayedCircleStrike(Vector2 position, float radius, float damage, float delay, string name)
    {
        yield return new WaitForSeconds(delay);
        SpawnCircleHazard(name, position, radius, damage, 0.25f, new Color(0.4f, 0.25f, 0.08f, 0.9f), false, 0f);
    }

    private IEnumerator GiantRootSweepRoutine()
    {
        ResolveTarget();
        Vector2 playerPosition = target != null ? (Vector2)target.position : Vector2.zero;
        bool horizontal = Random.value > 0.5f;
        Vector2 direction = horizontal ? Vector2.right : Vector2.up;
        Vector2 center = horizontal ? new Vector2(0f, playerPosition.y) : new Vector2(playerPosition.x, 0f);
        float length = 18f;
        float width = 1.8f;

        BossSkillTelegraph2D.CreateRectangle("BossGiantRootSweepWarning", center, direction, length, width, new Color(1f, 0f, 0f, 0.48f), sweepWarningTime, 7, true);
        yield return new WaitForSeconds(sweepWarningTime);
        SpawnRectangleHazard("BossGiantRootSweep", center, direction, length, width, boss.BaseDamage * 1.5f, 0.35f, new Color(0.33f, 0.18f, 0.06f, 0.95f));
    }

    private void BuildRoots()
    {
        CleanupRoots();
        for (int i = 0; i < rootCount; i++)
        {
            float angle = i * Mathf.PI * 2f / rootCount;
            Vector2 position = (Vector2)transform.position + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * rootSpawnRadius;
            roots.Add(CreateRootNode(position, i));
        }
    }

    private BossRootNode CreateRootNode(Vector2 position, int index)
    {
        GameObject rootObject = new GameObject($"WoodShieldRoot_{index + 1}");
        rootObject.transform.position = position;
        rootObject.layer = LayerMask.NameToLayer("Enemy");
        rootObject.tag = "Enemy";

        SpriteRenderer renderer = rootObject.AddComponent<SpriteRenderer>();
        renderer.sprite = BossRuntimeSprites.Square;
        renderer.color = new Color(0.33f, 0.22f, 0.1f, 1f);
        renderer.sortingOrder = 2;
        rootObject.transform.localScale = new Vector3(0.7f, 1.55f, 1f);
        rootObject.transform.rotation = Quaternion.Euler(0f, 0f, Random.Range(-25f, 25f));

        BoxCollider2D collider = rootObject.AddComponent<BoxCollider2D>();
        collider.isTrigger = true;
        Rigidbody2D body = rootObject.AddComponent<Rigidbody2D>();
        body.bodyType = RigidbodyType2D.Kinematic;
        body.gravityScale = 0f;
        rootObject.AddComponent<Health>();
        BossRootNode root = rootObject.AddComponent<BossRootNode>();
        root.Initialize(this, rootHP, new Color(0.33f, 0.22f, 0.1f, 1f));
        return root;
    }

    private void SpawnProjectileHazard(Vector2 position, Vector2 direction, float speed, float damage, Color color)
    {
        GameObject obj = CreateVisualHazard("BossRotatingBullet", position, BossRuntimeSprites.Circle, color, new Vector2(0.36f, 0.36f), 6);
        CircleCollider2D collider = obj.AddComponent<CircleCollider2D>();
        collider.radius = 0.5f;
        BossHazard2D hazard = obj.AddComponent<BossHazard2D>();
        hazard.Initialize(damage, 4f, direction.normalized * speed, gameObject, true, 0.2f);
    }

    private void SpawnCircleHazard(string name, Vector2 position, float radius, float damage, float lifetime, Color color, bool rootPlayer, float rootDuration)
    {
        GameObject obj = CreateVisualHazard(name, position, BossRuntimeSprites.Circle, color, new Vector2(radius * 2f, radius * 2f), 6);
        CircleCollider2D collider = obj.AddComponent<CircleCollider2D>();
        collider.radius = 0.5f;
        BossHazard2D hazard = obj.AddComponent<BossHazard2D>();
        hazard.Initialize(damage, lifetime, Vector2.zero, gameObject, true, 0.25f, rootPlayer, rootDuration);
    }

    private void SpawnRectangleHazard(string name, Vector2 center, Vector2 direction, float length, float width, float damage, float lifetime, Color color)
    {
        GameObject obj = CreateVisualHazard(name, center, BossRuntimeSprites.Square, color, new Vector2(length, width), 6);
        obj.transform.right = direction.sqrMagnitude > 0.0001f ? direction.normalized : Vector2.right;
        BoxCollider2D collider = obj.AddComponent<BoxCollider2D>();
        collider.size = Vector2.one;
        BossHazard2D hazard = obj.AddComponent<BossHazard2D>();
        hazard.Initialize(damage, lifetime, Vector2.zero, gameObject, true, 0.25f);
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

    private void SetShieldActive(bool active)
    {
        boss.SetInvulnerable(active);
    }

    private void CreateShieldBreakFlash(bool temporary)
    {
        Color color = temporary
            ? new Color(0.2f, 1f, 0.45f, 0.45f)
            : new Color(1f, 0.85f, 0.18f, 0.55f);
        BossSkillTelegraph2D.CreateCircle(temporary ? "WoodShieldOpenFlash" : "WoodShieldDestroyedFlash", transform.position, 2.2f, color, 0.65f, 8, true);
    }

    private void CleanupRoots()
    {
        for (int i = roots.Count - 1; i >= 0; i--)
        {
            if (roots[i] == null || roots[i].IsDead)
            {
                roots.RemoveAt(i);
            }
        }
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
