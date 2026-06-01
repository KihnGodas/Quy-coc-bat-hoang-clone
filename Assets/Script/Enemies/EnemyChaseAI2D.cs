using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public sealed class EnemyChaseAI2D : MonoBehaviour
{
    [SerializeField, Min(0f)] private float moveSpeed = 2.5f;
    [SerializeField] private Transform target;
    [SerializeField] private bool rotateToMoveDirection;

    private Rigidbody2D body;

    public Transform Target => target;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        body.gravityScale = 0f;
        body.freezeRotation = !rotateToMoveDirection;

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
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            body.MoveRotation(angle);
        }
    }

    private void OnValidate()
    {
        Rigidbody2D enemyBody = GetComponent<Rigidbody2D>();
        if (enemyBody == null)
        {
            return;
        }

        enemyBody.gravityScale = 0f;
        enemyBody.freezeRotation = !rotateToMoveDirection;
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

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            target = playerObject.transform;
        }
    }
}
