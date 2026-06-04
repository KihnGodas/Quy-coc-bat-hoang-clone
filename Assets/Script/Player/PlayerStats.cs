using UnityEngine;

public sealed class PlayerStats : MonoBehaviour
{
    [SerializeField, Min(1f)] private float maxHP = 200f;
    [SerializeField, Min(0f)] private float baseDamage = 50f;
    [SerializeField, Min(0f)] private float moveSpeed = 12f;
    [SerializeField, Min(0f)] private float healthRegen = 1f;

    public float MaxHP => maxHP;
    public float BaseDamage => baseDamage;
    public float MoveSpeed => moveSpeed;
    public float HealthRegen => healthRegen;
}
