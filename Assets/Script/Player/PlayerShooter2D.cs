using UnityEngine;
using UnityEngine.InputSystem;

public sealed class PlayerShooter2D : MonoBehaviour
{
    [SerializeField] private Camera aimCamera;
    [SerializeField] private Projectile2D projectilePrefab;
    [SerializeField] private Transform projectileSpawnPoint;
    [SerializeField, Min(0.01f)] private float fireRate = 5f;
    [SerializeField] private bool rotatePlayerToAim = true;

    private Vector2 aimDirection = Vector2.right;
    private float nextFireTime;

    private void Awake()
    {
        if (aimCamera == null)
        {
            aimCamera = Camera.main;
        }

        if (projectileSpawnPoint == null)
        {
            projectileSpawnPoint = transform;
        }
    }

    private void Update()
    {
        UpdateAimDirection();
        TryShoot();
    }

    private void UpdateAimDirection()
    {
        Mouse mouse = Mouse.current;
        if (mouse == null || aimCamera == null)
        {
            return;
        }

        Vector3 mouseScreenPosition = mouse.position.ReadValue();
        Vector3 mouseWorldPosition = aimCamera.ScreenToWorldPoint(mouseScreenPosition);
        Vector2 origin = projectileSpawnPoint.position;
        Vector2 direction = (Vector2)mouseWorldPosition - origin;

        if (direction.sqrMagnitude < 0.0001f)
        {
            return;
        }

        aimDirection = direction.normalized;

        if (rotatePlayerToAim)
        {
            transform.right = aimDirection;
        }
    }

    private void TryShoot()
    {
        Mouse mouse = Mouse.current;
        if (mouse == null || !mouse.leftButton.isPressed)
        {
            return;
        }

        if (Time.time < nextFireTime)
        {
            return;
        }

        Fire();
        nextFireTime = Time.time + 1f / fireRate;
    }

    private void Fire()
    {
        if (projectilePrefab == null || projectileSpawnPoint == null)
        {
            return;
        }

        float angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.Euler(0f, 0f, angle);
        Projectile2D projectile = Instantiate(projectilePrefab, projectileSpawnPoint.position, rotation);
        projectile.Launch(aimDirection, transform.root);
    }
}
