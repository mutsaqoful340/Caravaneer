using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerConstraint : MonoBehaviour
{
    [Header("Camera Reference & Settings")]
    public Camera mainCamera;
    public Vector3 cameraOffset = new Vector3(0, 10, -10);
    public Vector3 cameraDamping = new Vector3(0.1f, 0.1f, 0.1f);
    public bool isMainMenu = true;

    private float velocityX;
    private float velocityY;
    private float velocityZ;

    [Header("Player References")]
    [SerializeField] private GameObject player1;
    [SerializeField] private GameObject player2;

    public void Start()
    {

    }

    private void LateUpdate()
    {
        if (isMainMenu) return;

        if (player1 == null)
        {
            player1 = FindPlayer();
            Debug.Log("Player 1 found: " + player1?.name);
        }

        if (player2 == null)
        {
            player2 = FindPlayer(player1);
            Debug.Log("Player 2 found: " + player2?.name);
        }
        
        if (player1 == null || player2 == null) return;

        Vector3 midpoint = (player1.transform.position + player2.transform.position) / 2f;
        Transform targetTransform = mainCamera != null ? mainCamera.transform : transform;
        Vector3 targetPosition = midpoint + cameraOffset;
        // targetPosition.z = targetTransform.position.z;

        Vector3 currentPosition = targetTransform.position;
        float smoothX = Mathf.Max(0.0001f, cameraDamping.x);
        float smoothY = Mathf.Max(0.0001f, cameraDamping.y);
        float smoothZ = Mathf.Max(0.0001f, cameraDamping.z);

        targetTransform.position = new Vector3(
            Mathf.SmoothDamp(currentPosition.x, targetPosition.x, ref velocityX, smoothX),
            Mathf.SmoothDamp(currentPosition.y, targetPosition.y, ref velocityY, smoothY),
            Mathf.SmoothDamp(currentPosition.z, targetPosition.z, ref velocityZ, smoothZ)
        );
    }

    private GameObject FindPlayer(GameObject excluded = null)
    {
        foreach (PlayerComponent player in FindObjectsByType<PlayerComponent>())
        {
            if (player.gameObject != excluded)
            {
                return player.gameObject;
            }
        }

        return null;
    }
}