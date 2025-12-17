using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState { Start, Playing, Paused, Ended }
public enum GameMode { Normal, Endless }

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public GameState CurrentState { get; private set; }

    public GameMode CurrentMode { get; private set; } = GameMode.Normal;

    // Endless mode scoring
    public int EndlessRunScore { get; private set; }
    public int EndlessBestScore { get; private set; }

    private const string EndlessBestScoreKey = "EndlessBestScore";

    public LevelDesigner levelDesigner;
    public StartScreenManager startScreenManager;
    public GameplayUI gameplayUI;

    void Awake()
    {
        // Singleton pattern - ensure only one GameManager exists
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persist across scene reloads

            // Load persistent endless best score
            EndlessBestScore = PlayerPrefs.GetInt(EndlessBestScoreKey, 0);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        StartGame();
    }

    public void StartGame()
    {

        // Re-acquire levelDesigner reference after scene reload
        if (levelDesigner == null)
        {
            levelDesigner = FindFirstObjectByType<LevelDesigner>();
            if (levelDesigner == null)
            {
                Debug.LogError("GameManager: LevelDesigner not found in scene!");
                return;
            }
        }

        if (startScreenManager == null)
        {
            startScreenManager = FindFirstObjectByType<StartScreenManager>();
            if (startScreenManager == null)
            {
                Debug.LogError("GameManager: StartScreenManager not found in scene!");
                return;
            }
        }

        if (gameplayUI == null)
        {
            gameplayUI = FindFirstObjectByType<GameplayUI>();
            if (gameplayUI == null)
            {
                Debug.LogError("GameManager: GameplayUI not found in scene!");
                return;
            }
        }

        levelDesigner.GenerateLevel();
        CurrentState = GameState.Start;
        startScreenManager.ShowStartScreen();
        if (gameplayUI != null)
        {
            gameplayUI.HideGameplayUI();
        }
        Time.timeScale = 0f; // Pause the game initially for start screen
    }

    // Start a normal (non-endless) run from the start menu
    public void StartNormalRun()
    {
        CurrentMode = GameMode.Normal;
        EndlessRunScore = 0;
        PlayGame();
    }

    // Start an endless run from the start menu
    public void StartEndlessRun()
    {
        CurrentMode = GameMode.Endless;
        EndlessRunScore = 0;
        PlayGame();
    }

    // Called when Play button is clicked or ESC is pressed while paused
    public void PlayGame()
    {
        CurrentState = GameState.Playing;
        Time.timeScale = 1f; // Resume normal time
        gameplayUI.ShowGameplayUI();
        Debug.Log("Game Started/Resumed from " + CurrentState);
    }

    // Called when ESC is pressed during gameplay
    public void PauseGame()
    {
        CurrentState = GameState.Paused;
        Time.timeScale = 0f; // Freeze time
        Debug.Log("Game Paused");
    }

    public void RegenerateLevel()
    {
        levelDesigner.GenerateLevel();
        PlayGame();
        Debug.Log("Level Regenerated");
    }

    // Called when Restart button is clicked
    public void RestartGame()
    {
        levelDesigner.RestartLevel();
        PlayGame();
        Debug.Log("Game Restarted");
    }

    // Called when player reaches the goal flag for the level, not a permanent end
    public void EndGame()
    {
        CurrentState = GameState.Ended;
        gameplayUI.HideGameplayUI();
        Debug.Log("Game Ended - Player reached goal! Current state: " + CurrentState);

        // Update endless scoring if in endless mode
        if (CurrentMode == GameMode.Endless)
        {
            EndlessRunScore++;

            if (EndlessRunScore > EndlessBestScore)
            {
                EndlessBestScore = EndlessRunScore;
                PlayerPrefs.SetInt(EndlessBestScoreKey, EndlessBestScore);
                PlayerPrefs.Save();
            }

            Debug.Log($"Endless mode: run score = {EndlessRunScore}, best score = {EndlessBestScore}");
        }

        Time.timeScale = 0f; // Freeze time
        Debug.Log("Game Ended - Player reached goal!");
    }

    // Explicitly end an endless run and return to the start state
    public void EndEndlessRun()
    {
        if (CurrentMode == GameMode.Endless)
        {
            // Ensure best score is up to date
            if (EndlessRunScore > EndlessBestScore)
            {
                EndlessBestScore = EndlessRunScore;
                PlayerPrefs.SetInt(EndlessBestScoreKey, EndlessBestScore);
                PlayerPrefs.Save();
            }
        }

        EndlessRunScore = 0;
        CurrentMode = GameMode.Normal;

        // Reset back to a fresh level and start screen
        StartGame();
        Debug.Log("Endless run ended, returning to start screen.");
    }

    public void NextLevel()
    {
        levelDesigner.GenerateLevel();
        PlayGame();
        Debug.Log("Next Level");
    }

    // Called when Quit button is clicked
    public void QuitGame()
    {
        Debug.Log("Quitting game...");
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            //this is the one that's working right now (when the menus aren't working)
        #else
            Application.Quit();
        #endif
    }
}