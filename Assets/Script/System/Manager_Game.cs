using UnityEngine;
using UnityEngine.InputSystem;

[System.Serializable]
public enum GameState
{
    UI,
    Gameplay
}

public enum GameScene
{
    MainMenu,
    Gameplay
}

public class Manager_Game : MonoBehaviour
{
    public static Manager_Game Instance { get; private set; }
    public Manager_Input inputManager;
    [Tooltip("Manages player input modes.")]
    public GameState currentGameState = GameState.Gameplay;
    [Tooltip("Manages the current game scene.")]
    public GameScene currentGameScene = GameScene.MainMenu;

    private void Awake()
    {
        currentGameState = GameState.Gameplay;
        // currentGameScene = GameScene.MainMenu;

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
                inputManager.SwitchMode(GameState.UI);
                break;
            case GameState.Gameplay:
                inputManager.SwitchMode(GameState.Gameplay);
                break;
        }
    }

    public void SetScene(GameScene newScene)
    {
        if (currentGameScene == newScene)
            return;

        currentGameScene = newScene;
        OnGameSceneChanged(newScene);
    }

    private void OnGameSceneChanged(GameScene newScene)
    {
        switch (newScene)
        {
            case GameScene.MainMenu:
                // Handle main menu scene logic
                break;
            case GameScene.Gameplay:
                // Handle gameplay scene logic
                break;
        }
        
    }
}
