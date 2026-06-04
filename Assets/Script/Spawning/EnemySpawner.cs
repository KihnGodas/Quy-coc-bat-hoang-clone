using System.Collections.Generic;
using UnityEngine;

public sealed class EnemySpawner : MonoBehaviour
{
    private enum SpawnMode
    {
        MixedTable,
        SingleTestEnemy,
        CycleTestEnemies
    }

    [SerializeField] private EnemyChaseAI2D enemyPrefab;
    [SerializeField] private EnemyBase enemyBasePrefab;
    [SerializeField] private SpawnMode spawnMode = SpawnMode.SingleTestEnemy;
    [SerializeField] private EnemyData testEnemyData;
    [SerializeField] private EnemySpawnEntry[] enemySpawnTable;
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
    [SerializeField] private bool spawningEnabled = true;
    [SerializeField] private CombatManager combatManager;
    [SerializeField, Min(0f)] private float spawnLockBeforeCombatEnd = 0.5f;

    private float nextSpawnTime;
    private int nextCycleSpawnIndex;
    private int totalKilled;
    private readonly List<GameObject> spawnedEnemies = new List<GameObject>();

    public int TotalKilled => totalKilled;
    private float runtimeSpawnInterval = -1f;
    private int runtimeMaxAliveEnemies = -1;

    private int totalSpawnedCount;
  

    public int AliveCount
    {
        get
        {
            CleanupDeadEnemies();
            return spawnedEnemies.Count;
        }
    }

    public bool IsSpawningEnabled => spawningEnabled;
    public float EffectiveSpawnInterval => runtimeSpawnInterval > 0f ? runtimeSpawnInterval : spawnInterval;
    public int EffectiveMaxAliveEnemies => runtimeMaxAliveEnemies > 0 ? runtimeMaxAliveEnemies : maxAliveEnemies;
    public int TotalSpawnedCount => totalSpawnedCount;
    public int DefeatedEnemyCount
    {
        get
        {
            CleanupDeadEnemies();
            return Mathf.Max(0, totalSpawnedCount - spawnedEnemies.Count);
        }
    }

    public void SetSpawningEnabled(bool enabled)
    {
        spawningEnabled = enabled;
        if (spawningEnabled)
        {
            nextSpawnTime = Time.time + EffectiveSpawnInterval;
        }
    }

    public void SetDifficultySpawnSettings(float effectiveSpawnInterval, int effectiveMaxAliveEnemies)
    {
        runtimeSpawnInterval = Mathf.Max(0.1f, effectiveSpawnInterval);
        runtimeMaxAliveEnemies = Mathf.Max(1, effectiveMaxAliveEnemies);
    }

    public void ClearDifficultySpawnSettings()
    {
        runtimeSpawnInterval = -1f;
        runtimeMaxAliveEnemies = -1;
    }

    private void Start()
    {
        FindPlayerIfNeeded();
        FindCombatManagerIfNeeded();
        nextSpawnTime = Time.time + EffectiveSpawnInterval;
    }

    private void Update()
    {
        if (!spawningEnabled)
        {
            return;
        }

        if (!CanSpawnForCombatState())
        {
            return;
        }

        FindPlayerIfNeeded();

        if (Time.time < nextSpawnTime)
        {
            return;
        }

        nextSpawnTime = Time.time + EffectiveSpawnInterval;

        CleanupDeadEnemies();

        if ((enemyPrefab == null && enemyBasePrefab == null) || spawnedEnemies.Count >= EffectiveMaxAliveEnemies)
        {
            return;
        }

        SpawnEnemy();
    }

    private void SpawnEnemy()
    {
        Vector2 center = GetSpawnCenter();
        Vector2 spawnPosition = GetSpawnPosition(center);
        EnemyData selectedData = SelectEnemyData();

        if (enemyBasePrefab != null)
        {
            EnemyBase enemy = Instantiate(enemyBasePrefab, spawnPosition, Quaternion.identity);
            EnsureEnemyRuntimeComponents(enemy);
            if (selectedData != null)
            {
                enemy.SetData(selectedData);
            }

            if (player != null)
            {
                enemy.SetTarget(player);
            }

            spawnedEnemies.Add(enemy.gameObject);
            SubscribeToEnemyDeath(enemy.gameObject);
            totalSpawnedCount++;
            return;
        }

        EnemyChaseAI2D legacyEnemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
        if (player != null)
        {
            legacyEnemy.SetTarget(player);

            EnemyBase enemyBase = legacyEnemy.GetComponent<EnemyBase>();
            if (enemyBase != null)
            {
                EnsureEnemyRuntimeComponents(enemyBase);
                if (selectedData != null)
                {
                    enemyBase.SetData(selectedData);
                }

                enemyBase.SetTarget(player);
            }
        }

        spawnedEnemies.Add(legacyEnemy.gameObject);
        SubscribeToEnemyDeath(legacyEnemy.gameObject);
    }

    private void SubscribeToEnemyDeath(GameObject enemyObj)
    {
        if (enemyObj == null)
        {
            return;
        }

        Health health = enemyObj.GetComponent<Health>();
        if (health != null)
        {
            health.OnDeath += () => totalKilled++;
        }

        SimpleHealth simpleHealth = enemyObj.GetComponent<SimpleHealth>();
        if (simpleHealth != null)
        {
            simpleHealth.OnDeath += () => totalKilled++;
        }
    }

