using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class Item : MonoBehaviour
{
    public ItemData itemData;
    public UI_UnivConfirmPanel confirmPanel;

    public void OnClickItem()
    {
        confirmPanel.OnShow(
            "Use Item",
            $"Are you sure you want to use {itemData.itemName}?",
            () => UseItem(),
            () => Debug.Log("Item use canceled.")
        );
    }

    private void UseItem()
    {
        Debug.Log($"Using item: {itemData.itemName}");
        Destroy(gameObject);
    }
}
