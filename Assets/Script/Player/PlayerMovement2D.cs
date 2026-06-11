using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public sealed class PlayerMovement2D : MonoBehaviour
{
    [SerializeField, Min(0f)] private float moveSpeed = 5f;
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private ArenaBounds arenaBounds;
    [SerializeField, Min(0f)] private float arenaPadding = 0.35f;
    [SerializeField] private PlayerStatus2D playerStatus;

    private Rigidbody2D body;
    private Animator animator;
    private Vector2 moveInput;

    public Vector2 MoveInput => moveInput;
    public Vector2 LastMoveDirection { get; private set; } = Vector2.right;
    public bool CanMove { get; set; } = true;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        body.gravityScale = 0f;
        body.freezeRotation = true;
        FindPlayerStatsIfNeeded();
        FindArenaBoundsIfNeeded();
        FindPlayerStatusIfNeeded();
    }

    private void Update()
    {
        Keyboard keyboard = Keyboard.current;
        if (keyboard == null)
        {
            moveInput = Vector2.zero;
            return;
        }

        float x = 0f;
        float y = 0f;

        if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed)
        {
            x -= 1f;
        }

        if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed)
        {
            x += 1f;
        }

        if (keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed)
        {
            y -= 1f;
        }

        if (keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed)
        {
            y += 1f;
        }

        moveInput = Vector2.ClampMagnitude(new Vector2(x, y), 1f);

        if (!CanMove)
            moveInput = Vector2.zero;

        if (moveInput.sqrMagnitude > 0.0001f)
        {
            LastMoveDirection = moveInput.normalized;
        }

        UpdateAnimator();
        UpdateSpriteFacing();
    }

    private void UpdateAnimator()
    {
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }

        if (animator != null && animator.runtimeAnimatorController != null)
        {
            animator.SetFloat("Speed", moveInput.magnitude);
        }
    }

    private void UpdateSpriteFacing()
    {
        if (moveInput.x < 0f)
        {
            transform.localScale = new Vector3(
                -Mathf.Abs(transform.localScale.x),
                transform.localScale.y,
                transform.localScale.z);
        }
        else if (moveInput.x > 0f)
        {
            transform.localScale = new Vector3(
                Mathf.Abs(transform.localScale.x),
                transform.localScale.y,
                transform.localScale.z);
        }
    }

    private void FixedUpdate()
    {
        FindPlayerStatusIfNeeded();

        if (!CanMove || (playerStatus != null && playerStatus.IsRooted))
        {
            return;
        }

        FindArenaBoundsIfNeeded();

        Vector2 nextPosition = body.position + moveInput * GetMoveSpeed() * Time.fixedDeltaTime;
        if (arenaBounds != null)
        {
            nextPosition = arenaBounds.ClampPosition(nextPosition, arenaPadding);
        }

        body.MovePosition(nextPosition);
    }

    private float GetMoveSpeed()
    {
        FindPlayerStatsIfNeeded();
        return playerStats != null ? playerStats.MoveSpeed : moveSpeed;
    }

    private void FindPlayerStatsIfNeeded()
    {
        if (playerStats == null)
        {
            playerStats = GetComponent<PlayerStats>();
        }
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

    private void FindPlayerStatusIfNeeded()
    {
        if (playerStatus == null)
        {
            playerStatus = GetComponent<PlayerStatus2D>();
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
