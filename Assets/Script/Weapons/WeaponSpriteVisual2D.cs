using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public sealed class WeaponSpriteVisual2D : MonoBehaviour
{
    [SerializeField] private SpriteRenderer weaponRenderer;
    [SerializeField] private Sprite fallbackSprite;
    [SerializeField] private Sprite swordSprite;
    [SerializeField] private Sprite spearSprite;
    [SerializeField] private Sprite axeSprite;
    [SerializeField] private Sprite flyingSwordSprite;
    [SerializeField] private SpriteRenderer secondaryPartRenderer;
    [SerializeField] private SpriteRenderer accentPartRenderer;
    [SerializeField, Min(0f)] private float holdDistance = 0.72f;
    [SerializeField, Min(0f)] private float idleBobAmplitude = 0.08f;
    [SerializeField, Min(0f)] private float idleBobFrequency = 4.5f;
    [SerializeField, Min(0f)] private float idleSwayAngle = 7f;
    [SerializeField, Min(0.01f)] private float normalAttackDuration = 0.16f;
    [SerializeField, Min(0.01f)] private float skillAttackDuration = 0.24f;
    [SerializeField] private int sortingOrder = 7;
    [SerializeField] private Color swordColor = new Color(0.75f, 0.95f, 1f, 1f);
    [SerializeField] private Color spearColor = new Color(1f, 0.9f, 0.45f, 1f);
    [SerializeField] private Color axeColor = new Color(1f, 0.45f, 0.2f, 1f);
    [SerializeField] private Color flyingSwordColor = new Color(0.55f, 0.85f, 1f, 1f);
    [SerializeField] private Color handleColor = new Color(0.52f, 0.34f, 0.18f, 1f);
    [SerializeField] private Color metalAccentColor = new Color(0.95f, 0.98f, 1f, 1f);

    private Coroutine attackRoutine;
    private Transform owner;
    private WeaponType currentWeapon = WeaponType.FlyingSword;
    private Vector2 aimDirection = Vector2.right;
    private bool isAnimating;

    private void Awake()
    {
        ResolveReferences();
        ApplyWeaponVisual(currentWeapon);
    }

    private void LateUpdate()
    {
        if (!isAnimating)
        {
            ApplyPose(aimDirection, 0f, 0f, 1f, 1f);
        }
    }

    public void SetWeapon(WeaponType weaponType)
    {
        if (currentWeapon == weaponType && weaponRenderer != null && weaponRenderer.sprite != null)
        {
            return;
        }

        currentWeapon = weaponType;
        ApplyWeaponVisual(currentWeapon);
    }

    public void SetAimDirection(Vector2 direction)
    {
        if (direction.sqrMagnitude > 0.0001f)
        {
            aimDirection = direction.normalized;
        }
    }

    public void PlayAttack(WeaponType weaponType, Vector2 direction, bool isSkill)
    {
        ResolveReferences();
        SetWeapon(weaponType);
        SetAimDirection(direction);

        if (attackRoutine != null)
        {
            StopCoroutine(attackRoutine);
        }

        attackRoutine = StartCoroutine(AnimateAttack(isSkill));
    }

    private IEnumerator AnimateAttack(bool isSkill)
    {
        isAnimating = true;
        float duration = isSkill ? skillAttackDuration : normalAttackDuration;
        float startTime = Time.time;

        while (Time.time - startTime < duration)
        {
            float progress = Mathf.Clamp01((Time.time - startTime) / duration);
            float eased = Mathf.Sin(progress * Mathf.PI);
            float extension = Mathf.Lerp(0.05f, GetAttackExtension(isSkill), eased);
            float angleOffset = GetAttackAngleOffset(progress, isSkill);
            float scaleMultiplier = 1f + eased * GetAttackScaleBoost(isSkill);
            ApplyPose(aimDirection, extension, angleOffset, scaleMultiplier, 1f);
            yield return null;
        }

        isAnimating = false;
        ApplyPose(aimDirection, 0f, 0f, 1f, 1f);
        attackRoutine = null;
    }

    private void ApplyWeaponVisual(WeaponType weaponType)
    {
        ResolveReferences();

        weaponRenderer.sprite = GetSprite(weaponType);
        weaponRenderer.color = GetMainColor(weaponType);
        weaponRenderer.sortingOrder = sortingOrder;
        transform.localScale = GetScale(weaponType);
        ConfigurePrototypeParts(weaponType);
    }

    private void ApplyPose(Vector2 direction, float extension, float angleOffset, float scaleMultiplier, float alpha)
    {
        ResolveReferences();
        direction = direction.sqrMagnitude > 0.0001f ? direction.normalized : Vector2.right;

        Vector3 basePosition = owner != null ? owner.position : transform.position;
        Vector2 side = Vector2.Perpendicular(direction);
        float idleBob = !isAnimating ? Mathf.Sin(Time.time * idleBobFrequency) * idleBobAmplitude : 0f;
        transform.position = basePosition + (Vector3)(direction * (holdDistance + extension) + side * idleBob);

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        float idleAngle = !isAnimating ? Mathf.Sin(Time.time * idleBobFrequency * 0.72f) * idleSwayAngle : 0f;
        transform.rotation = Quaternion.Euler(0f, 0f, angle + angleOffset + idleAngle);
        transform.localScale = GetScale(currentWeapon) * scaleMultiplier;

        Color color = GetMainColor(currentWeapon);
        color.a = alpha;
        weaponRenderer.color = color;
        ApplyRendererAlpha(secondaryPartRenderer, alpha);
        ApplyRendererAlpha(accentPartRenderer, alpha);
    }

    private float GetAttackAngleOffset(float progress, bool isSkill)
    {
        if (currentWeapon == WeaponType.Spear || currentWeapon == WeaponType.FlyingSword)
        {
            return Mathf.Sin(progress * Mathf.PI * 2f) * (isSkill ? 8f : 4f);
        }

        float swingRange = currentWeapon == WeaponType.Axe ? 115f : 85f;
        if (isSkill)
        {
            swingRange += 35f;
        }

        return Mathf.Lerp(-swingRange * 0.5f, swingRange * 0.5f, progress);
    }

    private float GetAttackExtension(bool isSkill)
    {
        float baseExtension = currentWeapon switch
        {
            WeaponType.Spear => 0.62f,
            WeaponType.Axe => 0.3f,
            WeaponType.FlyingSword => 0.42f,
            _ => 0.34f
        };

        return isSkill ? baseExtension * 1.45f : baseExtension;
    }

    private float GetAttackScaleBoost(bool isSkill)
    {
        float baseBoost = currentWeapon switch
        {
            WeaponType.Axe => 0.32f,
            WeaponType.Spear => 0.22f,
            WeaponType.FlyingSword => 0.26f,
            _ => 0.24f
        };

        return isSkill ? baseBoost + 0.18f : baseBoost;
    }

    private Sprite GetSprite(WeaponType weaponType)
    {
        Sprite selected = GetCustomSprite(weaponType);
        if (selected != null)
        {
            return selected;
        }

        Sprite baseSprite = GetBaseSprite();
        return baseSprite != null ? baseSprite : weaponRenderer != null ? weaponRenderer.sprite : null;
    }

    private Sprite GetCustomSprite(WeaponType weaponType)
    {
        Sprite selected = weaponType switch
        {
            WeaponType.Sword => swordSprite,
            WeaponType.Spear => spearSprite,
            WeaponType.Axe => axeSprite,
            WeaponType.FlyingSword => flyingSwordSprite,
            _ => null
        };

        return selected;
    }

    private Sprite GetBaseSprite()
    {
        return fallbackSprite != null ? fallbackSprite : weaponRenderer != null ? weaponRenderer.sprite : null;
    }

    private Color GetMainColor(WeaponType weaponType)
    {
        return weaponType switch
        {
            WeaponType.Sword => swordColor,
            WeaponType.Spear => spearColor,
            WeaponType.Axe => handleColor,
            WeaponType.FlyingSword => flyingSwordColor,
            _ => Color.white
        };
    }

    private Vector3 GetScale(WeaponType weaponType)
    {
        return weaponType switch
        {
            WeaponType.Sword => new Vector3(0.82f, 0.14f, 1f),
            WeaponType.Spear => new Vector3(1.65f, 0.07f, 1f),
            WeaponType.Axe => new Vector3(0.62f, 0.13f, 1f),
            WeaponType.FlyingSword => new Vector3(0.58f, 0.08f, 1f),
            _ => Vector3.one
        };
    }

    private void ConfigurePrototypeParts(WeaponType weaponType)
    {
        Sprite partSprite = GetBaseSprite();
        bool usePrototypeParts = partSprite != null && GetCustomSprite(weaponType) == null;
        SetPartActive(secondaryPartRenderer, usePrototypeParts);
        SetPartActive(accentPartRenderer, usePrototypeParts);

        if (!usePrototypeParts)
        {
            return;
        }

        secondaryPartRenderer.sprite = partSprite;
        accentPartRenderer.sprite = partSprite;
        secondaryPartRenderer.sortingOrder = sortingOrder + 1;
        accentPartRenderer.sortingOrder = sortingOrder + 2;

        switch (weaponType)
        {
            case WeaponType.Sword:
                ConfigurePart(secondaryPartRenderer, new Vector2(-0.34f, 0f), new Vector2(0.16f, 1.75f), 0f, metalAccentColor);
                ConfigurePart(accentPartRenderer, new Vector2(0.52f, 0f), new Vector2(0.12f, 0.85f), 0f, Color.white);
                break;
            case WeaponType.Spear:
                ConfigurePart(secondaryPartRenderer, new Vector2(0.58f, 0f), new Vector2(0.14f, 4.4f), 45f, metalAccentColor);
                ConfigurePart(accentPartRenderer, new Vector2(-0.58f, 0f), new Vector2(0.08f, 2.4f), 0f, handleColor);
                break;
            case WeaponType.Axe:
                ConfigurePart(secondaryPartRenderer, new Vector2(0.52f, 0.24f), new Vector2(0.5f, 3.1f), 0f, axeColor);
                ConfigurePart(accentPartRenderer, new Vector2(0.52f, -0.24f), new Vector2(0.5f, 3.1f), 0f, axeColor);
                break;
            case WeaponType.FlyingSword:
                ConfigurePart(secondaryPartRenderer, new Vector2(0.42f, 0f), new Vector2(0.12f, 1.9f), 45f, metalAccentColor);
                ConfigurePart(accentPartRenderer, new Vector2(-0.16f, 0f), new Vector2(0.55f, 1.8f), 0f, new Color(0.3f, 0.75f, 1f, 0.55f));
                break;
        }
    }

    private void ConfigurePart(SpriteRenderer renderer, Vector2 localPosition, Vector2 localScale, float localRotationZ, Color color)
    {
        renderer.transform.localPosition = localPosition;
        renderer.transform.localRotation = Quaternion.Euler(0f, 0f, localRotationZ);
        renderer.transform.localScale = new Vector3(localScale.x, localScale.y, 1f);
        renderer.color = color;
    }

    private void ApplyRendererAlpha(SpriteRenderer renderer, float alpha)
    {
        if (renderer == null || !renderer.gameObject.activeSelf)
        {
            return;
        }

        Color color = renderer.color;
        color.a = alpha;
        renderer.color = color;
    }

    private void SetPartActive(SpriteRenderer renderer, bool active)
    {
        if (renderer != null && renderer.gameObject.activeSelf != active)
        {
            renderer.gameObject.SetActive(active);
        }
    }

    private void ResolveReferences()
    {
        if (weaponRenderer == null)
        {
            weaponRenderer = GetComponent<SpriteRenderer>();
        }

        if (secondaryPartRenderer == null)
        {
            secondaryPartRenderer = CreatePartRenderer("SecondaryWeaponPart", sortingOrder + 1);
        }

        if (accentPartRenderer == null)
        {
            accentPartRenderer = CreatePartRenderer("AccentWeaponPart", sortingOrder + 2);
        }

        if (owner == null && transform.parent != null)
        {
            owner = transform.parent;
        }
    }

    private SpriteRenderer CreatePartRenderer(string partName, int partSortingOrder)
    {
        GameObject partObject = new GameObject(partName);
        partObject.transform.SetParent(transform, false);

        SpriteRenderer renderer = partObject.AddComponent<SpriteRenderer>();
        renderer.sortingOrder = partSortingOrder;
        renderer.material = weaponRenderer != null ? weaponRenderer.sharedMaterial : null;
        return renderer;
    }
}
