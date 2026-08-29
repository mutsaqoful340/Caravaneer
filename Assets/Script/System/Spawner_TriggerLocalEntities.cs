using UnityEngine;

public class Spawner_TriggerLocalEntities : MonoBehaviour
{
    public bool isMainMenu = false;
    public Transform spawnPoint_Mechanics;
    public Transform spawnPoint_Mercenary;
    public Transform spawnPoint_Wagon;
    public GameObject folder;

    void Start()
    {
        if (!isMainMenu)
        {
            Spawner_Wagon.Instance.SpawnWagon(spawnPoint_Wagon, folder);
        }
        Spawner_Player.Instance.SpawnPlayer(spawnPoint_Mechanics, spawnPoint_Mercenary, folder);
    }
}