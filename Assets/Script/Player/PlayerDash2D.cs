using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public sealed class PlayerDash2D : MonoBehaviour
{
    [SerializeField] private PlayerMovement2D playerMovement;
    [SerializeField] private PlayerHealth2D playerHealth;
    [SerializeField, Min(0f)] private float dashSpeed = 12f;
    [SerializeField, Min(0.01f)] private float dashDuration = 0.15f;
    [SerializeField, Min(0f)] private float dashCooldown = 1f;
    [SerializeField] private string playerLayerName = "Player";
    [SerializeField] private string enemyLayerName = "Enemy";

    private Rigidbody2D body;
    private Vector2 dashDirection = Vector2.right;
    private float dashEndTime;
    private float nextDashTime;
    private bool isDashing;
    private bool hasStoredLayerCollision;
    private bool previousPlayerEnemyCollisionIgnored;

    public bool IsDashing => isDashing;
    public bool IsDashReady => Time.time >= nextDashTime && !isDashing;
    public float DashCooldownRemaining => Mathf.Max(nextDashTime - Time.time, 0f);

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
    }

    private void Update()
    {
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

        Vector2 nextPosition = body.position + dashDirection * dashSpeed * Time.fixedDeltaTime;
        body.MovePosition(nextPosition);
    }

    private void TryStartDash()
    {
        if (!IsDashReady)
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

        if (playerHealth != null)
        {
            playerHealth.SetInvincible(false);
        }

        RestorePlayerEnemyCollision();
    }

    private void OnDisable()
    {
        if (isDashing)
        {
            StopDash();
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
}
