using UnityEngine;
using UnityEngine.InputSystem;

public enum GameState
{
    UI,
    Gameplay
}

public class Manager_Game : MonoBehaviour
{
    public static Manager_Game Instance { get; private set; }
    public GameState currentGameState { get; set; }

    private Player_Input playerInput;

    // Subscribe to player input
    void OnEnable()
    {
        playerInput.Gameplay.Pause.performed += HandlePause;
        playerInput.Gameplay.Enable();
    }

    void OnDisable()
    {
        playerInput.Gameplay.Pause.performed -= HandlePause;
        playerInput.Gameplay.Disable();
    }

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
    }

    private void HandlePause(InputAction.CallbackContext context)
    {
        SetState(currentGameState == GameState.UI ? GameState.Gameplay : GameState.UI); 
    }
}
