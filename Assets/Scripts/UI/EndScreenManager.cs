using UnityEngine;
using UnityEngine.UI;

public class EndScreenManager : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("The Canvas GameObject for the end screen")]
    public GameObject endScreenCanvas;
    
    [Tooltip("The Next Level button (for now, same as restart)")]
    public Button nextLevelButton;
    
    [Tooltip("The Restart Level button")]
    public Button restartButton;
    
    [Tooltip("The Quit button")]
    public Button quitButton;

    void Start()
    {
        // Ensure canvas is hidden at start
        if (endScreenCanvas != null)
        {
            endScreenCanvas.SetActive(false);
        }

        // Connect buttons to their respective methods
        if (nextLevelButton != null)
        {
            nextLevelButton.onClick.AddListener(OnNextLevelButtonClicked);
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
        // Update canvas visibility based on game state
        if (GameManager.Instance != null && endScreenCanvas != null)
        {
            bool shouldShow = (GameManager.Instance.CurrentState == GameState.Ended);
            if (endScreenCanvas.activeSelf != shouldShow)
            {
                endScreenCanvas.SetActive(shouldShow);
            }
        }
    }

    void OnNextLevelButtonClicked()
    {
        // For now, Next Level does the same as Restart
        // You can extend this later to load different levels
        if (GameManager.Instance != null)
        {
            GameManager.Instance.NextLevel();
        }
    }

    void OnRestartButtonClicked()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.RestartGame();
        }
    }

    void OnQuitButtonClicked()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.QuitGame();
        }
    }

    public void ShowEndScreen()
    {
        if (endScreenCanvas != null)
        {
            endScreenCanvas.SetActive(true);
        }
    }

    public void HideEndScreen()
    {
        if (endScreenCanvas != null)
        {
            endScreenCanvas.SetActive(false);
        }
    }
}
