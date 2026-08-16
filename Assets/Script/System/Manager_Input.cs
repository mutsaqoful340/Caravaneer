using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Manager_Input : MonoBehaviour
{
    public static Manager_Input Instance { get; private set; }
    public GameState currentGameState { get; private set; } = GameState.Gameplay;
    [SerializeField] private readonly List<PlayerInput> players = new();

    private void Awake()
    {
        Instance = this;
        SwitchMode(GameState.Gameplay); // Set the default action map to "Gameplay" for all players
    }

    public void RegisterPlayer(PlayerInput player)
    {
        if (player == null || players.Contains(player))
            return;

        players.Add(player);
    }

    public void UnregisterPlayer(PlayerInput player)
    {
        players.Remove(player);
    }

    public void SwitchMode(GameState newGameState)
    {
        currentGameState = newGameState;
        string actionMap = newGameState == GameState.UI ? "UI" : "Gameplay";

        if (players == null || players.Count <= 0)
        {
            Debug.LogWarning("No players registered to switch action maps."); return;
        }

        foreach (PlayerInput player in players)
        {
            if (player == null) continue;

            player.SwitchCurrentActionMap(actionMap);
            Debug.Log($"Switched {player.gameObject.name} to {actionMap} action map.");
        }
    }
}