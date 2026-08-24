using UnityEngine;

public class PlayerReviveManager : MonoBehaviour
{
    private bool isReviving = false;
    private PlayerComponent targetPlayer;

    private void Start()
    {
        targetPlayer = GetComponentInParent<PlayerComponent>();
    }

    public void OnRevivePlayer()
    {
        if (targetPlayer != null && !isReviving)
        {
            isReviving = true;
            targetPlayer.OnRevive();
            Debug.Log($"{gameObject.name} has revived {targetPlayer.gameObject.name}!");

            Destroy(gameObject); // Destroy the revive manager after reviving the player
        }
    }

    private void LateUpdate()
    {
        this.transform.parent = null; // Detach from parent to prevent rotation issues
    }
}