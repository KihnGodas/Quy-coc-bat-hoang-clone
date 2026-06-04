using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public sealed class PlayerDash2D : MonoBehaviour
{
    [SerializeField] private PlayerMovement2D playerMovement;
    [SerializeField] private PlayerHealth2D playerHealth;
    [SerializeField] private PlayerStatus2D playerStatus;
    [SerializeField, Min(0f)] private float dashDistance = 4.5f;
    [SerializeField, Min(0f)] private float dashSpeed = 12f;
    [SerializeField, Min(0.01f)] private float dashDuration = 0.15f;
    [SerializeField, Min(0f)] private float dashCooldown = 1f;
    [SerializeField, Min(0f)] private float invulnerableDuration = 0.2f;
    [SerializeField] private string playerLayerName = "Player";
    [SerializeField] private string enemyLayerName = "Enemy";
    [SerializeField] private ArenaBounds arenaBounds;
    [SerializeField, Min(0f)] private float arenaPadding = 0.35f;

    private Rigidbody2D body;
    private Vector2 dashDirection = Vector2.right;
    private float dashEndTime;
    private float nextDashTime;
    private bool isDashing;
    private bool dashInvulnerabilityActive;
    private float invulnerableEndTime;
    private bool hasStoredLayerCollision;
    private bool previousPlayerEnemyCollisionIgnored;

    public bool IsDashing => isDashing;
    public bool IsDashReady => Time.time >= nextDashTime && !isDashing;
    public float DashCooldownRemaining => Mathf.Max(nextDashTime - Time.time, 0f);
    public float DashCooldownTotal => dashCooldown;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();

        if (playerMovement == null)
        {
            playerMovement = GetComponent<PlayerMovement2D>();
        }

        if (playerHealth == null)
        {
            playerHealth = GetComponent<PlayerHealth2D>();
        }

        if (playerStatus == null)
        {
            playerStatus = GetComponent<PlayerStatus2D>();
        }

        FindArenaBoundsIfNeeded();
    }

    private void Update()
    {
        UpdateDashInvulnerability();

        Keyboard keyboard = Keyboard.current;
        if (keyboard == null || !keyboard.spaceKey.wasPressedThisFrame)
        {
            return;
        }

        TryStartDash();
    }

    private void FixedUpdate()
    {
        if (!isDashing)
        {
            return;
        }

        if (Time.time >= dashEndTime)
        {
            StopDash();
            return;
        }

        FindArenaBoundsIfNeeded();

        Vector2 nextPosition = body.position + dashDirection * GetDashSpeed() * Time.fixedDeltaTime;
        if (arenaBounds != null)
        {
            nextPosition = arenaBounds.ClampPosition(nextPosition, arenaPadding);
        }

        body.MovePosition(nextPosition);
    }

    private void TryStartDash()
    {
        if (!IsDashReady)
        {
            return;
        }

        if (playerStatus == null)
        {
            playerStatus = GetComponent<PlayerStatus2D>();
        }

        if (playerStatus != null && playerStatus.IsRooted)
        {
            return;
        }

        dashDirection = GetDashDirection();
        isDashing = true;
        dashEndTime = Time.time + dashDuration;
        nextDashTime = Time.time + dashCooldown;

        if (playerMovement != null)
        {
            playerMovement.CanMove = false;
        }

        if (playerHealth != null)
        {
            playerHealth.SetInvincible(true);
            dashInvulnerabilityActive = true;
            invulnerableEndTime = Time.time + invulnerableDuration;
        }

        SetPlayerEnemyCollisionIgnored(true);
    }

    private Vector2 GetDashDirection()
    {
        if (playerMovement == null)
        {
            return dashDirection.sqrMagnitude > 0.0001f ? dashDirection.normalized : Vector2.right;
        }

        if (playerMovement.MoveInput.sqrMagnitude > 0.0001f)
        {
            return playerMovement.MoveInput.normalized;
        }

        if (playerMovement.LastMoveDirection.sqrMagnitude > 0.0001f)
        {
            return playerMovement.LastMoveDirection.normalized;
        }

        return Vector2.right;
    }

    private void StopDash()
    {
        isDashing = false;

        if (playerMovement != null)
        {
            playerMovement.CanMove = true;
        }

        if (playerHealth != null && Time.time >= invulnerableEndTime)
        {
            playerHealth.SetInvincible(false);
            dashInvulnerabilityActive = false;
        }

        RestorePlayerEnemyCollision();
    }

    private void OnDisable()
    {
        if (isDashing)
        {
            StopDash();
        }

        if (playerHealth != null && dashInvulnerabilityActive)
        {
            playerHealth.SetInvincible(false);
            dashInvulnerabilityActive = false;
        }
    }

    private void SetPlayerEnemyCollisionIgnored(bool ignored)
    {
        int playerLayer = LayerMask.NameToLayer(playerLayerName);
        int enemyLayer = LayerMask.NameToLayer(enemyLayerName);

        if (playerLayer < 0 || enemyLayer < 0)
        {
            return;
        }

        if (!hasStoredLayerCollision)
        {
            previousPlayerEnemyCollisionIgnored = Physics2D.GetIgnoreLayerCollision(playerLayer, enemyLayer);
            hasStoredLayerCollision = true;
        }

        Physics2D.IgnoreLayerCollision(playerLayer, enemyLayer, ignored);
    }

    private void RestorePlayerEnemyCollision()
    {
        if (!hasStoredLayerCollision)
        {
            return;
        }

        int playerLayer = LayerMask.NameToLayer(playerLayerName);
        int enemyLayer = LayerMask.NameToLayer(enemyLayerName);

        if (playerLayer >= 0 && enemyLayer >= 0)
        {
            Physics2D.IgnoreLayerCollision(playerLayer, enemyLayer, previousPlayerEnemyCollisionIgnored);
        }

        hasStoredLayerCollision = false;
    }

    private float GetDashSpeed()
    {
        if (dashDistance > 0f && dashDuration > 0f)
        {
            return dashDistance / dashDuration;
        }

        return dashSpeed;
    }

    private void UpdateDashInvulnerability()
    {
        if (!dashInvulnerabilityActive || playerHealth == null || Time.time < invulnerableEndTime)
        {
            return;
        }

        playerHealth.SetInvincible(false);
        dashInvulnerabilityActive = false;
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
