using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Manager_Input : MonoBehaviour
{
    public static Manager_Input Instance { get; private set; }
    public GameState currentGameState { get; private set; }
    [SerializeField] private readonly List<PlayerInput> players = new();

    private void Awake()
    {
        Instance = this;
        SwitchAllToGameplay(); // Set the default action map to "Gameplay" for all players
    }

    public void RegisterPlayer(PlayerInput player)
    {
        if (player == null || players.Contains(player))
            return;

        players.Add(player);
        player.SwitchCurrentActionMap(currentGameState == GameState.UI ? "UI" : "Gameplay");
    }

    public void UnregisterPlayer(PlayerInput player)
    {
        players.Remove(player);
    }

    public void SwitchAllToUI()
    {
        currentGameState = GameState.UI;

        if (players == null || players.Count <= 0)
        {
            Debug.LogWarning("No players registered to switch action maps."); return;
        }

        foreach (PlayerInput player in players)
        {
            if (player == null) continue;

            player.SwitchCurrentActionMap("UI");
            Debug.Log($"Switched {player.gameObject.name} to UI action map.");
        }
    }

    public void SwitchAllToGameplay()
    {
        currentGameState = GameState.Gameplay;

        if (players == null || players.Count <= 0)
        {
            Debug.LogWarning("No players registered to switch action maps."); return;
        }

        foreach (PlayerInput player in players)
        {
            if (player == null) continue;

            player.SwitchCurrentActionMap("Gameplay");
            Debug.Log($"Switched {player.gameObject.name} to Gameplay action map.");
        }
    }
}