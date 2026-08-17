using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class Item : MonoBehaviour
{
    public CanvasGroup parentCanvas;
    public ItemData itemData;
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
        Debug.Log($"Destroying: {itemData.itemName}");
        Destroy(gameObject);
    }
}
