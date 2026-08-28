using UnityEngine;

public class Item_Player : MonoBehaviour
{
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
        Spawner_Player.Instance.playerMechHPStart += itemModifierValue; // Example effect: Increase wagon HP by item price
        Spawner_Player.Instance.playerMercHPStart += itemModifierValue; // Example effect: Increase wagon HP by item price
        Debug.Log($"Used {itemData.itemName} to increase wagon HP by {itemModifierValue}.");

        Destroy(gameObject);
    }
}