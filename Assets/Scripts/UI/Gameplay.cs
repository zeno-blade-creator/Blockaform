using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameplayUI : MonoBehaviour
{
    public static GameplayUI Instance;
    [Header("UI References")]
    [Tooltip("The Canvas GameObject for the gameplay UI")]
    public GameObject gameplayCanvas;

    [Tooltip("The Double Jump text")]
    public TMP_Text doubleJumpText;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    
    void Start()
    {
        // Ensure canvas is hidden at start
        if (gameplayCanvas != null)
        {
            gameplayCanvas.SetActive(false);
        }
    }


    public void ShowGameplayUI()
    {
        if (gameplayCanvas != null)
        {
            gameplayCanvas.SetActive(true);
        }
    }

    public void HideGameplayUI()
    {
        if (gameplayCanvas != null)
        {
            gameplayCanvas.SetActive(false);
        }
    }

    public void UpdateDoubleJumpUI(int doubleJumpAmount)
    {
        if (doubleJumpText != null)
        {
            doubleJumpText.text = "Double Jumps: " + doubleJumpAmount.ToString();
        }
    }
}