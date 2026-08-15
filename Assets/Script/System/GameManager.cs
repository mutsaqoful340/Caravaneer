using UnityEngine;

public enum GameState
{
    UI,
    Gameplay,
    Paused,
    GameOver
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public GameState currentGameState { get; private set; }

    private void Awake()
    {
        if (Instance == null){
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else{
            Destroy(gameObject);
            return;
        }

        QualitySettings.vSyncCount = 0; // Disable VSync
        Application.targetFrameRate = 60;
    }

    public void SetState(GameState newState)
    {
        if (currentGameState == newState)
            return;

        currentGameState = newState;
    }

    public bool IsGameplay => currentGameState == GameState.Gameplay;

    public bool IsUI => currentGameState == GameState.UI;
}
