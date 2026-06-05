using UnityEditor;
using UnityEngine;

public static class CreateStageAssets
{
    [MenuItem("Tools/Create All Stage Assets")]
    public static void CreateAllStages()
    {
        DeleteStaleAsset("Assets/Resources/StageProgression.asset");

        string stagesPath = "Assets/ScriptableObjects/Stages";
        if (!AssetDatabase.IsValidFolder(stagesPath))
        {
            AssetDatabase.CreateFolder("Assets/ScriptableObjects", "Stages");
        }

        StageData[] stages = new StageData[9];

        stages[0] = CreateStageData(
            "Stages/Stage1_RungMaSuong.asset",
            "Rừng Ma Sương", 1, "Khu rừng hoang vu đầy yêu thú cấp thấp",
            CombatManager.CombatMode.NormalCombat,
            duration: 30f,
            spawnData: new[] { ("EnemyData_DaiLang", 0f, 3f), ("EnemyData_HacLang", 5f, 2f) },
            spawnInterval: 2.5f, maxAlive: 8,
            enableDiff: true, earlySI: 2.8f, lateSI: 1.2f, earlyMA: 5, lateMA: 10,
            arenaSize: new Vector2(20f, 14f),
            reqStage: 0, reqLevel: 1, expReward: 50f);

        stages[1] = CreateStageData(
            "Stages/Stage2_SonDongYeuThu.asset",
            "Sơn Động Yêu Thú", 2, "Hang động tối tăm với lũ yêu quái hung dữ",
            CombatManager.CombatMode.NormalCombat,
            duration: 35f,
            spawnData: new[]
            {
                ("EnemyData_DaiLang", 0f, 3f),
                ("EnemyData_HacLang", 0f, 2f),
                ("EnemyData_KhoiLang", 8f, 2f),
                ("EnemyData_MocYeu", 12f, 1.5f),
            },
            spawnInterval: 2.2f, maxAlive: 10,
            enableDiff: true, earlySI: 2.5f, lateSI: 1f, earlyMA: 6, lateMA: 12,
            arenaSize: new Vector2(22f, 15f),
            reqStage: 1, reqLevel: 2, expReward: 80f);

        stages[2] = CreateStageData(
            "Stages/Stage3_HuyetTriXaVuong.asset",
            "Huyết Trì Xà Vương", 3, "Đầm lầy máu - nơi trú ngụ của Xà Yêu Vương",
            CombatManager.CombatMode.BossCombat,
            bossName: "Xà Yêu Vương", bossHP: 2000f, bossDamage: 35f,
            bossSpawn: Vector2.zero, bossScale: Vector2.one * 1.8f,
            arenaSize: new Vector2(18f, 12f),
            reqStage: 2, reqLevel: 3, expReward: 200f);

        stages[3] = CreateStageData(
            "Stages/Stage4_ThachLamCuNhan.asset",
            "Thạch Lâm Cự Nhân", 4, "Khu rừng đá với những tên Oai Hùng và Thiết Trư",
            CombatManager.CombatMode.NormalCombat,
            duration: 35f,
            spawnData: new[]
            {
                ("EnemyData_HacLang", 0f, 2f),
                ("EnemyData_KhoiLang", 0f, 2f),
                ("EnemyData_MocYeu", 5f, 2f),
                ("EnemyData_OaiHung", 8f, 1.5f),
                ("EnemyData_ThietTru", 12f, 1.5f),
            },
            spawnInterval: 2f, maxAlive: 10,
            enableDiff: true, earlySI: 2.2f, lateSI: 0.9f, earlyMA: 7, lateMA: 13,
            arenaSize: new Vector2(24f, 16f),
            reqStage: 3, reqLevel: 4, expReward: 120f);

        stages[4] = CreateStageData(
            "Stages/Stage5_CoMocLam.asset",
            "Cổ Mộc Lâm", 5, "Rừng cây cổ thụ với bọn Mộc Yêu và Ưng Yêu phiên bản mạnh",
            CombatManager.CombatMode.NormalCombat,
            duration: 40f,
            spawnData: new[]
            {
                ("EnemyData_OaiHung", 0f, 2f),
                ("EnemyData_ThietTru", 0f, 2f),
                ("EnemyData_KhoiLang", 0f, 2f),
                ("EnemyData_KimVien", 5f, 2f),
            },
            spawnInterval: 1.8f, maxAlive: 12,
            enableDiff: true, earlySI: 2f, lateSI: 0.8f, earlyMA: 8, lateMA: 14,
            arenaSize: new Vector2(26f, 17f),
            reqStage: 4, reqLevel: 5, expReward: 160f);

        stages[5] = CreateStageData(
            "Stages/Stage6_CoThuThanhTinh.asset",
            "Cổ Thụ Thành Tinh", 6, "Cổ thụ ngàn năm tu luyện thành tinh - Cổ Thụ Tôn Giả",
            CombatManager.CombatMode.BossCombat,
            bossName: "Cổ Thụ Tôn Giả", bossHP: 4500f, bossDamage: 60f,
            bossSpawn: Vector2.zero, bossScale: new Vector2(2.2f, 2.2f),
            arenaSize: new Vector2(22f, 14f),
            reqStage: 5, reqLevel: 7, expReward: 500f);

        stages[6] = CreateStageData(
            "Stages/Stage7_HoaDiemSon.asset",
            "Hỏa Diệm Sơn", 7, "Ngọn núi lửa rực cháy với yêu quái mạnh mẽ",
            CombatManager.CombatMode.NormalCombat,
            duration: 40f,
            spawnData: new[]
            {
                ("EnemyData_MocYeu", 0f, 2.5f),
                ("EnemyData_UngYeu", 0f, 2f),
                ("EnemyData_KimVien", 5f, 2f),
                ("EnemyData_DaiLang", 0f, 1.5f),
                ("EnemyData_XaYeu", 8f, 2f),
            },
            spawnInterval: 1.6f, maxAlive: 13,
            enableDiff: true, earlySI: 1.8f, lateSI: 0.7f, earlyMA: 9, lateMA: 16,
            arenaSize: new Vector2(26f, 17f),
            reqStage: 6, reqLevel: 7, expReward: 250f);

        stages[7] = CreateStageData(
            "Stages/Stage8_KimThietThanh.asset",
            "Kim Thiết Thành", 8, "Pháo đài kim loại với lũ yêu quái tinh nhuệ",
            CombatManager.CombatMode.NormalCombat,
            duration: 50f,
            spawnData: new[]
            {
                ("EnemyData_OaiHung", 0f, 2f),
                ("EnemyData_KimVien", 0f, 2f),
                ("EnemyData_UngYeu", 0f, 2f),
                ("EnemyData_XaYeu", 3f, 2f),
                ("EnemyData_HacLang", 0f, 2f),
                ("EnemyData_MocYeu", 3f, 1.5f),
            },
            spawnInterval: 1.4f, maxAlive: 15,
            enableDiff: true, earlySI: 1.6f, lateSI: 0.6f, earlyMA: 10, lateMA: 18,
            arenaSize: new Vector2(28f, 18f),
            reqStage: 7, reqLevel: 9, expReward: 350f);

        stages[8] = CreateStageData(
            "Stages/Stage9_BatHoangChiChu.asset",
            "Bát Hoang Chi Chủ", 9, "Chúa tể Bát Hoang - trận chiến cuối cùng",
            CombatManager.CombatMode.BossCombat,
            bossName: "Bát Hoang Chi Chủ", bossHP: 8000f, bossDamage: 100f,
            bossSpawn: Vector2.zero, bossScale: Vector2.one * 2.5f,
            arenaSize: new Vector2(30f, 20f),
            reqStage: 8, reqLevel: 10, expReward: 1000f);

        StageProgression progression = ScriptableObject.CreateInstance<StageProgression>();
        string progPath = "Assets/ScriptableObjects/Stages/StageProgression.asset";
        AssetDatabase.CreateAsset(progression, progPath);

        SerializedObject serializedProg = new SerializedObject(progression);
        SerializedProperty stagesProp = serializedProg.FindProperty("stages");
        stagesProp.ClearArray();
        stagesProp.arraySize = stages.Length;
        for (int i = 0; i < stages.Length; i++)
        {
            stagesProp.GetArrayElementAtIndex(i).objectReferenceValue = stages[i];
        }
        serializedProg.ApplyModifiedProperties();

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        string resourcesDir = "Assets/Resources";
        if (!AssetDatabase.IsValidFolder(resourcesDir))
        {
            AssetDatabase.CreateFolder("Assets", "Resources");
        }

        string resourcesProgPath = "Assets/Resources/StageProgression.asset";
        StageProgression resourcesCopy = Object.Instantiate(progression);
        AssetDatabase.CreateAsset(resourcesCopy, resourcesProgPath);
        SerializedObject serializedCopy = new SerializedObject(resourcesCopy);
        SerializedProperty copyStagesProp = serializedCopy.FindProperty("stages");
        copyStagesProp.ClearArray();
        copyStagesProp.arraySize = stages.Length;
        for (int i = 0; i < stages.Length; i++)
        {
            copyStagesProp.GetArrayElementAtIndex(i).objectReferenceValue = stages[i];
        }
        serializedCopy.ApplyModifiedProperties();

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"Created {stages.Length} stage assets + StageProgression\n  Main: {progPath}\n  Resources: {resourcesProgPath}");

