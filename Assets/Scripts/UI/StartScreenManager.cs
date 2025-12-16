using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class StartScreenManager : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("The Canvas GameObject for the start screen")]
    public GameObject startScreenCanvas;
    
    [Tooltip("The Play button")]
    public Button playButton;

    void Start()
    {
        // Ensure canvas is visible at start
        if (startScreenCanvas != null)
        {
            ShowStartScreen();
        }
        else
        {
            Debug.LogError("StartScreenManager: startScreenCanvas is not assigned in the Inspector!");
        }

        // Connect Play button to PlayGame method
        if (playButton != null)
        {
            // Remove any existing listeners first to avoid duplicates
            playButton.onClick.RemoveAllListeners();
            playButton.onClick.AddListener(OnPlayButtonClicked);
            Debug.Log("StartScreenManager: Play button connected to OnPlayButtonClicked method");
            
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
        Debug.Log("OnPlayButtonClicked called!");
        
        if (GameManager.Instance != null)
        {
            Debug.Log("Play button clicked to play game. Current state: " + GameManager.Instance.CurrentState);
            GameManager.Instance.PlayGame();
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
    }

    public void HideStartScreen()
    {
        Debug.Log("Hiding start screen");
        if (startScreenCanvas != null)
        {
            startScreenCanvas.SetActive(false);
        }
    }
}
