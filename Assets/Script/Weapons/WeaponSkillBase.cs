using UnityEngine;

public abstract class WeaponSkillBase : MonoBehaviour
{
    [SerializeField, Min(0.01f)] private float cooldown = 1f;

    private float nextReadyTime;

    public bool IsReady => Time.time >= nextReadyTime;
    public float CooldownRemaining => Mathf.Max(nextReadyTime - Time.time, 0f);

    protected void StartCooldown()
    {
        nextReadyTime = Time.time + cooldown;
    }

    public abstract void Use(WeaponController controller, WeaponData weaponData);
}
