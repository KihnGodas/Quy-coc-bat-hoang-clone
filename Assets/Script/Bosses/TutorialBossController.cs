using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BossBase))]
public sealed class TutorialBossController : MonoBehaviour
{
    private enum TutorialBossState
    {
        SatHach,
        KetThuc
    }

    [SerializeField] private BossBase boss;
    [SerializeField] private Transform target;
    [SerializeField] private CombatManager combatManager;
    [SerializeField, Min(1f)] private float trialDuration = 30f;
    [SerializeField, Min(0f)] private float movementSpeed = 10f;
    [SerializeField, Min(0.5f)] private float preferredDistance = 9f;
    [SerializeField, Min(1f)] private float arenaClampRadius = 13f;
    [SerializeField, Range(0f, 1f)] private float damageTakenMultiplier = 0.02f;
    [SerializeField] private string finalLine = "Mọi chuyện kết thúc ở đây thôi";

    [Header("Kiem Khi")]
    [SerializeField, Min(0.1f)] private float kiemKhiCooldown = 0.85f;
    [SerializeField, Min(1)] private int kiemKhiCount = 6;
    [SerializeField, Min(0f)] private float kiemKhiSpreadAngle = 18f;
    [SerializeField, Min(0.1f)] private float kiemKhiSpeed = 20f;
    [SerializeField, Min(0.1f)] private float kiemKhiLifetime = 2f;
    [SerializeField, Min(0.1f)] private float kiemKhiWidth = 2f;

    [Header("Phi Kiem")]
    [SerializeField, Min(0.1f)] private float phiKiemCooldown = 2.1f;
    [SerializeField, Min(1)] private int phiKiemCount = 10;
    [SerializeField, Min(0.01f)] private float phiKiemStepDelay = 0.2f;
    [SerializeField, Min(0.1f)] private float phiKiemTelegraphTime = 0.85f;
    [SerializeField, Min(0.1f)] private float phiKiemSpeed = 18f;

    [Header("Dao Van")]
    [SerializeField, Min(0.1f)] private float daoVanCooldown = 2.8f;
    [SerializeField, Min(1)] private int daoVanCount = 12;
    [SerializeField, Min(0.1f)] private float daoVanSearchRadius = 8f;
    [SerializeField, Min(0.1f)] private float daoVanRadius = 1.5f;
    [SerializeField, Min(0.1f)] private float daoVanTelegraphTime = 0.85f;

    [Header("Ket Thuc")]
    [SerializeField, Min(0.1f)] private float finalStrikeDelay = 1.35f;
    [SerializeField, Min(0f)] private float finalStrikeDamage = 99999f;
    [SerializeField, Min(0.1f)] private float finalStrikeRadius = 3.2f;
    [SerializeField, Min(0.03f)] private float finalLockLineInterval = 0.16f;

    private TutorialBossState state = TutorialBossState.SatHach;
    private float endTime;
    private string currentSkillName = "Sat Hach";
    private Coroutine attackLoop;
    private GUIStyle dialogueStyle;

    public float RemainingTrialTime => state == TutorialBossState.SatHach ? Mathf.Max(0f, endTime - Time.time) : 0f;
    public string CurrentSkillName => currentSkillName;
    public bool IsEnding => state == TutorialBossState.KetThuc;
    public string FinalLine => finalLine;

    private void Awake()
    {
        boss = GetComponent<BossBase>();
    }

    private void Start()
    {
        ResolveReferences();
        boss.SetInvulnerable(false);
        if (boss.Health != null)
        {
            boss.Health.SetDamageTakenMultiplier(damageTakenMultiplier);
        }

        endTime = Time.time + trialDuration;
        attackLoop = StartCoroutine(AttackLoop());
    }

    private void Update()
    {
        if (boss == null || boss.Health == null || boss.Health.IsDead || state == TutorialBossState.KetThuc)
        {
            return;
        }

        ResolveReferences();
        MoveFreely();

        if (Time.time >= endTime)
        {
            StartCoroutine(EndTrialRoutine());
        }
    }

    private void OnGUI()
    {
        if (state != TutorialBossState.KetThuc)
        {
            return;
        }

        EnsureDialogueStyle();
        Rect rect = new Rect(0f, Screen.height * 0.68f, Screen.width, 60f);
        GUI.Label(rect, $"\"{finalLine}\"", dialogueStyle);
    }

