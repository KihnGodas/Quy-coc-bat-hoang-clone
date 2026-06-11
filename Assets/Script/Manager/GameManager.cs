using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum CharacterType { Male, Female }

public sealed class GameManager : MonoBehaviour
{
    [SerializeField] private StageProgression stageProgression;
    [SerializeField] private string combatSceneName = "CombatScene";
    [SerializeField] private string mainMenuSceneName = "MainMenu";
    [SerializeField] private string tutorialSceneName = "TutorialScene";
    [SerializeField] private string stageProgressionResourcePath = "StageProgression";

    private int currentStageIndex;
    private readonly HashSet<int> unlockedStages = new HashSet<int>();
    private int savedLevel = 1;
    private float savedExperience;
    private bool tutorialCompleted;
    private readonly HashSet<TranThienKhiType> collectedTranThienKhi = new HashSet<TranThienKhiType>();

    public static GameManager Instance { get; private set; }
    public CharacterType SelectedCharacter { get; set; }
    public string PlayerName { get; set; }
    public WeaponType SelectedWeapon { get; set; }

    public StageProgression StageProgression => stageProgression;
    public int CurrentStageIndex => currentStageIndex;
    public StageData CurrentStage => stageProgression != null ? stageProgression.GetStage(currentStageIndex) : null;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (stageProgression == null)
        {
            stageProgression = Resources.Load<StageProgression>(stageProgressionResourcePath);

#if UNITY_EDITOR
            if (stageProgression == null)
            {
                stageProgression = UnityEditor.AssetDatabase.LoadAssetAtPath<StageProgression>(
                    "Assets/ScriptableObjects/Stages/StageProgression.asset");
            }
#endif

            if (stageProgression == null)
            {
                Debug.LogError("StageProgression not found! Run Tools > Create All Stage Assets in Unity Editor.");
            }
        }

        UnlockStage(0);
    }

    public bool IsStageUnlocked(int stageIndex)
    {
        return unlockedStages.Contains(stageIndex);
    }

    public void UnlockStage(int stageIndex)
    {
        unlockedStages.Add(stageIndex);
    }

    public void SavePlayerProgression(int level, float experience)
    {
        savedLevel = Mathf.Max(1, level);
        savedExperience = Mathf.Max(0f, experience);
    }

    public void RestorePlayerProgression(PlayerExperience playerExperience)
    {
        if (playerExperience == null)
        {
            return;
        }

        if (savedLevel > 1 || savedExperience > 0f)
        {
            playerExperience.SetProgress(savedLevel, savedExperience);
        }
    }

    public bool TryLoadStage(int stageIndex)
    {
        if (stageProgression == null)
        {
            Debug.LogError("StageProgression not assigned in GameManager!");
            return false;
        }

        StageData stage = stageProgression.GetStage(stageIndex);
        if (stage == null)
        {
            Debug.LogError($"Stage index {stageIndex} not found in progression!");
            return false;
        }

        if (!unlockedStages.Contains(stageIndex))
        {
            Debug.LogWarning($"Stage {stage.StageName} is not unlocked yet!");
            return false;
        }

        currentStageIndex = stageIndex;
        SceneManager.LoadScene(combatSceneName);
        return true;
    }

    public void CompleteCurrentStage(CombatResult result)
    {
        if (result != CombatResult.Victory)
        {
            return;
        }

        int nextIndex = currentStageIndex + 1;
        if (stageProgression != null && nextIndex < stageProgression.TotalStages)
        {
            UnlockStage(nextIndex);
        }
    }

    public void CollectTranThienKhi(TranThienKhiType type)
    {
        if (type != TranThienKhiType.None)
        {
            collectedTranThienKhi.Add(type);
        }
    }

    public bool HasTranThienKhi(TranThienKhiType type)
    {
        return collectedTranThienKhi.Contains(type);
    }

    public HashSet<TranThienKhiType>.Enumerator GetAllTranThienKhi()
    {
        return collectedTranThienKhi.GetEnumerator();
    }

    public static bool TutorialCompleted => Instance != null && Instance.tutorialCompleted;

    public void LoadTutorial()
    {
        SceneManager.LoadScene(tutorialSceneName);
    }

    public void CompleteTutorial()
    {
        tutorialCompleted = true;
        if (!unlockedStages.Contains(0))
        {
            UnlockStage(0);
        }
        GoToMainMenu();
    }

    public void CompleteTutorialAndPlayStage1()
    {
        tutorialCompleted = true;
        if (!unlockedStages.Contains(0))
        {
            UnlockStage(0);
        }
        TryLoadStage(0);
    }

    public void GoToMainMenu()
    {
        SavePlayerProgressionFromScene();
        SceneManager.LoadScene(mainMenuSceneName);
    }

    public void ReloadCurrentStage()
    {
        SavePlayerProgressionFromScene();
        SceneManager.LoadScene(combatSceneName);
    }

    public void LoadNextStage()
    {
        SavePlayerProgressionFromScene();

        int nextIndex = currentStageIndex + 1;
        if (stageProgression != null && nextIndex < stageProgression.TotalStages && unlockedStages.Contains(nextIndex))
        {
            TryLoadStage(nextIndex);
        }
        else
        {
            GoToMainMenu();
        }
    }

    private void SavePlayerProgressionFromScene()
    {
        PlayerExperience exp = FindComponentInScene<PlayerExperience>();
        if (exp != null)
        {
            SavePlayerProgression(exp.Level, exp.CurrentExperience);
        }
    }

    private static T FindComponentInScene<T>() where T : UnityEngine.Object
    {
#if UNITY_2023_1_OR_NEWER
        return FindFirstObjectByType<T>();
#else
        return FindObjectOfType<T>();
#endif
    }
}
