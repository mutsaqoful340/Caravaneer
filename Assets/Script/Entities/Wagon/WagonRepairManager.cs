/// <summary>
/// This script manages the repair functionality for the wagon.
/// </summary>
using UnityEngine;

public class WagonRepairManager : MonoBehaviour
{
    public float repairRadius = 5f; // The radius within which the wagon can be repaired
    public float repairInterval = 1f; // The interval at which the the material will be consumed for repair
    public WagonComponent wagonComponent; // Reference to the wagon component
    [Tooltip("The prefab for the visual representation of the repair material that can be collected.")]
    public GameObject repairMaterialVisualPrefab; // The prefab for the visual of repair material that can be collected
    
}
