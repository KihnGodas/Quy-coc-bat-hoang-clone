using UnityEngine;

public struct DamageInfo
{
    public float amount;
    public GameObject source;
    public bool canCrit;
    public float critChance;
    public float critMultiplier;
    public Vector2 hitDirection;
    public bool wasCritical;

    public DamageInfo(
        float amount,
        GameObject source = null,
        bool canCrit = false,
        float critChance = 0f,
        float critMultiplier = 2f,
        Vector2 hitDirection = default)
    {
        this.amount = amount;
        this.source = source;
        this.canCrit = canCrit;
        this.critChance = critChance;
        this.critMultiplier = critMultiplier;
        this.hitDirection = hitDirection;
        wasCritical = false;
    }

    public float RollFinalAmount()
    {
        float finalAmount = Mathf.Max(amount, 0f);
        wasCritical = false;

        if (canCrit && finalAmount > 0f && critMultiplier > 1f && Random.value <= Mathf.Clamp01(critChance))
        {
            wasCritical = true;
            finalAmount *= critMultiplier;
        }

        return finalAmount;
    }
}