        Selection.activeObject = progression;
        EditorGUIUtility.PingObject(progression);
    }

    private static StageData CreateStageData(
        string assetPath,
        string name, int number, string description,
        CombatManager.CombatMode mode,
        float duration = 30f,
        (string enemyName, float unlockTime, float weight)[] spawnData = null,
        float spawnInterval = 2f,
        int maxAlive = 10,
        bool enableDiff = true,
        float earlySI = 2.2f,
        float lateSI = 0.85f,
        int earlyMA = 7,
        int lateMA = 14,
        string bossName = "Boss",
        float bossHP = 4500f,
        float bossDamage = 60f,
        Vector2 bossSpawn = default,
        Vector2 bossScale = default,
        Vector2 arenaSize = default,
        int reqStage = 0,
        int reqLevel = 1,
        float expReward = 0f)
    {
        StageData stage = ScriptableObject.CreateInstance<StageData>();

        string fullPath = $"Assets/ScriptableObjects/{assetPath}";
        AssetDatabase.CreateAsset(stage, fullPath);

        SerializedObject so = new SerializedObject(stage);
        so.FindProperty("stageName").stringValue = name;
        so.FindProperty("stageNumber").intValue = number;
        so.FindProperty("description").stringValue = description;
        so.FindProperty("combatMode").enumValueIndex = (int)mode;

        if (mode == CombatManager.CombatMode.NormalCombat)
        {
            so.FindProperty("duration").floatValue = duration;
            so.FindProperty("spawnInterval").floatValue = spawnInterval;
            so.FindProperty("maxAliveEnemies").intValue = maxAlive;
            so.FindProperty("enableDifficultyScaling").boolValue = enableDiff;
            so.FindProperty("earlySpawnInterval").floatValue = earlySI;
            so.FindProperty("lateSpawnInterval").floatValue = lateSI;
            so.FindProperty("earlyMaxAlive").intValue = earlyMA;
            so.FindProperty("lateMaxAlive").intValue = lateMA;

            if (spawnData != null && spawnData.Length > 0)
            {
                SerializedProperty tableProp = so.FindProperty("spawnTable");
                tableProp.ClearArray();
                tableProp.arraySize = spawnData.Length;

                for (int i = 0; i < spawnData.Length; i++)
                {
                    SerializedProperty entry = tableProp.GetArrayElementAtIndex(i);

                    string[] guids = AssetDatabase.FindAssets(spawnData[i].enemyName, new[] { "Assets/ScriptableObjects/Enemies" });
                    if (guids.Length > 0)
                    {
                        EnemyData enemyData = AssetDatabase.LoadAssetAtPath<EnemyData>(
                            AssetDatabase.GUIDToAssetPath(guids[0]));
                        entry.FindPropertyRelative("enemyData").objectReferenceValue = enemyData;
                    }

                    entry.FindPropertyRelative("unlockTime").floatValue = spawnData[i].unlockTime;
                    entry.FindPropertyRelative("weight").floatValue = spawnData[i].weight;
                }
            }
        }
        else
        {
            so.FindProperty("bossName").stringValue = bossName;
            so.FindProperty("bossHP").floatValue = bossHP;
            so.FindProperty("bossDamage").floatValue = bossDamage;
            so.FindProperty("bossSpawnPosition").vector2Value = bossSpawn;
            so.FindProperty("bossScale").vector2Value = bossScale == default ? Vector2.one : bossScale;
        }

        if (arenaSize != default)
        {
            so.FindProperty("arenaSize").vector2Value = arenaSize;
        }

        so.FindProperty("requiredStageNumber").intValue = reqStage;
        so.FindProperty("requiredPlayerLevel").intValue = reqLevel;
        so.FindProperty("experienceReward").floatValue = expReward;

        so.ApplyModifiedProperties();
        return stage;
    }

    private static void DeleteStaleAsset(string path)
    {
        if (!string.IsNullOrEmpty(AssetDatabase.AssetPathToGUID(path)))
        {
            AssetDatabase.DeleteAsset(path);
            AssetDatabase.Refresh();
        }
    }
}