    private IEnumerator AttackLoop()
    {
        yield return new WaitForSeconds(0.9f);

        while (state == TutorialBossState.SatHach)
        {
            yield return KiemKhiRoutine();
            yield return new WaitForSeconds(kiemKhiCooldown);
            yield return PhiKiemRoutine();
            yield return new WaitForSeconds(phiKiemCooldown);
            yield return DaoVanRoutine();
            yield return new WaitForSeconds(daoVanCooldown);
        }
    }

    private IEnumerator KiemKhiRoutine()
    {
        currentSkillName = "Kiem Khi";
        ResolveReferences();
        if (target == null)
        {
            yield break;
        }

        Vector2 start = transform.position;
        Vector2 direction = ((Vector2)target.position - start).normalized;
        if (direction.sqrMagnitude < 0.0001f)
        {
            direction = Vector2.right;
        }

        float startAngle = kiemKhiCount > 1 ? -kiemKhiSpreadAngle * 0.5f : 0f;
        float angleStep = kiemKhiCount > 1 ? kiemKhiSpreadAngle / (kiemKhiCount - 1) : 0f;
        for (int i = 0; i < kiemKhiCount; i++)
        {
            Vector2 shotDirection = Quaternion.Euler(0f, 0f, startAngle + angleStep * i) * direction;
            SpawnRectangleProjectile("KiemKhi", start + shotDirection * 1.1f, shotDirection, 1.25f, kiemKhiWidth, boss.BaseDamage, kiemKhiLifetime, kiemKhiSpeed, new Color(0.72f, 0.92f, 1f, 0.82f));
        }

        currentSkillName = "Sat Hach";
        yield return null;
    }

    private IEnumerator PhiKiemRoutine()
    {
        currentSkillName = "Phi Kiem";
        ResolveReferences();
        if (target == null)
        {
            yield break;
        }

        List<Vector2> directions = new List<Vector2>();
        for (int i = 0; i < phiKiemCount; i++)
        {
            Vector2 start = transform.position;
            Vector2 direction = ((Vector2)target.position - start).normalized;
            if (direction.sqrMagnitude < 0.0001f)
            {
                direction = Vector2.right;
            }

            directions.Add(direction);
            BossLineTelegraph2D.Create("PhiKiemLineWarning", start, start + direction * 18f, new Color(1f, 0.05f, 0.05f, 0.82f), phiKiemTelegraphTime, 0.16f, 10);
            yield return new WaitForSeconds(phiKiemStepDelay);
        }

        yield return new WaitForSeconds(phiKiemTelegraphTime);
        for (int i = 0; i < directions.Count; i++)
        {
            SpawnRectangleProjectile("PhiKiem", transform.position, directions[i], 1.35f, 0.42f, boss.BaseDamage * 0.75f, 1.4f, phiKiemSpeed, new Color(0.95f, 0.98f, 1f, 0.95f));
            yield return new WaitForSeconds(phiKiemStepDelay);
        }

        currentSkillName = "Sat Hach";
    }

    private IEnumerator DaoVanRoutine()
    {
        currentSkillName = "Dao Van";
        ResolveReferences();
        if (target == null)
        {
            yield break;
        }

        List<Vector2> positions = PickDaoVanPositions(target.position);
        for (int i = 0; i < positions.Count; i++)
        {
            BossSkillTelegraph2D.CreateCircle("DaoVanWarning", positions[i], daoVanRadius, new Color(1f, 0.05f, 0.05f, 0.48f), daoVanTelegraphTime, 8, true);
            StartCoroutine(DelayedDaoVanStrike(positions[i]));
        }

        yield return new WaitForSeconds(daoVanTelegraphTime + 0.2f);
        currentSkillName = "Sat Hach";
    }

    private IEnumerator DelayedDaoVanStrike(Vector2 position)
    {
        yield return new WaitForSeconds(daoVanTelegraphTime);
        SpawnCircleHazard("DaoVanStrike", position, daoVanRadius, boss.BaseDamage * 1.5f, 0.28f, new Color(0.82f, 0.18f, 1f, 0.78f), true, 0.2f);
    }

