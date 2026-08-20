/// <summary>
/// This script would work as a collective inventory for both players.
/// Will store items, repair materials, money, and other relevant things.
/// </summary>

using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class PlayerInventory : MonoBehaviour
{
    public static PlayerInventory Instance { get; private set; }
    [Header("Inventory Data")]
    public int coins;
    public int repairMaterials;

    [Header("HUD References")]
    public TextMeshProUGUI textCoins;
    public TextMeshProUGUI textRepairMaterials;

    [Header("Other Inventory References")]
    public Animator animator;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void AddCoins(int amount)
    {
        coins += amount;
        UpdateHUD();
    }

    public void AddRepairMaterials(int amount)
    {
        repairMaterials += amount;
        UpdateHUD();
    }

    private void UpdateHUD()
    {
        if (!textCoins || !textRepairMaterials)
        {
            Debug.LogWarning("PlayerInventory: HUD references are not assigned.");
            return;
        }
        
        textCoins.text = $"Coins: {coins}";
        textRepairMaterials.text = $"Repair Materials: {repairMaterials}";
    }
}