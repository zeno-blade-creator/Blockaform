using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

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
    
    [Header("Debug")]
    [Tooltip("Enable manual click detection as fallback")]
    public bool useManualClickDetection = true;

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
        
        // Manual click detection fallback for end screen buttons
        if (useManualClickDetection && endScreenCanvas != null && endScreenCanvas.activeSelf)
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
            if (nextLevelButton != null && (result.gameObject == nextLevelButton.gameObject || 
                result.gameObject.transform.IsChildOf(nextLevelButton.transform)))
            {
                Debug.Log("Manual click detection: Next Level button clicked!");
                OnNextLevelButtonClicked();
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

    void OnNextLevelButtonClicked()
    {
        // For now, Next Level does the same as Restart
        // You can extend this later to load different levels
        if (GameManager.Instance != null)
        {
            GameManager.Instance.RestartLevel();
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
