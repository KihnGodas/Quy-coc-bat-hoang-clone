using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public sealed class EnemyChaseAI2D : MonoBehaviour
{
    [SerializeField, Min(0f)] private float moveSpeed = 2.5f;
    [SerializeField] private Transform target;
    [SerializeField] private bool rotateToMoveDirection;
    [SerializeField] private ArenaBounds arenaBounds;
    [SerializeField, Min(0f)] private float arenaPadding = 0.35f;

    private Rigidbody2D body;

    public Transform Target => target;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        body.gravityScale = 0f;
        body.freezeRotation = !rotateToMoveDirection;

        FindTargetIfNeeded();
        FindArenaBoundsIfNeeded();
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
        FindArenaBoundsIfNeeded();

        if (arenaBounds != null)
        {
            nextPosition = arenaBounds.ClampPosition(nextPosition, arenaPadding);
        }

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
