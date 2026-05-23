using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public sealed class EnemyChaseAI2D : MonoBehaviour
{
    [SerializeField, Min(0f)] private float moveSpeed = 3f;
    [SerializeField] private Transform target;
    [SerializeField] private bool rotateToMoveDirection = true;

    private Rigidbody2D body;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        body.gravityScale = 0f;
        body.freezeRotation = true;
    }

    private void Start()
    {
        FindTargetIfNeeded();
    }

    private void FixedUpdate()
    {
        FindTargetIfNeeded();

        if (target == null)
        {
            return;
        }

        Vector2 direction = ((Vector2)target.position - body.position).normalized;
        Vector2 nextPosition = body.position + direction * moveSpeed * Time.fixedDeltaTime;
        body.MovePosition(nextPosition);

        if (rotateToMoveDirection && direction.sqrMagnitude > 0.0001f)
        {
            transform.right = direction;
        }
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    private void FindTargetIfNeeded()
    {
        if (target != null)
        {
            return;
        }

        PlayerMovement2D playerMovement = FindFirstObjectByType<PlayerMovement2D>();
        if (playerMovement != null)
        {
            target = playerMovement.transform;
            return;
        }

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            target = playerObject.transform;
        }
    }
}
