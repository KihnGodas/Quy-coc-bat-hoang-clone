using UnityEngine;

public sealed class MuzzleFlash2D : MonoBehaviour
{
    [SerializeField] private PlayerAim2D playerAim;
    [SerializeField] private Sprite fallbackSprite;
    [SerializeField] private Color flashColor = new Color(1f, 0.92f, 0.35f, 0.9f);
    [SerializeField, Min(0.01f)] private float flashLifetime = 0.08f;
    [SerializeField, Min(0.1f)] private float flashScaleMultiplier = 1.6f;
    [SerializeField] private int sortingOrder = 5;

    private void Awake()
    {
        if (playerAim == null)
        {
            playerAim = GetComponent<PlayerAim2D>();
        }
    }

    public void PlayFlash(Transform firePoint, Vector2 shootDirection)
    {
        if (firePoint == null)
        {
            return;
        }

        Sprite sprite = fallbackSprite;
        SpriteRenderer firePointRenderer = firePoint.GetComponent<SpriteRenderer>();
        if (sprite == null && firePointRenderer != null)
        {
            sprite = firePointRenderer.sprite;
        }

        if (sprite == null)
        {
            return;
        }

        float angle = shootDirection.sqrMagnitude > 0.0001f
            ? Mathf.Atan2(shootDirection.y, shootDirection.x) * Mathf.Rad2Deg
            : firePoint.eulerAngles.z;

        Vector3 scale = firePoint.lossyScale * flashScaleMultiplier;
        TemporaryEffect2D.CreateSpriteEffect(
            "MuzzleFlash",
            sprite,
            firePoint.position,
            Quaternion.Euler(0f, 0f, angle),
            scale,
            flashColor,
            sortingOrder,
            flashLifetime);
    }
}
