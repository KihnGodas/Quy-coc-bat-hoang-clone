using System.Collections.Generic;
using UnityEngine;

public sealed class EnemySpawner : MonoBehaviour
{
    [SerializeField] private EnemyChaseAI2D enemyPrefab;
    [SerializeField] private Transform player;
    [SerializeField, Min(0.1f)] private float spawnInterval = 2f;
    [SerializeField, Min(1)] private int maxAliveEnemies = 10;
    [SerializeField, Min(0.1f)] private float spawnRadius = 8f;
    [SerializeField, Min(0f)] private float minDistanceFromPlayer = 2f;
    [SerializeField] private bool spawnAroundPlayer = true;

    private float nextSpawnTime;
    private readonly List<EnemyChaseAI2D> spawnedEnemies = new();

    private void Start()
    {
        FindPlayerIfNeeded();
        nextSpawnTime = Time.time + spawnInterval;
    }

    private void Update()
    {
        FindPlayerIfNeeded();

        if (Time.time < nextSpawnTime)
        {
            return;
        }

        nextSpawnTime = Time.time + spawnInterval;

        CleanupDeadEnemies();

        if (enemyPrefab == null || spawnedEnemies.Count >= maxAliveEnemies)
        {
            return;
        }

        SpawnEnemy();
    }

    private void SpawnEnemy()
    {
        Vector2 center = GetSpawnCenter();
        Vector2 spawnPosition = GetSpawnPosition(center);
        EnemyChaseAI2D enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);

        if (player != null)
        {
            enemy.SetTarget(player);
        }

        spawnedEnemies.Add(enemy);
    }

    private Vector2 GetSpawnCenter()
    {
        if (spawnAroundPlayer && player != null)
        {
            return player.position;
        }

        return transform.position;
    }

    private Vector2 GetSpawnPosition(Vector2 center)
    {
        float safeMinDistance = player != null
            ? Mathf.Min(minDistanceFromPlayer, spawnRadius)
            : 0f;

        float distance = Random.Range(safeMinDistance, spawnRadius);
        Vector2 direction = Random.insideUnitCircle.normalized;

        if (direction.sqrMagnitude < 0.0001f)
        {
            direction = Vector2.right;
        }

        return center + direction * distance;
    }

    private void CleanupDeadEnemies()
    {
        for (int i = spawnedEnemies.Count - 1; i >= 0; i--)
        {
            if (spawnedEnemies[i] == null)
            {
                spawnedEnemies.RemoveAt(i);
            }
        }
    }

    private void FindPlayerIfNeeded()
    {
        if (player != null)
        {
            return;
        }

        PlayerMovement2D playerMovement = FindFirstObjectByType<PlayerMovement2D>();
        if (playerMovement != null)
        {
            player = playerMovement.transform;
            return;
        }

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 center = Application.isPlaying ? GetSpawnCenter() : transform.position;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(center, spawnRadius);

        if (minDistanceFromPlayer > 0f)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(center, minDistanceFromPlayer);
        }
    }
}
