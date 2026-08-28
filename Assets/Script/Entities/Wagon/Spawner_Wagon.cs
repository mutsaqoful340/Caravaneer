using UnityEngine;

public class Spawner_Wagon : MonoBehaviour
{
    public static Spawner_Wagon Instance { get; private set; }
    [Header("Wagon Settings")]
    public int wagonHPFunctionalStart = 5; // Default/pre-buff starting HP for functional wagons
    public int wagonHPBrokenStart = 6; // Default/pre-buff starting HP for broken wagons
    public GameObject wagonPrefab; // The wagon prefab to spawn

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

    public void SpawnWagon(Transform wagonSpawnPoint, GameObject folder)
    {
        if (wagonPrefab != null && wagonSpawnPoint != null)
        {
            GameObject wagonObject = Instantiate(wagonPrefab, wagonSpawnPoint.position, wagonSpawnPoint.rotation);
            if (folder != null)
            {
                wagonObject.transform.SetParent(folder.transform, true);
            }
            WagonComponent wagonComponent = wagonObject.GetComponent<WagonComponent>();
            wagonComponent?.InitializeStartingHP(wagonHPFunctionalStart, wagonHPBrokenStart);
        }
        else
        {
            Debug.LogWarning("Wagon prefab or spawn point is not assigned.");
        }
    }
}