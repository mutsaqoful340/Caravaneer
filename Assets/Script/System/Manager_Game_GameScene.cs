/// <summary>
/// This script is responsible for managing the game scene based on the selected game type.
/// It sets the current game scene in the Manager_Game singleton based on the GameType enum and then commit suicide.
/// </summary>
using UnityEngine;
public enum GameType
{
    MainMenu,
    Gameplay,
    VN
}

public class Manager_Game_GameScene : MonoBehaviour
{
    public GameType currentGameType = GameType.MainMenu;

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
            case GameType.VN:
                Manager_Game.Instance.currentGameScene = GameScene.MainMenu;
                break;
            case GameType.Gameplay:
                Manager_Game.Instance.currentGameScene = GameScene.Gameplay;
                break;
        }

        Destroy(this.gameObject); // Destroy this script's GameObject after setting the game scene
    }
}
