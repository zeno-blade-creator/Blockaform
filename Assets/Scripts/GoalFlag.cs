using UnityEngine;

public class GoalFlag : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("Tag name for the player GameObject (default: 'Player')")]
    public string playerTag = "Player";
    
    private bool hasBeenTriggered = false;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag) && !hasBeenTriggered)
        {
            hasBeenTriggered = true;
            
            // Trigger end game
            if (GameManager.Instance != null)
            {
                GameManager.Instance.EndGame();
                Debug.Log("Player reached the goal flag!");
            }
            else
            {
                Debug.LogError("GoalFlag: GameManager.Instance is null! Make sure GameManager exists in the scene.");
            }
        }
    }
}