    private EnemyData SelectEnemyData()
    {
        if (spawnMode == SpawnMode.SingleTestEnemy)
        {
            return testEnemyData != null ? testEnemyData : SelectFirstTableEnemy();
        }

        if (spawnMode == SpawnMode.CycleTestEnemies)
        {
            return SelectNextCycleEnemy();
        }

        if (enemySpawnTable == null || enemySpawnTable.Length == 0)
        {
            return null;
        }

        float elapsedTime = combatManager != null ? combatManager.ElapsedTime : Time.timeSinceLevelLoad;
        float totalWeight = 0f;

        for (int i = 0; i < enemySpawnTable.Length; i++)
        {
            EnemySpawnEntry entry = enemySpawnTable[i];
            if (entry != null && entry.CanSpawn(elapsedTime))
            {
                totalWeight += entry.Weight;
            }
        }

        if (totalWeight <= 0f)
        {
            return null;
        }

        float roll = Random.Range(0f, totalWeight);
        for (int i = 0; i < enemySpawnTable.Length; i++)
        {
            EnemySpawnEntry entry = enemySpawnTable[i];
            if (entry == null || !entry.CanSpawn(elapsedTime))
            {
                continue;
            }

            roll -= entry.Weight;
            if (roll <= 0f)
            {
                return entry.EnemyData;
            }
        }

        return null;
    }

    private EnemyData SelectFirstTableEnemy()
    {
        if (enemySpawnTable == null)
        {
            return null;
        }

        for (int i = 0; i < enemySpawnTable.Length; i++)
        {
            if (enemySpawnTable[i] != null && enemySpawnTable[i].EnemyData != null)
            {
                return enemySpawnTable[i].EnemyData;
            }
        }

        return null;
    }

    private EnemyData SelectNextCycleEnemy()
    {
        if (enemySpawnTable == null || enemySpawnTable.Length == 0)
        {
            return testEnemyData;
        }

        for (int attempt = 0; attempt < enemySpawnTable.Length; attempt++)
        {
            int index = nextCycleSpawnIndex % enemySpawnTable.Length;
            nextCycleSpawnIndex++;

            EnemySpawnEntry entry = enemySpawnTable[index];
            if (entry != null && entry.EnemyData != null && entry.Weight > 0f)
            {
                return entry.EnemyData;
            }
        }

        return testEnemyData;
    }

    private static void EnsureEnemyRuntimeComponents(EnemyBase enemy)
    {
        if (enemy == null)
        {
            return;
        }

        GameObject enemyObject = enemy.gameObject;
        AddComponentIfMissing<EnemyRoleController>(enemyObject);
        AddComponentIfMissing<EnemyVisual2D>(enemyObject);
        AddComponentIfMissing<EnemyChargeAttack>(enemyObject);
        AddComponentIfMissing<EnemyProjectileAttack>(enemyObject);
        AddComponentIfMissing<EnemyRockThrowAttack>(enemyObject);
        AddComponentIfMissing<EnemyRootAttack>(enemyObject);
        AddComponentIfMissing<EnemyExperienceDropper>(enemyObject);
    }

    private static void AddComponentIfMissing<T>(GameObject gameObject) where T : Component
    {
        if (gameObject.GetComponent<T>() == null)
        {
            gameObject.AddComponent<T>();
        }
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
        int edge = Random.Range(0, 4);

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

        PlayerMovement2D playerMovement = FindPlayerMovementInScene();
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

    private void FindCombatManagerIfNeeded()
    {
        if (combatManager != null)
        {
            return;
        }

        combatManager = FindComponentInScene<CombatManager>();
    }

    private bool CanSpawnForCombatState()
    {
        FindCombatManagerIfNeeded();

        if (combatManager == null)
        {
            return true;
        }

        if (!combatManager.IsRunning)
        {
            return false;
        }

        if (combatManager.Mode == CombatManager.CombatMode.BossCombat)
        {
            return false;
        }

        if (combatManager.Mode == CombatManager.CombatMode.NormalCombat
            && combatManager.RemainingTime <= spawnLockBeforeCombatEnd)
        {
            return false;
        }

        return true;
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
            Vector3 boundsCenter = new Vector3((minX + maxX) * 0.5f, (minY + maxY) * 0.5f, 0f);
            Vector3 boundsSize = new Vector3(maxX - minX, maxY - minY, 0f);

            Gizmos.color = Color.cyan;
            Gizmos.DrawWireCube(boundsCenter, boundsSize);

            if (spawnFromArenaEdges)
            {
                Vector3 edgeBoundsCenter = boundsCenter;
                Vector3 edgeBoundsSize = new Vector3(
                    Mathf.Max(boundsSize.x - edgeSpawnPadding * 2f, 0f),
                    Mathf.Max(boundsSize.y - edgeSpawnPadding * 2f, 0f),
                    0f);

                Gizmos.color = Color.green;
                Gizmos.DrawWireCube(edgeBoundsCenter, edgeBoundsSize);
            }
        }
    }

    private static PlayerMovement2D FindPlayerMovementInScene()
    {
        return FindComponentInScene<PlayerMovement2D>();
    }

    private static T FindComponentInScene<T>() where T : Object
    {
#if UNITY_2023_1_OR_NEWER
        return FindFirstObjectByType<T>();
#else
        return FindObjectOfType<T>();
#endif
    }
}
