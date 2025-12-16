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
            startScreenCanvas.SetActive(true);
        }

        // Connect Play button to StartGame method
        if (playButton != null)
        {
            playButton.onClick.AddListener(OnPlayButtonClicked);
        }
    }

    void Update()
    {
        // Handle ESC key to quit from start screen
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (GameManager.Instance != null && GameManager.Instance.CurrentState == GameState.Start)
            {
                GameManager.Instance.QuitGame();
            }
        }
    }

    void OnPlayButtonClicked()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.StartGame();
            // Hide start screen when game starts
            if (startScreenCanvas != null)
            {
                startScreenCanvas.SetActive(false);
            }
        }
    }

    // Public method to show/hide the start screen (useful for scene transitions)
    public void ShowStartScreen()
    {
        if (startScreenCanvas != null)
        {
            startScreenCanvas.SetActive(true);
        }
    }

    public void HideStartScreen()
    {
        if (startScreenCanvas != null)
        {
            startScreenCanvas.SetActive(false);
        }
    }
}
