using UnityEngine;

public enum GameState { Start, Playing, Paused, Ended }
public class GameManager : MonoBehaviour {
    public static GameManager Instance;
    public GameState CurrentState { get; private set; }

    bool GameStart() {
        CurrentState = GameState.Playing;
        Debug.Log("Game Started");
        return true;
    }
    bool GamePause() {
        CurrentState = GameState.Paused;
        Debug.Log("Game Paused");
        return true;
    }
    bool GameResume() {
        CurrentState = GameState.Playing;
        Debug.Log("Game Resumed");
        return true;
    }
    bool GameRestart() {
        CurrentState = GameState.Playing;
        Debug.Log("Game Restarted");
        return true;
    }
    bool GameEnd() {
        CurrentState = GameState.Ended;
        Debug.Log("Game Ended");
        return true;
    }
    // State transition methods
}