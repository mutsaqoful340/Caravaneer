using UnityEngine;
public enum GameType
{
    MainMenu,
    Gameplay
}

public class Manager_Game_GameScene : MonoBehaviour
{
    public GameType currentGameType = GameType.MainMenu;

    [Header("Debug")]
    public bool isGameplayScene = false; // Flag to indicate if the current scene is the gameplay scene

    private void Start()
    {
        if (Manager_Game.Instance == null)
        {
            Debug.LogError("Manager_Game instance is null. Ensure that the Manager_Game script is attached to a GameObject in the scene.");
            return;
        }

        switch (currentGameType)
        {
            case GameType.MainMenu:
                Manager_Game.Instance.currentGameScene = GameScene.MainMenu;
                break;
            case GameType.Gameplay:
                Manager_Game.Instance.currentGameScene = GameScene.Gameplay;
                break;
        }

        Destroy(this.gameObject); // Destroy this script's GameObject after setting the game scene
    }
}
