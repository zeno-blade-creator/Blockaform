using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

public class StartScreenManager : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("The Canvas GameObject for the start screen")]
    public GameObject startScreenCanvas;
    
    [Tooltip("The Play button for normal mode")]
    public Button playButton;

    [Tooltip("The Play button for endless mode")]
    public Button endlessButton;

    [Header("Endless Mode UI")]
    [Tooltip("Text element to show the best endless score")]
    public TMP_Text endlessBestScoreText;

    void Start()
    {

        // Connect Play button (normal mode)
        if (playButton != null)
        {
            // Remove any existing listeners first to avoid duplicates
            playButton.onClick.RemoveAllListeners();
            playButton.onClick.AddListener(OnPlayButtonClicked);
            Debug.Log("StartScreenManager: Play button (Normal) connected to OnPlayButtonClicked method");
            
            // Verify button is interactable
            if (!playButton.interactable)
            {
                Debug.LogWarning("StartScreenManager: Play button is not interactable! Enabling it now.");
                playButton.interactable = true;
            }
        }
        else
        {
            Debug.LogError("StartScreenManager: playButton is not assigned in the Inspector!");
        }

        // Connect Endless mode button
        if (endlessButton != null)
        {
            endlessButton.onClick.RemoveAllListeners();
            endlessButton.onClick.AddListener(OnEndlessButtonClicked);
            Debug.Log("StartScreenManager: Endless button connected to OnEndlessButtonClicked method");
        }
        else
        {
            Debug.LogWarning("StartScreenManager: endlessButton is not assigned in the Inspector (endless mode will only be startable via code).");
        }

        UpdateEndlessBestScoreUI();
    }

    void Update()
    {
        // Handle ESC key to quit from start screen
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (GameManager.Instance != null && GameManager.Instance.CurrentState == GameState.Start)
            {
                Debug.Log("ESC key pressed to quit game");
                GameManager.Instance.QuitGame();
            }
        }
        
        // Debug: Test button with Enter key (for troubleshooting)
        if (Keyboard.current != null && Keyboard.current.enterKey.wasPressedThisFrame)
        {
            if (GameManager.Instance != null && GameManager.Instance.CurrentState == GameState.Start)
            {
                Debug.Log("Enter key pressed - manually triggering PlayGame");
                OnPlayButtonClicked();
            }
        }
    }

    void OnPlayButtonClicked()
    {
        Debug.Log("OnPlayButtonClicked called (Normal mode)!");
        
        if (GameManager.Instance != null)
        {
            Debug.Log("Play button clicked to play NORMAL game. Current state: " + GameManager.Instance.CurrentState);
            GameManager.Instance.StartNormalRun();
            // Hide start screen when game starts
            if (startScreenCanvas != null)
            {
                HideStartScreen();
            }
        }
        else
        {
            Debug.LogError("StartScreenManager: GameManager.Instance is null! Make sure GameManager exists in the scene.");
        }
    }

    void OnEndlessButtonClicked()
    {
        Debug.Log("OnEndlessButtonClicked called (Endless mode)!");

        if (GameManager.Instance != null)
        {
            Debug.Log("Endless button clicked to play ENDLESS game. Current state: " + GameManager.Instance.CurrentState);
            GameManager.Instance.StartEndlessRun();
            // Hide start screen when game starts
            if (startScreenCanvas != null)
            {
                HideStartScreen();
            }
        }
        else
        {
            Debug.LogError("StartScreenManager: GameManager.Instance is null! Make sure GameManager exists in the scene.");
        }
    }

    // Public method to show/hide the start screen (useful for scene transitions)
    public void ShowStartScreen()
    {
        Debug.Log("Showing start screen");
        if (startScreenCanvas != null)
        {
            startScreenCanvas.SetActive(true);
        }
        UpdateEndlessBestScoreUI();
    }

    public void HideStartScreen()
    {
        Debug.Log("Hiding start screen");
        if (startScreenCanvas != null)
        {
            startScreenCanvas.SetActive(false);
        }
    }

    void UpdateEndlessBestScoreUI()
    {
        if (endlessBestScoreText != null && GameManager.Instance != null)
        {
            endlessBestScoreText.text = "Best Endless: " + GameManager.Instance.EndlessBestScore.ToString();
        }
    }
}
