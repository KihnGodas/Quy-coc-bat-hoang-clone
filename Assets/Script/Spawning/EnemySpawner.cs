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
    [SerializeField] private Collider2D leftWall;
    [SerializeField] private Collider2D rightWall;
    [SerializeField] private Collider2D topWall;
    [SerializeField] private Collider2D bottomWall;
    [SerializeField] private bool spawnFromArenaEdges = true;
    [SerializeField, Min(0f)] private float edgeSpawnPadding = 1f;
    [SerializeField, Min(1)] private int spawnPositionAttempts = 20;
    [SerializeField, Min(0f)] private float wallPadding = 0.5f;

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
        for (int i = 0; i < spawnPositionAttempts; i++)
        {
            Vector2 candidate = spawnFromArenaEdges
                ? GetRandomEdgeSpawnPosition(center)
                : GetRandomSpawnPosition(center);

            if (IsInsideWallBounds(candidate))
            {
                return candidate;
            }
        }

        Vector2 fallback = spawnFromArenaEdges
            ? GetRandomEdgeSpawnPosition(center)
            : GetRandomSpawnPosition(center);

        return ClampToWallBounds(fallback);
    }

    private Vector2 GetRandomSpawnPosition(Vector2 center)
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

    private Vector2 GetRandomEdgeSpawnPosition(Vector2 center)
    {
        if (!TryGetWallBounds(out float minX, out float maxX, out float minY, out float maxY))
        {
            return GetRandomSpawnPosition(center);
        }

        for (int i = 0; i < spawnPositionAttempts; i++)
        {
            Vector2 candidate = GetRandomPointOnArenaEdge(minX, maxX, minY, maxY);
            if (IsFarEnoughFromPlayer(candidate))
            {
                return candidate;
            }
        }

        return GetRandomPointOnArenaEdge(minX, maxX, minY, maxY);
    }

    private Vector2 GetRandomPointOnArenaEdge(float minX, float maxX, float minY, float maxY)
    {
        float safePadding = Mathf.Max(edgeSpawnPadding, 0f);
        float edge = Random.Range(0, 4);

        if (edge == 0)
        {
            return new Vector2(Random.Range(minX, maxX), Mathf.Max(maxY - safePadding, minY));
        }

        if (edge == 1)
        {
            return new Vector2(Random.Range(minX, maxX), Mathf.Min(minY + safePadding, maxY));
        }

        if (edge == 2)
        {
            return new Vector2(Mathf.Min(minX + safePadding, maxX), Random.Range(minY, maxY));
        }

        return new Vector2(Mathf.Max(maxX - safePadding, minX), Random.Range(minY, maxY));
    }

    private bool IsFarEnoughFromPlayer(Vector2 position)
    {
        if (player == null || minDistanceFromPlayer <= 0f)
        {
            return true;
        }

        return Vector2.Distance(position, player.position) >= minDistanceFromPlayer;
    }

    private bool IsInsideWallBounds(Vector2 position)
    {
        if (!TryGetWallBounds(out float minX, out float maxX, out float minY, out float maxY))
        {
            return true;
        }

        return position.x >= minX
            && position.x <= maxX
            && position.y >= minY
            && position.y <= maxY;
    }

    private Vector2 ClampToWallBounds(Vector2 position)
    {
        if (!TryGetWallBounds(out float minX, out float maxX, out float minY, out float maxY))
        {
            return position;
        }

        return new Vector2(
            Mathf.Clamp(position.x, minX, maxX),
            Mathf.Clamp(position.y, minY, maxY));
    }

    private bool TryGetWallBounds(out float minX, out float maxX, out float minY, out float maxY)
    {
        minX = maxX = minY = maxY = 0f;

        if (leftWall == null || rightWall == null || topWall == null || bottomWall == null)
        {
            return false;
        }

        minX = leftWall.bounds.max.x + wallPadding;
        maxX = rightWall.bounds.min.x - wallPadding;
        minY = bottomWall.bounds.max.y + wallPadding;
        maxY = topWall.bounds.min.y - wallPadding;

        return minX <= maxX && minY <= maxY;
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

        if (TryGetWallBounds(out float minX, out float maxX, out float minY, out float maxY))
        {
            Vector3 boundsCenter = new((minX + maxX) * 0.5f, (minY + maxY) * 0.5f, 0f);
            Vector3 boundsSize = new(maxX - minX, maxY - minY, 0f);

            Gizmos.color = Color.cyan;
            Gizmos.DrawWireCube(boundsCenter, boundsSize);

            if (spawnFromArenaEdges)
            {
                Vector3 edgeBoundsCenter = boundsCenter;
                Vector3 edgeBoundsSize = new(
                    Mathf.Max(boundsSize.x - edgeSpawnPadding * 2f, 0f),
                    Mathf.Max(boundsSize.y - edgeSpawnPadding * 2f, 0f),
                    0f);

                Gizmos.color = Color.green;
                Gizmos.DrawWireCube(edgeBoundsCenter, edgeBoundsSize);
            }
        }
    }
}
