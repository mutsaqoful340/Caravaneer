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

    private void Start()
    {
        UpdateHUD();
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
    
    public bool TrySpendRepairMaterials(int amount)
    {
        if (amount <= 0 || repairMaterials < amount)
        {
            return false;
        }
        
        repairMaterials -= amount;
        UpdateHUD();
        return true;
    }

    public bool TrySpendCoins(int amount)
    {
        if (coins < amount)
        {
            return false;
        }
        
        coins -= amount;
        UpdateHUD();
        return true;
    }
    private void UpdateHUD()
    {
        if (Player_LocalInvenvory.Instance == null)
        {
            Debug.LogWarning("PlayerInventory: HUD references are not assigned.");
            return;
        }
        
        Player_LocalInvenvory.Instance.UpdateLocalInventory();
    }
}