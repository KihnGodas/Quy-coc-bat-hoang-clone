using System.Collections.Generic;
using UnityEngine;

public sealed class EnemySpawner : MonoBehaviour
{
    [SerializeField] private EnemyData[] enemyTypes;
    [SerializeField] private Transform player;
    [SerializeField, Min(0.1f)] private float spawnInterval = 2f;
    [SerializeField, Min(1)] private int maxAliveEnemies = 10;
    [SerializeField, Min(0.1f)] private float spawnRadius = 8f;
    [SerializeField, Min(0f)] private float minDistanceFromPlayer = 2f;
    [SerializeField] private bool spawnAroundPlayer = true;
    public bool isSpawning = true;

    private float nextSpawnTime;
    private readonly List<Enemy> spawnedEnemies = new();

    private void Start()
    {
        FindPlayerIfNeeded();
        nextSpawnTime = Time.time + spawnInterval;
    }

    private void Update()
    {
        if (!isSpawning) return;

        FindPlayerIfNeeded();

        if (Time.time < nextSpawnTime)
        {
            return;
        }

        nextSpawnTime = Time.time + spawnInterval;

        CleanupDeadEnemies();

        if (spawnedEnemies.Count >= maxAliveEnemies)
        {
            return;
        }

        SpawnEnemy();
    }

    private void SpawnEnemy()
    {
        Vector2 center = GetSpawnCenter();
        Vector2 spawnPosition = GetSpawnPosition(center);

        GameObject go = new GameObject("Enemy");
        go.transform.position = spawnPosition;
        go.transform.localScale = Vector3.one * 0.8f;

        SpriteRenderer renderer = go.AddComponent<SpriteRenderer>();
        renderer.sortingOrder = 1;

        Rigidbody2D body = go.AddComponent<Rigidbody2D>();
        body.gravityScale = 0f;
        body.freezeRotation = true;

        CircleCollider2D collider = go.AddComponent<CircleCollider2D>();
        collider.radius = 0.5f;

        go.AddComponent<SimpleHealth>();

        EnemyChaseAI2D ai = go.AddComponent<EnemyChaseAI2D>();
        if (player != null)
        {
            ai.SetTarget(player);
        }

        Enemy enemyComponent = go.AddComponent<Enemy>();
        if (enemyTypes != null && enemyTypes.Length > 0)
        {
            EnemyData randomData = enemyTypes[Random.Range(0, enemyTypes.Length)];
            enemyComponent.SetData(randomData);
        }

        spawnedEnemies.Add(enemyComponent);
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
