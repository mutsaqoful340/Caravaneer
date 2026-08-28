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
    public UI_UnivConfirmPanel confirmPanel;

    public void OnClickItem()
    {
        confirmPanel.OnShow(
            "Use Item",
            $"Are you sure you want to use {itemData.itemName}?",
            () => UseItem(),
            () => Debug.Log("Item use canceled."),
            parentCanvas
        );
    }

    private void UseItem()
    {
        if (itemType == ItemTypeWagon.HPFunctional)
        {
            Spawner_Wagon.Instance.wagonHPFunctionalStart += itemModifierValue; // Example effect: Increase wagon HP by item price
            Debug.Log($"Used {itemData.itemName} to increase wagon HP by {itemModifierValue}.");
        }
        else if (itemType == ItemTypeWagon.HPBroken)
        {
            Spawner_Wagon.Instance.wagonHPBrokenStart += itemModifierValue; // Example effect: Increase broken wagon HP by item price
            Debug.Log($"Used {itemData.itemName} to increase broken wagon HP by {itemModifierValue}.");
        }
        else
        {
            Debug.LogWarning($"Item type {itemType} is not implemented.");
        }
        Destroy(gameObject);
    }
}