using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public sealed class EnemyMovement : MonoBehaviour
{
    [SerializeField] private EnemyBase enemyBase;
    [SerializeField] private ArenaBounds arenaBounds;

    private Rigidbody2D body;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        body.gravityScale = 0f;
        ResolveReferences();
    }

    private void OnValidate()
    {
        Rigidbody2D enemyBody = GetComponent<Rigidbody2D>();
        if (enemyBody != null)
        {
            enemyBody.gravityScale = 0f;
        }
    }

    public void MoveTowards(Vector2 targetPosition, float moveSpeed, bool rotateToMoveDirection, float arenaPadding)
    {
        if (body == null)
        {
            body = GetComponent<Rigidbody2D>();
        }

        Vector2 direction = (targetPosition - body.position).normalized;
        Vector2 nextPosition = body.position + direction * moveSpeed * Time.fixedDeltaTime;
        ResolveReferences();

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

    private void ResolveReferences()
    {
        if (enemyBase == null)
        {
            enemyBase = GetComponent<EnemyBase>();
        }

        if (arenaBounds == null)
        {
            arenaBounds = ArenaBounds.Instance;
        }

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
