using UnityEngine;

public enum EnemyClassification
{
    Square,
    Triangle
}

[CreateAssetMenu(menuName = "TKCC/Enemy Data", fileName = "NewEnemyData")]
public sealed class EnemyData : ScriptableObject
{
    [SerializeField] private Sprite sprite;
    [SerializeField] private EnemyClassification classification;
    [SerializeField, Min(1f)] private float maxHealth = 3f;
    [SerializeField, Min(0f)] private float moveSpeed = 3f;
    [SerializeField, Min(0f)] private float contactDamage = 1f;
    [SerializeField] private Color colorTint = Color.white;

    public Sprite Sprite => sprite;
    public EnemyClassification Classification => classification;
    public float MaxHealth => maxHealth;
    public float MoveSpeed => moveSpeed;
    public float ContactDamage => contactDamage;
    public Color ColorTint => colorTint;
}
