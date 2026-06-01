using UnityEngine;
using UnityEngine.InputSystem;

public sealed class PlayerCombat2D : MonoBehaviour
{
    [SerializeField] private PlayerAim2D playerAim;
    [SerializeField] private Projectile2D basicProjectilePrefab;
    [SerializeField, Min(0.01f)] private float basicFireCooldown = 0.25f;
    [SerializeField] private Projectile2D skillProjectilePrefab;
    [SerializeField, Min(0.01f)] private float skillCooldown = 3f;

    private float nextBasicFireTime;
    private float nextSkillTime;

    public float BasicFireCooldown => basicFireCooldown;
    public bool IsBasicAttackReady => Time.time >= nextBasicFireTime;
    public float BasicAttackCooldownRemaining => Mathf.Max(nextBasicFireTime - Time.time, 0f);
    public float SkillCooldown => skillCooldown;
    public bool IsSkillReady => Time.time >= nextSkillTime;
    public float SkillCooldownRemaining => Mathf.Max(nextSkillTime - Time.time, 0f);

    private void Awake()
    {
        if (playerAim == null)
        {
            playerAim = GetComponent<PlayerAim2D>();
        }
    }

    private void Update()
    {
        Mouse mouse = Mouse.current;
        if (mouse != null && mouse.leftButton.isPressed)
        {
            TryBasicAttack();
        }

        Keyboard keyboard = Keyboard.current;
        if (keyboard != null && keyboard.qKey.wasPressedThisFrame)
        {
            TrySkillAttack();
        }
    }

    private void TryBasicAttack()
    {
        if (!IsBasicAttackReady || basicProjectilePrefab == null)
        {
            return;
        }

        FireProjectile(basicProjectilePrefab, GetShootDirection());
        nextBasicFireTime = Time.time + basicFireCooldown;
    }

    private void TrySkillAttack()
    {
        if (!IsSkillReady || skillProjectilePrefab == null)
        {
            return;
        }

        FireProjectile(skillProjectilePrefab, GetShootDirection());
        nextSkillTime = Time.time + skillCooldown;
    }

    private void FireProjectile(Projectile2D projectilePrefab, Vector2 shootDirection)
    {
        Transform spawnPoint = playerAim != null && playerAim.FirePoint != null
            ? playerAim.FirePoint
            : transform;

        float angle = Mathf.Atan2(shootDirection.y, shootDirection.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.Euler(0f, 0f, angle);
        Projectile2D projectile = Instantiate(projectilePrefab, spawnPoint.position, rotation);
        projectile.Launch(shootDirection, transform.root);
    }

    private Vector2 GetShootDirection()
    {
        if (playerAim != null && playerAim.AimDirection.sqrMagnitude > 0.0001f)
        {
            return playerAim.AimDirection.normalized;
        }

        Vector2 fallbackDirection = transform.right;
        return fallbackDirection.sqrMagnitude > 0.0001f
            ? fallbackDirection.normalized
            : Vector2.right;
    }
}
