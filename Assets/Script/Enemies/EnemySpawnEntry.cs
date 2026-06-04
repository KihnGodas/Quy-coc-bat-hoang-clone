using UnityEngine;

[System.Serializable]
public sealed class EnemySpawnEntry
{
    [SerializeField] private EnemyData enemyData;
    [SerializeField, Min(0f)] private float unlockTime;
    [SerializeField, Min(0f)] private float weight = 1f;

    public EnemyData EnemyData => enemyData;
    public float UnlockTime => unlockTime;
    public float Weight => weight;

    public bool CanSpawn(float elapsedTime)
    {
        return enemyData != null && weight > 0f && elapsedTime >= unlockTime;
    }
}
