using UnityEngine;
using UnityEngine.SceneManagement;

public sealed class GameManager : MonoBehaviour
{
    public float levelDuration = 20f;

    public enum GameState { Playing, Won, Lost }
    public GameState CurrentState { get; private set; } = GameState.Playing;

    private float timeRemaining;
    private SimpleHealth playerHealth;
    private EnemySpawner spawner;
    private GameUI gameUI;

    private void Start()
    {
        timeRemaining = levelDuration;
        CurrentState = GameState.Playing;
        Time.timeScale = 1f;
        FindPlayerHealth();
        FindSpawner();
        CreateGameUI();
    }

    private void FindPlayerHealth()
    {
        var playerMovement = FindFirstObjectByType<PlayerMovement2D>();
        if (playerMovement != null)
        {
            playerHealth = playerMovement.GetComponent<SimpleHealth>();
            if (playerHealth != null)
                playerHealth.OnDeath.AddListener(OnPlayerDied);
        }
    }

    private void FindSpawner()
    {
        spawner = FindFirstObjectByType<EnemySpawner>();
    }

    private void CreateGameUI()
    {
        var uiGO = new GameObject("GameUI", typeof(GameUI));
        gameUI = uiGO.GetComponent<GameUI>();
    }

    private void Update()
    {
        if (CurrentState != GameState.Playing)
            return;

        timeRemaining -= Time.deltaTime;
        if (timeRemaining <= 0f)
        {
            timeRemaining = 0f;
            Victory();
        }

        if (gameUI != null)
            gameUI.UpdateTimer(timeRemaining);
    }

    private void OnPlayerDied()
    {
        if (CurrentState != GameState.Playing)
            return;
        Defeat();
    }

    private void Victory()
    {
        CurrentState = GameState.Won;
        StopGame();
        if (gameUI != null)
            gameUI.ShowResult(true);
    }

    private void Defeat()
    {
        CurrentState = GameState.Lost;
        StopGame();
        if (gameUI != null)
            gameUI.ShowResult(false);
    }

    private void StopGame()
    {
        if (spawner != null)
            spawner.isSpawning = false;
        Time.timeScale = 0f;
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void AutoCreate()
    {
        if (FindFirstObjectByType<GameManager>() == null)
        {
            var go = new GameObject("GameManager");
            go.AddComponent<GameManager>();
        }
    }
}