    private IEnumerator EndTrialRoutine()
    {
        state = TutorialBossState.KetThuc;
        currentSkillName = "Ket Thuc";
        if (attackLoop != null)
        {
            StopCoroutine(attackLoop);
        }

        GameObject lockOnEffect = CreateFinalLockOnEffect(target != null ? (Vector2)target.position : (Vector2)transform.position);
        float startTime = Time.time;
        float nextLineTime = 0f;
        while (Time.time - startTime < finalStrikeDelay)
        {
            ResolveReferences();
            Vector2 lockPosition = target != null ? (Vector2)target.position : (Vector2)transform.position;
            if (lockOnEffect != null)
            {
                lockOnEffect.transform.position = lockPosition;
                AnimateFinalLockOn(lockOnEffect.transform, startTime);
            }

            if (Time.time >= nextLineTime)
            {
                BossLineTelegraph2D.Create("FinalJudgementLockLine", transform.position, lockPosition, new Color(1f, 0.86f, 0.28f, 0.75f), finalLockLineInterval * 1.8f, 0.2f, 13);
                nextLineTime = Time.time + finalLockLineInterval;
            }

            yield return null;
        }

        ResolveReferences();
        Vector2 strikePosition = target != null ? (Vector2)target.position : (Vector2)transform.position;
        if (lockOnEffect != null)
        {
            lockOnEffect.transform.position = strikePosition;
        }

        CreateFinalImpactVisual(strikePosition);
        SpawnCircleHazard("ThaiThuongPhanQuyet", strikePosition, finalStrikeRadius, finalStrikeDamage, 0.6f, new Color(1f, 0.95f, 0.55f, 0.92f), false, 0.05f);
        if (lockOnEffect != null)
        {
            Destroy(lockOnEffect, 0.35f);
        }

        PlayerHealth2D playerHealth = target != null ? target.GetComponentInParent<PlayerHealth2D>() : null;
        if (playerHealth != null)
        {
            playerHealth.SetInvincible(false);
            playerHealth.TakeDamage(new DamageInfo(finalStrikeDamage, gameObject));
        }

        yield return new WaitForSeconds(0.35f);
        if (combatManager != null && combatManager.State == CombatManager.CombatState.Running)
        {
            combatManager.CompleteCombat(CombatResult.Defeat);
        }
    }

    private GameObject CreateFinalLockOnEffect(Vector2 position)
    {
        GameObject root = new GameObject("ThaiThuongPhanQuyetLockOn");
        root.transform.position = position;

        CreateEffectPart(root.transform, "OuterSeal", BossRuntimeSprites.Circle, Vector2.zero, new Vector2(finalStrikeRadius * 2f, finalStrikeRadius * 2f), new Color(1f, 0.78f, 0.12f, 0.28f), 11);
        CreateEffectPart(root.transform, "MiddleSeal", BossRuntimeSprites.Square, Vector2.zero, new Vector2(finalStrikeRadius * 1.15f, finalStrikeRadius * 1.15f), new Color(0.55f, 0.86f, 1f, 0.35f), 12);
        CreateEffectPart(root.transform, "InnerSeal", BossRuntimeSprites.Circle, Vector2.zero, new Vector2(finalStrikeRadius * 0.74f, finalStrikeRadius * 0.74f), new Color(1f, 0.95f, 0.55f, 0.42f), 13);
        CreateEffectPart(root.transform, "HeavenCore", BossRuntimeSprites.Circle, Vector2.zero, new Vector2(0.55f, 0.55f), new Color(1f, 1f, 0.82f, 0.95f), 14);

        for (int i = 0; i < 8; i++)
        {
            float angle = i * 45f;
            Vector2 offset = Quaternion.Euler(0f, 0f, angle) * Vector2.up * (finalStrikeRadius * 0.58f);
            SpriteRenderer sword = CreateEffectPart(root.transform, $"JudgementSword_{i}", BossRuntimeSprites.Square, offset, new Vector2(0.12f, 0.95f), new Color(0.86f, 0.96f, 1f, 0.86f), 14);
            sword.transform.localRotation = Quaternion.Euler(0f, 0f, angle);
        }

        return root;
    }

    private void AnimateFinalLockOn(Transform root, float startTime)
    {
        float elapsed = Time.time - startTime;
        float t = Mathf.Clamp01(elapsed / finalStrikeDelay);
        float pulse = 1f + Mathf.Sin(Time.time * 16f) * Mathf.Lerp(0.04f, 0.14f, t);
        root.localScale = Vector3.one * pulse;

        for (int i = 0; i < root.childCount; i++)
        {
            Transform child = root.GetChild(i);
            float direction = i % 2 == 0 ? 1f : -1f;
            child.Rotate(0f, 0f, direction * Mathf.Lerp(45f, 155f, t) * Time.deltaTime);
        }
    }

    private void CreateFinalImpactVisual(Vector2 position)
    {
        GameObject core = CreateVisualHazard("ThaiThuongImpactCore", position, BossRuntimeSprites.Circle, new Color(1f, 1f, 0.72f, 0.96f), new Vector2(finalStrikeRadius * 1.3f, finalStrikeRadius * 1.3f), 15);
        GameObject beam = CreateVisualHazard("ThaiThuongHeavenBeam", position + Vector2.up * 2.2f, BossRuntimeSprites.Square, new Color(1f, 0.92f, 0.5f, 0.86f), new Vector2(0.72f, 7.2f), 16);
        Destroy(core, 0.45f);
        Destroy(beam, 0.45f);
    }

