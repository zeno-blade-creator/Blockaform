using UnityEngine;
using UnityEngine.UI;
using TMPro;

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

    [Tooltip("The End Run button (used in Endless mode instead of Restart)")]
    public Button endRunButton;

    [Header("Endless Mode UI")]
    [Tooltip("Text showing the current endless run score")]
    public TMP_Text currentRunScoreText;

    [Tooltip("Text showing the best endless score")]
    public TMP_Text bestEndlessScoreText;

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

        if (endRunButton != null)
        {
            endRunButton.onClick.RemoveAllListeners();
            endRunButton.onClick.AddListener(OnEndRunButtonClicked);
            endRunButton.gameObject.SetActive(false); // hidden by default; shown in endless mode
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

            if (shouldShow)
            {
                bool isEndless = GameManager.Instance.CurrentMode == GameMode.Endless;

                // In endless mode, hide Restart and show End Run.
                if (restartButton != null)
                {
                    restartButton.gameObject.SetActive(!isEndless);
                }

                if (endRunButton != null)
                {
                    endRunButton.gameObject.SetActive(isEndless);
                }

                // Update endless score UI if assigned
                if (isEndless)
                {
                    if (currentRunScoreText != null)
                    {
                        currentRunScoreText.text = "Run Score: " + GameManager.Instance.EndlessRunScore.ToString();
                        currentRunScoreText.gameObject.SetActive(true);
                    }

                    if (bestEndlessScoreText != null)
                    {
                        bestEndlessScoreText.text = "Best Endless: " + GameManager.Instance.EndlessBestScore.ToString();
                        bestEndlessScoreText.gameObject.SetActive(true);
                    }
                } else {
                    if (currentRunScoreText != null)
                    {
                        currentRunScoreText.gameObject.SetActive(false);
                    }

                    if (bestEndlessScoreText != null)
                    {
                        bestEndlessScoreText.gameObject.SetActive(false);
                    }
                }
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

    void OnEndRunButtonClicked()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.EndEndlessRun();
            HideEndScreen();
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
