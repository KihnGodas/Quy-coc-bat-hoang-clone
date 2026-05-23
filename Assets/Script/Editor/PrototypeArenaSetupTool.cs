using UnityEditor;
using UnityEngine;

public static class PrototypeArenaSetupTool
{
    private const string GeneratedPrefix = "TKCC_Generated_";
    private const string PrefabFolder = "Assets/_Project/Prefabs";
    private const string SquareSpritePath = PrefabFolder + "/TKCC_Generated_SquareSprite.asset";
    private const string ProjectilePrefabPath = PrefabFolder + "/TKCC_Generated_Projectile.prefab";
    private const string EnemyPrefabPath = PrefabFolder + "/TKCC_Generated_Enemy.prefab";

    [MenuItem("Tools/TKCC/Create Prototype Arena")]
    public static void CreatePrototypeArena()
    {
        EnsurePrefabFolder();

        Sprite squareSprite = GetOrCreateSquareSprite();
        Projectile2D projectilePrefab = CreateOrUpdateProjectilePrefab(squareSprite);
        EnemyChaseAI2D enemyPrefab = CreateOrUpdateEnemyPrefab(squareSprite);

        Camera mainCamera = CreateOrUpdateMainCamera();
        GameObject arena = CreateOrUpdateArena(squareSprite);
        GameObject player = CreateOrUpdatePlayer(squareSprite, mainCamera, projectilePrefab);
        GameObject spawner = CreateOrUpdateSpawner(enemyPrefab, player.transform);

        Selection.objects = new Object[] { player, spawner, arena };
        EditorUtility.SetDirty(player);
        EditorUtility.SetDirty(spawner);
        EditorUtility.SetDirty(arena);

        Debug.Log("Created TKCC prototype arena setup. Press Play to test movement, shooting, enemy spawning, and chase AI.");
    }

    private static void EnsurePrefabFolder()
    {
        if (!AssetDatabase.IsValidFolder("Assets/_Project"))
        {
            AssetDatabase.CreateFolder("Assets", "_Project");
        }

        if (!AssetDatabase.IsValidFolder(PrefabFolder))
        {
            AssetDatabase.CreateFolder("Assets/_Project", "Prefabs");
        }
    }

    private static Sprite GetOrCreateSquareSprite()
    {
        Sprite existingSprite = AssetDatabase.LoadAssetAtPath<Sprite>(SquareSpritePath);
        if (existingSprite != null)
        {
            return existingSprite;
        }

        Texture2D texture = new Texture2D(16, 16, TextureFormat.RGBA32, false)
        {
            name = "TKCC_Generated_SquareTexture"
        };

        Color[] pixels = new Color[16 * 16];
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = Color.white;
        }

        texture.SetPixels(pixels);
        texture.Apply();

        AssetDatabase.CreateAsset(texture, SquareSpritePath);

        Sprite sprite = Sprite.Create(texture, new Rect(0f, 0f, 16f, 16f), new Vector2(0.5f, 0.5f), 16f);
        sprite.name = "TKCC_Generated_SquareSprite";
        AssetDatabase.AddObjectToAsset(sprite, texture);
        AssetDatabase.SaveAssets();

