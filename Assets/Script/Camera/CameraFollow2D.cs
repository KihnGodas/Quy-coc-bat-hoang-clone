using UnityEngine;

[RequireComponent(typeof(Camera))]
public sealed class CameraFollow2D : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private ArenaBounds arenaBounds;
    [SerializeField, Min(0f)] private float smoothTime = 0.12f;
    [SerializeField] private Vector2 followOffset;
    [SerializeField] private bool clampToArena = true;

    private Camera targetCamera;
    private Vector3 velocity;
    private float fixedZ;

    private void Awake()
    {
        targetCamera = GetComponent<Camera>();
        fixedZ = transform.position.z;
        ResolveReferences();
        SnapToTarget();
    }

    private void LateUpdate()
    {
        ResolveReferences();

        if (target == null)
        {
            return;
        }

        Vector3 desiredPosition = new Vector3(
            target.position.x + followOffset.x,
            target.position.y + followOffset.y,
            fixedZ);

        desiredPosition = ClampCameraPosition(desiredPosition);
        transform.position = smoothTime <= 0f
            ? desiredPosition
            : Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, smoothTime);
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
        SnapToTarget();
    }

    private void SnapToTarget()
    {
        if (target == null)
        {
            return;
        }

        Vector3 desiredPosition = new Vector3(
            target.position.x + followOffset.x,
            target.position.y + followOffset.y,
            fixedZ);

        transform.position = ClampCameraPosition(desiredPosition);
        velocity = Vector3.zero;
    }

    private Vector3 ClampCameraPosition(Vector3 desiredPosition)
    {
        if (!clampToArena || arenaBounds == null || targetCamera == null || !targetCamera.orthographic)
        {
            return desiredPosition;
        }

        if (!arenaBounds.TryGetWorldBounds(out float minX, out float maxX, out float minY, out float maxY))
        {
            return desiredPosition;
        }

        float verticalExtent = targetCamera.orthographicSize;
        float horizontalExtent = verticalExtent * targetCamera.aspect;

        float cameraMinX = minX + horizontalExtent;
        float cameraMaxX = maxX - horizontalExtent;
        float cameraMinY = minY + verticalExtent;
        float cameraMaxY = maxY - verticalExtent;

        if (cameraMinX > cameraMaxX)
        {
            float centerX = (minX + maxX) * 0.5f;
            cameraMinX = centerX;
            cameraMaxX = centerX;
        }

        if (cameraMinY > cameraMaxY)
        {
            float centerY = (minY + maxY) * 0.5f;
            cameraMinY = centerY;
            cameraMaxY = centerY;
        }

        return new Vector3(
            Mathf.Clamp(desiredPosition.x, cameraMinX, cameraMaxX),
            Mathf.Clamp(desiredPosition.y, cameraMinY, cameraMaxY),
            desiredPosition.z);
    }

    private void ResolveReferences()
    {
        if (target == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null)
            {
                target = playerObject.transform;
            }
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
