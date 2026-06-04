using UnityEngine;
using UnityEngine.InputSystem;

public sealed class PlayerAim2D : MonoBehaviour
{
    [SerializeField] private Camera aimCamera;
    [SerializeField] private Transform firePoint;
    [SerializeField, Min(0f)] private float firePointDistance = 1f;

    public Vector2 AimDirection { get; private set; } = Vector2.right;
    public Vector3 MouseWorldPosition { get; private set; }
    public Transform FirePoint => firePoint;

    private void Awake()
    {
        if (aimCamera == null)
        {
            aimCamera = Camera.main;
        }
    }

    private void Update()
    {
        UpdateAim();
    }

    private void UpdateAim()
    {
        if (aimCamera == null || Mouse.current == null)
        {
            return;
        }

        Vector3 mouseScreenPosition = Mouse.current.position.ReadValue();
        mouseScreenPosition.z = Mathf.Abs(aimCamera.transform.position.z - transform.position.z);
        MouseWorldPosition = aimCamera.ScreenToWorldPoint(mouseScreenPosition);

        Vector2 direction = (Vector2)(MouseWorldPosition - transform.position);
        if (direction.sqrMagnitude < 0.0001f)
        {
            return;
        }

        AimDirection = direction.normalized;
        UpdateFirePoint();
    }

    private void UpdateFirePoint()
    {
        if (firePoint == null)
        {
            return;
        }

        firePoint.position = transform.position + (Vector3)(AimDirection * firePointDistance);
        firePoint.right = AimDirection;
    }
}
