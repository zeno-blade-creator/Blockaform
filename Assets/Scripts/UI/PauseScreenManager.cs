using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class PauseScreenManager : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("The Canvas GameObject for the pause screen")]
    public GameObject pauseScreenCanvas;
    
    [Tooltip("The Resume button")]
    public Button resumeButton;
    
    [Tooltip("The Restart Level button")]
    public Button restartButton;
    
    [Tooltip("The Quit button")]
    public Button quitButton;

    void Start()
    {
        // Ensure canvas is hidden at start
        if (pauseScreenCanvas != null)
        {
            pauseScreenCanvas.SetActive(false);
        }

        // Connect buttons to their respective methods
        if (resumeButton != null)
        {
            resumeButton.onClick.AddListener(OnResumeButtonClicked);
        }

        if (restartButton != null)
        {
            restartButton.onClick.AddListener(OnRestartButtonClicked);
        }

        if (quitButton != null)
        {
            quitButton.onClick.AddListener(OnQuitButtonClicked);
        }
    }

    void Update()
    {
        // Handle ESC key to toggle pause
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (GameManager.Instance != null)
            {
                if (GameManager.Instance.CurrentState == GameState.Playing)
                {
                    // Pause the game
                    GameManager.Instance.PauseGame();
                    ShowPauseScreen();
                }
                else if (GameManager.Instance.CurrentState == GameState.Paused)
                {
                    // Resume the game
                    GameManager.Instance.PlayGame();
                    HidePauseScreen();
                }
            }
        }
    }

    void OnResumeButtonClicked()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.PlayGame();
            HidePauseScreen();
        }
    }

    void OnRestartButtonClicked()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.RestartGame();
            HidePauseScreen();
        }
    }

    void OnQuitButtonClicked()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.QuitGame();
        }
    }

    public void ShowPauseScreen()
    {
        if (pauseScreenCanvas != null)
        {
            pauseScreenCanvas.SetActive(true);
        }
    }

    public void HidePauseScreen()
    {
        if (pauseScreenCanvas != null)
        {
            pauseScreenCanvas.SetActive(false);
        }
    }
}
