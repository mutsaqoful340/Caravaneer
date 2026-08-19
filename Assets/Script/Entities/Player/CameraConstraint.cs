using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraConstraint : MonoBehaviour
{
    public static CameraConstraint Instance { get; private set; }
    [Header("Camera Reference & Settings")]
    public GameObject mainCamera;
    public bool isFreeezeZ = false;
    public Vector3 cameraOffset = new Vector3(0, 10, -10);
    public Vector3 cameraDamping = new Vector3(0.1f, 0.1f, 0.1f);
    public bool isMainMenu = true;
    [Header("Camera Shake")]
    [Min(0f)] public float shakeDuration = 0.15f;
    [Min(0f)] public float shakeMagnitude = 0.05f;
    [Min(0.1f)] public float shakeFrequency = 25f;
    public Vector3 shakeAxisMultiplier = new Vector3(1.5f, 0.5f, 0f);

    private float velocityX;
    private float velocityY;
    private float velocityZ;
    private float startingZ;
    private Vector3 shakeOffset;
    private Coroutine shakeCoroutine;

    [Header("Player References")]
    [SerializeField] private GameObject[] targets;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    public void Start()
    {
        Transform targetTransform = mainCamera != null ? mainCamera.transform : transform;
        startingZ = targetTransform.position.z;
    }

    private void LateUpdate()
    {
        Transform targetTransform = mainCamera != null ? mainCamera.transform : transform;
        Vector3 currentPosition = targetTransform.position - shakeOffset;

        if (!isMainMenu && targets != null && targets.Length > 0)
        {
            Vector3 targetCenter = Vector3.zero;
            int validTargetCount = 0;

            foreach (GameObject target in targets)
            {
                if (target == null) continue;

                targetCenter += target.transform.position;
                validTargetCount++;
            }

            if (validTargetCount > 0)
            {
                Vector3 midpoint = targetCenter / validTargetCount;
                Vector3 targetPosition = midpoint + cameraOffset;
                if (isFreeezeZ)
                {
                    targetPosition.z = startingZ;
                }

                float smoothX = Mathf.Max(0.0001f, cameraDamping.x);
                float smoothY = Mathf.Max(0.0001f, cameraDamping.y);
                float smoothZ = Mathf.Max(0.0001f, cameraDamping.z);

                currentPosition = new Vector3(
                    Mathf.SmoothDamp(currentPosition.x, targetPosition.x, ref velocityX, smoothX),
                    Mathf.SmoothDamp(currentPosition.y, targetPosition.y, ref velocityY, smoothY),
                    Mathf.SmoothDamp(currentPosition.z, targetPosition.z, ref velocityZ, smoothZ)
                );
            }
        }

        targetTransform.position = currentPosition + shakeOffset;
    }

    public void CameraShake()
    {
        CameraShake(shakeDuration, shakeMagnitude);
    }

    public void CameraShake(float duration, float magnitude)
    {
        if (duration <= 0f || magnitude <= 0f) return;

        if (shakeCoroutine != null)
        {
            StopCoroutine(shakeCoroutine);
        }

        shakeCoroutine = StartCoroutine(Shake(duration, magnitude));
    }

    private IEnumerator Shake(float duration, float magnitude)
    {
        float elapsed = 0f;
        float sampleTimer = 0f;
        float sampleInterval = 1f / Mathf.Max(0.1f, shakeFrequency);
        Vector3 currentOffset = Vector3.zero;
        Vector3 targetOffset = Random.insideUnitCircle;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            sampleTimer += Time.deltaTime;

            if (sampleTimer >= sampleInterval)
            {
                sampleTimer -= sampleInterval;
                targetOffset = Random.insideUnitCircle;
            }

            float interpolation = 1f - Mathf.Exp(-shakeFrequency * Time.deltaTime);
            currentOffset = Vector3.Lerp(currentOffset, targetOffset, interpolation);
            currentOffset = Vector3.Scale(currentOffset, shakeAxisMultiplier);
            float strength = 1f - Mathf.Clamp01(elapsed / duration);
            shakeOffset = currentOffset * (magnitude * strength);

            yield return null;
        }

        shakeOffset = Vector3.zero;
        shakeCoroutine = null;
    }
}