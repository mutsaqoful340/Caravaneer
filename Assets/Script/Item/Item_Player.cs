using UnityEngine;

public class Item_Player : MonoBehaviour
{
    public CanvasGroup parentCanvas;
    public ItemData itemData;
    public int itemModifierValue; // This can be used to modify the item's effect, e.g., amount of HP restored
    public UI_UnivConfirmPanel confirmPanel;

    [Header("Debug")]
    private bool isPurchased = false;

    public void OnClickItem()
    {
        confirmPanel.OnShow(
            $"Buy {itemData.itemName}.",
            $"Are you sure you want to use {itemData.itemName}?",
            () => UseItem(),
            () => Debug.Log("Item use canceled."),
            parentCanvas
        );
    }

    private void UseItem()
    {
        if (PlayerInventory.Instance == null)
        {
            Debug.LogError("PlayerInventory instance is null. Cannot proceed with buying the item.");
            return;
        }

        if (isPurchased) return; // Prevent multiple purchases

        if (PlayerInventory.Instance.TrySpendCoins(itemData.itemPrice))
        {
            Spawner_Player.Instance.playerMechHPStart += itemModifierValue; // Example effect: Increase wagon HP by item price
            Spawner_Player.Instance.playerMercHPStart += itemModifierValue; // Example effect: Increase wagon HP by item price
            Destroy(gameObject);
            Debug.Log($"Purchased {itemData.itemID} {itemData.itemName} to increase wagon HP by {itemModifierValue}.");
        }
        else
        {
            Debug.LogWarning("Not enough coins to buy this item.");
        }
    }
}