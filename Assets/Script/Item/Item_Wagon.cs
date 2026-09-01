using UnityEngine;

public enum ItemTypeWagon
{
    HPFunctional,
    HPBroken
}

public class Item_Wagon : MonoBehaviour
{
    public ItemTypeWagon itemType;
    public CanvasGroup parentCanvas;
    public ItemData itemData;
    public int itemModifierValue; // This can be used to modify the item's effect, e.g., amount of HP restored
    
    [Header("Debug")]
    private bool isPurchased = false;

    public void OnClickItem()
    {
        UI_UnivConfirmPanel.Instance.OnShow(
            $"Buy {itemData.itemName}.",
            $"Are you sure you want to use {itemData.itemName}?",
            () => BuyItem(),
            () => Debug.Log("Item use canceled."),
            parentCanvas
        );
    }

    private void BuyItem()
    {
        if (PlayerInventory.Instance == null)
        {
            Debug.LogError("PlayerInventory instance is null. Cannot proceed with buying the item.");
            return;
        }

        if (isPurchased) return; // Prevent multiple purchases

        if (itemType == ItemTypeWagon.HPFunctional)
        {
            if (PlayerInventory.Instance.TrySpendCoins(itemData.itemPrice))
            {
                Spawner_Wagon.Instance.wagonHPFunctionalStart += itemModifierValue; // Example effect: Increase wagon HP by item price
                Debug.Log($"Purchased {itemData.itemID} {itemData.itemName} to increase wagon HP by {itemModifierValue}.");
                Destroy(gameObject);
            }
            else
            {
                Debug.LogWarning("Not enough coins to buy this item.");
            }
        }
        else if (itemType == ItemTypeWagon.HPBroken)
        {
            if (PlayerInventory.Instance.TrySpendCoins(itemData.itemPrice))
            {
                Spawner_Wagon.Instance.wagonHPBrokenStart += itemModifierValue; // Example effect: Increase wagon HP by item price
                Debug.Log($"Used {itemData.itemID} {itemData.itemName} to increase wagon HP by {itemModifierValue}.");
                Destroy(gameObject);
            }
            else
            {
                Debug.LogWarning("Not enough coins to buy this item.");
            }
        }
    }
}