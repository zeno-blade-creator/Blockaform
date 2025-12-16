using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

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
    
    [Header("Debug")]
    [Tooltip("Enable manual click detection as fallback")]
    public bool useManualClickDetection = true;

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
        
        // Manual click detection fallback for pause screen buttons
        if (useManualClickDetection && pauseScreenCanvas != null && pauseScreenCanvas.activeSelf)
        {
            if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
            {
                CheckManualButtonClicks();
            }
        }
    }
    
    void CheckManualButtonClicks()
    {
        PointerEventData pointerData = new PointerEventData(EventSystem.current);
        pointerData.position = Mouse.current.position.ReadValue();
        
        var results = new System.Collections.Generic.List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);
        
        foreach (var result in results)
        {
            if (resumeButton != null && (result.gameObject == resumeButton.gameObject || 
                result.gameObject.transform.IsChildOf(resumeButton.transform)))
            {
                Debug.Log("Manual click detection: Resume button clicked!");
                OnResumeButtonClicked();
                break;
            }
            else if (restartButton != null && (result.gameObject == restartButton.gameObject || 
                result.gameObject.transform.IsChildOf(restartButton.transform)))
            {
                Debug.Log("Manual click detection: Restart button clicked!");
                OnRestartButtonClicked();
                break;
            }
            else if (quitButton != null && (result.gameObject == quitButton.gameObject || 
                result.gameObject.transform.IsChildOf(quitButton.transform)))
            {
                Debug.Log("Manual click detection: Quit button clicked!");
                OnQuitButtonClicked();
                break;
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
            GameManager.Instance.RestartLevel();
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
