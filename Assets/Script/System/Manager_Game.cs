using UnityEngine;
using UnityEngine.InputSystem;

[System.Serializable]
public enum GameState
{
    UI,
    Gameplay
}

public class Manager_Game : MonoBehaviour
{
    public static Manager_Game Instance { get; private set; }
    public Manager_Input inputManager;
    public GameState currentGameState = GameState.Gameplay;

    private Player_Input playerInput;

    private void Awake()
    {
        playerInput = new Player_Input();

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
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
        OnGameStateChanged(newState);
    }

    private void OnGameStateChanged(GameState newState)
    {
        switch (newState)
        {
            case GameState.UI:
                inputManager.SwitchAllToUI();
                break;
            case GameState.Gameplay:
                inputManager.SwitchAllToGameplay();
                break;
        }
    }
}