    private SpriteRenderer CreateEffectPart(Transform parent, string partName, Sprite sprite, Vector2 localPosition, Vector2 localScale, Color color, int sortingOrder)
    {
        GameObject part = new GameObject(partName);
        part.transform.SetParent(parent, false);
        part.transform.localPosition = localPosition;
        part.transform.localScale = new Vector3(localScale.x, localScale.y, 1f);

        SpriteRenderer renderer = part.AddComponent<SpriteRenderer>();
        renderer.sprite = sprite;
        renderer.color = color;
        renderer.sortingOrder = sortingOrder;
        return renderer;
    }

    private void MoveFreely()
    {
        if (target == null)
        {
            return;
        }

        Vector2 position = transform.position;
        Vector2 toTarget = (Vector2)target.position - position;
        float distance = toTarget.magnitude;
        if (distance < 0.001f)
        {
            return;
        }

        Vector2 toward = toTarget / distance;
        Vector2 orbit = new Vector2(-toward.y, toward.x);
        float orbitSign = Mathf.Sin(Time.time * 0.45f) >= 0f ? 1f : -1f;
        orbit *= orbitSign;

        Vector2 desired = distance > preferredDistance
            ? Vector2.Lerp(toward, orbit, 0.38f).normalized
            : Vector2.Lerp(-toward, orbit, 0.72f).normalized;

        Vector2 nextPosition = position + desired * movementSpeed * Time.deltaTime;
        if (arenaClampRadius > 0f && nextPosition.magnitude > arenaClampRadius)
        {
            nextPosition = nextPosition.normalized * arenaClampRadius;
        }

        transform.position = nextPosition;
    }

    private List<Vector2> PickDaoVanPositions(Vector2 center)
    {
        List<Vector2> positions = new List<Vector2>();
        int attempts = 0;
        while (positions.Count < daoVanCount && attempts < 80)
        {
            attempts++;
            Vector2 candidate = center + Random.insideUnitCircle * daoVanSearchRadius;
            bool valid = true;
            for (int i = 0; i < positions.Count; i++)
            {
                if (Vector2.Distance(candidate, positions[i]) <= daoVanRadius * 2f)
                {
                    valid = false;
                    break;
                }
            }

            if (valid)
            {
                positions.Add(candidate);
            }
        }

        while (positions.Count < daoVanCount)
        {
            float angle = positions.Count * Mathf.PI * 2f / daoVanCount;
            positions.Add(center + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * daoVanSearchRadius * 0.65f);
        }

        return positions;
    }

    private void SpawnRectangleProjectile(string name, Vector2 start, Vector2 direction, float length, float width, float damage, float lifetime, float speed, Color color)
    {
        GameObject obj = CreateVisualHazard(name, start, BossRuntimeSprites.Square, color, new Vector2(length, width), 8);
        obj.transform.right = direction.sqrMagnitude > 0.0001f ? direction.normalized : Vector2.right;
        BoxCollider2D collider = obj.AddComponent<BoxCollider2D>();
        collider.size = Vector2.one;
        BossHazard2D hazard = obj.AddComponent<BossHazard2D>();
        hazard.Initialize(damage, lifetime, direction.normalized * speed, gameObject, true, 0.2f);
    }

    private void SpawnCircleHazard(string name, Vector2 position, float radius, float damage, float lifetime, Color color, bool damageOnce, float tickInterval)
    {
        GameObject obj = CreateVisualHazard(name, position, BossRuntimeSprites.Circle, color, new Vector2(radius * 2f, radius * 2f), 8);
        CircleCollider2D collider = obj.AddComponent<CircleCollider2D>();
        collider.radius = 0.5f;
        BossHazard2D hazard = obj.AddComponent<BossHazard2D>();
        hazard.Initialize(damage, lifetime, Vector2.zero, gameObject, damageOnce, tickInterval);
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

    private void EnsureDialogueStyle()
    {
        if (dialogueStyle != null)
        {
            return;
        }

        dialogueStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = 28,
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.MiddleCenter
        };
        dialogueStyle.normal.textColor = new Color(1f, 0.92f, 0.62f, 1f);
    }

    private void ResolveReferences()
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

        if (combatManager == null)
        {
            combatManager = FindComponentInScene<CombatManager>();
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