        return sprite;
    }

    private static Projectile2D CreateOrUpdateProjectilePrefab(Sprite sprite)
    {
        GameObject projectileObject = new GameObject("TKCC_Generated_Projectile");

        SpriteRenderer renderer = projectileObject.AddComponent<SpriteRenderer>();
        renderer.sprite = sprite;
        renderer.color = new Color(1f, 0.85f, 0.25f);
        projectileObject.transform.localScale = new Vector3(0.25f, 0.25f, 1f);

        Rigidbody2D body = projectileObject.AddComponent<Rigidbody2D>();
        body.bodyType = RigidbodyType2D.Kinematic;
        body.gravityScale = 0f;

        CircleCollider2D collider = projectileObject.AddComponent<CircleCollider2D>();
        collider.isTrigger = true;
        collider.radius = 0.5f;

        projectileObject.AddComponent<Projectile2D>();

        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(projectileObject, ProjectilePrefabPath);
        Object.DestroyImmediate(projectileObject);

        return prefab.GetComponent<Projectile2D>();
    }

    private static EnemyChaseAI2D CreateOrUpdateEnemyPrefab(Sprite sprite)
    {
        GameObject enemyObject = new GameObject("TKCC_Generated_Enemy");

        SpriteRenderer renderer = enemyObject.AddComponent<SpriteRenderer>();
        renderer.sprite = sprite;
        renderer.color = new Color(0.9f, 0.2f, 0.2f);
        enemyObject.transform.localScale = new Vector3(0.8f, 0.8f, 1f);

        Rigidbody2D body = enemyObject.AddComponent<Rigidbody2D>();
        body.gravityScale = 0f;
        body.freezeRotation = true;

        CircleCollider2D collider = enemyObject.AddComponent<CircleCollider2D>();
        collider.radius = 0.5f;

        enemyObject.AddComponent<SimpleHealth>();
        enemyObject.AddComponent<EnemyChaseAI2D>();

        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(enemyObject, EnemyPrefabPath);
        Object.DestroyImmediate(enemyObject);

        return prefab.GetComponent<EnemyChaseAI2D>();
    }

    private static Camera CreateOrUpdateMainCamera()
    {
        GameObject cameraObject = GameObject.Find(GeneratedPrefix + "Main Camera");
        if (cameraObject == null)
        {
            Camera existingMainCamera = Camera.main;
            cameraObject = existingMainCamera != null && existingMainCamera.name.StartsWith(GeneratedPrefix)
                ? existingMainCamera.gameObject
                : new GameObject(GeneratedPrefix + "Main Camera");

            if (cameraObject.scene.IsValid())
            {
                Undo.RegisterCreatedObjectUndo(cameraObject, "Create Prototype Camera");
            }
        }

        Camera camera = cameraObject.GetComponent<Camera>();
        if (camera == null)
        {
            camera = Undo.AddComponent<Camera>(cameraObject);
        }

        Undo.RecordObject(cameraObject.transform, "Configure Prototype Camera");
        Undo.RecordObject(camera, "Configure Prototype Camera");
        cameraObject.tag = "MainCamera";
        cameraObject.transform.position = new Vector3(0f, 0f, -10f);
        cameraObject.transform.rotation = Quaternion.identity;
        camera.orthographic = true;
        camera.orthographicSize = 6f;
        camera.backgroundColor = new Color(0.08f, 0.08f, 0.1f);

        return camera;
    }

    private static GameObject CreateOrUpdateArena(Sprite sprite)
    {
        GameObject arena = GetOrCreateGeneratedObject("Arena_Background", "Create Prototype Arena Background");
        Undo.RecordObject(arena.transform, "Configure Prototype Arena Background");
        arena.transform.position = Vector3.zero;
        arena.transform.localScale = new Vector3(9f, 16f, 1f);

        SpriteRenderer renderer = arena.GetComponent<SpriteRenderer>();
        if (renderer == null)
        {
            renderer = Undo.AddComponent<SpriteRenderer>(arena);
        }

        Undo.RecordObject(renderer, "Configure Prototype Arena Background");
        renderer.sprite = sprite;
        renderer.color = new Color(0.12f, 0.16f, 0.18f);
        renderer.sortingOrder = -10;

        return arena;
    }

    private static GameObject CreateOrUpdatePlayer(Sprite sprite, Camera aimCamera, Projectile2D projectilePrefab)
    {
        GameObject player = GetOrCreateGeneratedObject("Player", "Create Prototype Player");
        Undo.RecordObject(player.transform, "Configure Prototype Player");
        player.transform.position = Vector3.zero;
        player.transform.localScale = new Vector3(0.8f, 0.8f, 1f);
        TrySetTag(player, "Player");

        SpriteRenderer renderer = player.GetComponent<SpriteRenderer>();
        if (renderer == null)
        {
            renderer = Undo.AddComponent<SpriteRenderer>(player);
        }

        Undo.RecordObject(renderer, "Configure Prototype Player");
        renderer.sprite = sprite;
        renderer.color = new Color(0.25f, 0.65f, 1f);
        renderer.sortingOrder = 1;

        Rigidbody2D body = player.GetComponent<Rigidbody2D>();
        if (body == null)
        {
            body = Undo.AddComponent<Rigidbody2D>(player);
        }

        Undo.RecordObject(body, "Configure Prototype Player");
        body.gravityScale = 0f;
        body.freezeRotation = true;

        CircleCollider2D collider = player.GetComponent<CircleCollider2D>();
        if (collider == null)
        {
            collider = Undo.AddComponent<CircleCollider2D>(player);
        }

        Undo.RecordObject(collider, "Configure Prototype Player");
        collider.radius = 0.5f;

        if (player.GetComponent<PlayerMovement2D>() == null)
        {
            Undo.AddComponent<PlayerMovement2D>(player);
        }

        PlayerShooter2D shooter = player.GetComponent<PlayerShooter2D>();
        if (shooter == null)
        {
            shooter = Undo.AddComponent<PlayerShooter2D>(player);
        }

        Transform spawnPoint = player.transform.Find(GeneratedPrefix + "ProjectileSpawnPoint");
        if (spawnPoint == null)
        {
            GameObject spawnPointObject = new GameObject(GeneratedPrefix + "ProjectileSpawnPoint");
            Undo.RegisterCreatedObjectUndo(spawnPointObject, "Create Projectile Spawn Point");
            spawnPointObject.transform.SetParent(player.transform);
            spawnPoint = spawnPointObject.transform;
        }

        Undo.RecordObject(spawnPoint, "Configure Projectile Spawn Point");
        spawnPoint.localPosition = new Vector3(0.75f, 0f, 0f);
        spawnPoint.localRotation = Quaternion.identity;
        spawnPoint.localScale = Vector3.one;

        SerializedObject shooterObject = new SerializedObject(shooter);
        shooterObject.FindProperty("aimCamera").objectReferenceValue = aimCamera;
        shooterObject.FindProperty("projectilePrefab").objectReferenceValue = projectilePrefab;
        shooterObject.FindProperty("projectileSpawnPoint").objectReferenceValue = spawnPoint;
        shooterObject.FindProperty("fireRate").floatValue = 6f;
        shooterObject.ApplyModifiedProperties();

        return player;
    }

    private static GameObject CreateOrUpdateSpawner(EnemyChaseAI2D enemyPrefab, Transform player)
    {
        GameObject spawner = GetOrCreateGeneratedObject("EnemySpawner", "Create Enemy Spawner");
        Undo.RecordObject(spawner.transform, "Configure Enemy Spawner");
        spawner.transform.position = Vector3.zero;

        EnemySpawner enemySpawner = spawner.GetComponent<EnemySpawner>();
        if (enemySpawner == null)
        {
            enemySpawner = Undo.AddComponent<EnemySpawner>(spawner);
        }

        SerializedObject spawnerObject = new SerializedObject(enemySpawner);
        spawnerObject.FindProperty("enemyPrefab").objectReferenceValue = enemyPrefab;
        spawnerObject.FindProperty("player").objectReferenceValue = player;
        spawnerObject.FindProperty("spawnInterval").floatValue = 1.5f;
        spawnerObject.FindProperty("maxAliveEnemies").intValue = 8;
        spawnerObject.FindProperty("spawnRadius").floatValue = 7f;
        spawnerObject.FindProperty("minDistanceFromPlayer").floatValue = 2.5f;
        spawnerObject.FindProperty("spawnAroundPlayer").boolValue = true;
        spawnerObject.ApplyModifiedProperties();

        return spawner;
    }

    private static GameObject GetOrCreateGeneratedObject(string shortName, string undoName)
    {
        string objectName = GeneratedPrefix + shortName;
        GameObject existing = GameObject.Find(objectName);
        if (existing != null)
        {
            Undo.RecordObject(existing.transform, undoName);
            return existing;
        }

        GameObject created = new GameObject(objectName);
        Undo.RegisterCreatedObjectUndo(created, undoName);
        return created;
    }

    private static void TrySetTag(GameObject gameObject, string tagName)
    {
        try
        {
            gameObject.tag = tagName;
        }
        catch (UnityException)
        {
            Debug.LogWarning($"Tag '{tagName}' does not exist. Player lookup still works through PlayerMovement2D.");
        }
    }
}
