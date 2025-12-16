using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState { Start, Playing, Paused, Ended }

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public GameState CurrentState { get; private set; }

    public LevelDesigner levelDesigner;

    void Awake()
    {
        // Singleton pattern - ensure only one GameManager exists
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persist across scene reloads
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

    private void StartGame()
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

        levelDesigner.GenerateLevel();
        CurrentState = GameState.Start;
        Time.timeScale = 0f; // Pause the game initially for start screen
    }

    // Called when Play button is clicked or ESC is pressed while paused
    public void PlayGame()
    {
        CurrentState = GameState.Playing;
        Time.timeScale = 1f; // Resume normal time
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

    // Called when player reaches the goal flag
    public void EndGame()
    {
        CurrentState = GameState.Ended;
        Debug.Log("Game Ended - Player reached goal! Current state: " + CurrentState);
        Time.timeScale = 0f; // Freeze time
        Debug.Log("Game Ended - Player reached goal!");
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