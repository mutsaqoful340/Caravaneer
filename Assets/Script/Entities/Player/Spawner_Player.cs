using UnityEngine;
using UnityEngine.InputSystem;

public class Spawner_Player : MonoBehaviour
{
    [Header("Player Settings")]
    [SerializeField] private GameObject playerPrefab_1;
    [SerializeField] private GameObject playerPrefab_2;
    [SerializeField] private Transform spawnPointP1;
    [SerializeField] private Transform spawnPointP2;

    [Header("Additional References")]
    public GameObject folder;

    private void Start()
    {
        // Make sure a keyboard is plugged in
        if (Keyboard.current == null) return;

        if (folder == null)
        {
            Debug.LogWarning("Spawner_Player: 'folder' is not assigned. Players will be spawned at root level.");
        }

        // Instantiate Player 1 using the P1 control scheme
        var p1 = PlayerInput.Instantiate(
            playerPrefab_1,
            playerIndex: 0,
            controlScheme: "P1",
            pairWithDevice: Keyboard.current
        );
        var p1Component = p1.GetComponent<PlayerComponent>();
        if (p1Component != null)
        {
            p1Component.prevParent = folder != null ? folder : (p1.transform.parent != null ? p1.transform.parent.gameObject : p1.gameObject);
        }
        if (folder != null) p1.transform.SetParent(folder.transform, true);
        p1.transform.position = spawnPointP1.position;
        p1.gameObject.name = "Player_Mechanic";

        // Instantiate Player 2 using the P2 control scheme
        var p2 = PlayerInput.Instantiate(
            playerPrefab_2,
            playerIndex: 1,
            controlScheme: "P2",
            pairWithDevice: Keyboard.current
        );
        var p2Component = p2.GetComponent<PlayerComponent>();
        if (p2Component != null)
        {
            p2Component.prevParent = folder != null ? folder : (p2.transform.parent != null ? p2.transform.parent.gameObject : p2.gameObject);
        }
        if (folder != null) p2.transform.SetParent(folder.transform, true);
        p2.transform.position = spawnPointP2.position;
        p2.gameObject.name = "Player_Mercenary";
    }
}