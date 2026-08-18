using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraConstraint : MonoBehaviour
{
    [Header("Camera Reference & Settings")]
    public Camera mainCamera;
    public bool isFreeezeZ = false;
    public Vector3 cameraOffset = new Vector3(0, 10, -10);
    public Vector3 cameraDamping = new Vector3(0.1f, 0.1f, 0.1f);
    public bool isMainMenu = true;

    private float velocityX;
    private float velocityY;
    private float velocityZ;
    private float startingZ;

    [Header("Player References")]
    [SerializeField] private GameObject[] targets;

    public void Start()
    {
        Transform targetTransform = mainCamera != null ? mainCamera.transform : transform;
        startingZ = targetTransform.position.z;
    }

    private void LateUpdate()
    {
        if (isMainMenu) return;

        if (targets == null || targets.Length == 0) return;

        Vector3 targetCenter = Vector3.zero;
        int validTargetCount = 0;

        foreach (GameObject target in targets)
        {
            if (target == null) continue;

            targetCenter += target.transform.position;
            validTargetCount++;
        }

        if (validTargetCount == 0) return;

        Vector3 midpoint = targetCenter / validTargetCount;
        Transform targetTransform = mainCamera != null ? mainCamera.transform : transform;
        Vector3 targetPosition = midpoint + cameraOffset;
        if (isFreeezeZ)
        {
            targetPosition.z = startingZ;
        }

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

}