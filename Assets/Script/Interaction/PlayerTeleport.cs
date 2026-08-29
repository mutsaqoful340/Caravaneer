using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerTeleport : MonoBehaviour
{
    private const int RequiredPlayerCount = 2;

    [Header("Teleportation References")]
    [Tooltip("The start points of the SOURCE line that players will teleport from.")]
    public Transform sourceLineStart;
    [Tooltip("The end points of the SOURCE line that players will teleport from.")]
    public Transform sourceLineEnd;
    [Tooltip("The start points of the DESTINATION line that players will teleport to.")]
    public Transform destinationLineStart;
    [Tooltip("The end points of the DESTINATION line that players will teleport to.")]
    public Transform destinationLineEnd;
    [Tooltip("The paired gate that should ignore the incoming player until they leave its trigger volume.")]
    public PlayerTeleport pairedGate;

    [Header("Teleportation Settings")]
    [Tooltip("The time in seconds to ignore incoming players after they leave the trigger volume.")]
    [SerializeField, Min(0f)] private float incomingIgnoreFallbackSeconds = 0.25f;
    [SerializeField, Min(0f)] private float exitNudgeDistance = 0.1f;
    [SerializeField, Min(0f)] private float exitNudgeDuration = 0.12f;
    [Tooltip("The delay in seconds before teleporting players after both enter the trigger volume.")]
    [SerializeField, Min(0f)] private float teleportDelay = 0.05f;
    [Tooltip("Fixed world direction used for exit nudge. Keep Y at 0 for top-down movement.")]
    [SerializeField] private Vector3 exitNudgeWorldDirection = Vector3.left;

    [Header("Unity Events")]
    public UnityEvent onDelayStarted;
    public UnityEvent onPlayersTeleported;


    [Header("Debug")]
    [SerializeField] private int playerCount = 0;
    [SerializeField] private PlayerComponent[] playerComponents = new PlayerComponent[RequiredPlayerCount];

    private readonly HashSet<int> playersInsideTrigger = new HashSet<int>();
    private readonly HashSet<int> playersPendingIgnore = new HashSet<int>();
    private readonly Dictionary<int, Coroutine> activeNudges = new Dictionary<int, Coroutine>();
    private readonly Dictionary<int, Vector3> pendingProjectionCompensation = new Dictionary<int, Vector3>();
    private readonly Dictionary<int, Collider> waitingPlayerColliders = new Dictionary<int, Collider>();
    private Coroutine pendingTeleport;

    private void OnTriggerEnter(Collider other)
    {
        PlayerComponent playerComponent = other.GetComponentInParent<PlayerComponent>();
        if (playerComponent == null)
        {
            return;
        }

        if (sourceLineStart == null || sourceLineEnd == null || destinationLineStart == null || destinationLineEnd == null)
        {
            Debug.LogWarning($"{gameObject.name} is missing source/destination line references.");
            return;
        }

        int playerId = playerComponent.gameObject.GetInstanceID();

        if (playersPendingIgnore.Contains(playerId))
        {
            playersPendingIgnore.Remove(playerId);
            playersInsideTrigger.Add(playerId);
            return;
        }

        if (playersInsideTrigger.Contains(playerId))
        {
            return;
        }

        if (!AddWaitingPlayer(playerComponent, other))
        {
            return;
        }

        if (playerCount == RequiredPlayerCount)
        {
            StartDelayedTeleport();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        PlayerComponent playerComponent = other.GetComponentInParent<PlayerComponent>();
        if (playerComponent == null)
        {
            return;
        }

        int playerId = playerComponent.gameObject.GetInstanceID();
        playersInsideTrigger.Remove(playerId);
        playersPendingIgnore.Remove(playerId);
        RemoveWaitingPlayer(playerComponent);

        if (playerCount < RequiredPlayerCount)
        {
            CancelPendingTeleport();
        }
    }

    private bool AddWaitingPlayer(PlayerComponent playerComponent, Collider playerCollider)
    {
        for (int i = 0; i < playerComponents.Length; i++)
        {
            if (playerComponents[i] == playerComponent)
            {
                return false;
            }
        }

        for (int i = 0; i < playerComponents.Length; i++)
        {
            if (playerComponents[i] != null)
            {
                continue;
            }

            playerComponents[i] = playerComponent;
            waitingPlayerColliders[playerComponent.gameObject.GetInstanceID()] = playerCollider;
            RefreshWaitingPlayerCount();
            return true;
        }

        return false;
    }

    private void RemoveWaitingPlayer(PlayerComponent playerComponent)
    {
        for (int i = 0; i < playerComponents.Length; i++)
        {
            if (playerComponents[i] != playerComponent)
            {
                continue;
            }

            playerComponents[i] = null;
            waitingPlayerColliders.Remove(playerComponent.gameObject.GetInstanceID());
            RefreshWaitingPlayerCount();
            return;
        }
    }

    private void StartDelayedTeleport()
    {
        if (pendingTeleport != null)
        {
            return;
        }

        if (teleportDelay <= 0f)
        {
            TeleportWaitingPlayers();
            return;
        }

        onDelayStarted?.Invoke();
        pendingTeleport = StartCoroutine(TeleportAfterDelay());
    }

    private IEnumerator TeleportAfterDelay()
    {
        yield return new WaitForSeconds(teleportDelay);
        pendingTeleport = null;

        if (playerCount == RequiredPlayerCount)
        {
            TeleportWaitingPlayers();
            onPlayersTeleported?.Invoke();
        }
    }

    private void CancelPendingTeleport()
    {
        if (pendingTeleport == null)
        {
            return;
        }

        StopCoroutine(pendingTeleport);
        pendingTeleport = null;
    }

    private void TeleportWaitingPlayers()
    {
        PlayerComponent[] waitingPlayers = new PlayerComponent[playerComponents.Length];
        playerComponents.CopyTo(waitingPlayers, 0);

        for (int i = 0; i < playerComponents.Length; i++)
        {
            playerComponents[i] = null;
        }

        RefreshWaitingPlayerCount();

        for (int i = 0; i < waitingPlayers.Length; i++)
        {
            PlayerComponent playerComponent = waitingPlayers[i];
            if (playerComponent == null)
            {
                continue;
            }

            int playerId = playerComponent.gameObject.GetInstanceID();
            waitingPlayerColliders.TryGetValue(playerId, out Collider playerCollider);
            waitingPlayerColliders.Remove(playerId);

            if (!TryTeleportPlayer(playerComponent.transform, playerCollider, playerId, out Vector3 appliedNudgeOffset))
            {
                continue;
            }

            if (pairedGate != null)
            {
                pairedGate.QueueIncomingPlayer(playerId, appliedNudgeOffset);
            }
        }
    }

    private void RefreshWaitingPlayerCount()
    {
        playerCount = 0;

        for (int i = 0; i < playerComponents.Length; i++)
        {
            if (playerComponents[i] != null)
            {
                playerCount++;
            }
        }
    }

    private void QueueIncomingPlayer(int playerId, Vector3 projectionCompensation)
    {
        playersPendingIgnore.Add(playerId);
        pendingProjectionCompensation[playerId] = projectionCompensation;
        StartCoroutine(ClearPendingIgnoreAfterDelay(playerId));
    }

    private IEnumerator ClearPendingIgnoreAfterDelay(int playerId)
    {
        if (incomingIgnoreFallbackSeconds > 0f)
        {
            yield return new WaitForSeconds(incomingIgnoreFallbackSeconds);
        }
        else
        {
            yield return null;
        }

        if (playersPendingIgnore.Contains(playerId) && !playersInsideTrigger.Contains(playerId))
        {
            playersPendingIgnore.Remove(playerId);
        }
    }

    private bool TryTeleportPlayer(Transform playerTransform, Collider playerCollider, int playerId, out Vector3 appliedNudgeOffset)
    {
        appliedNudgeOffset = Vector3.zero;

        Vector3 sourceStart = sourceLineStart.position;
        Vector3 sourceEnd = sourceLineEnd.position;
        Vector3 destinationStart = destinationLineStart.position;
        Vector3 destinationEnd = destinationLineEnd.position;

        Vector3 sourceDirection = sourceEnd - sourceStart;
        float sourceLengthSqr = sourceDirection.sqrMagnitude;
        if (sourceLengthSqr <= Mathf.Epsilon)
        {
            Debug.LogWarning($"{gameObject.name} source line is too short.");
            return false;
        }

        float estimatedT = Vector3.Dot(playerTransform.position - sourceStart, sourceDirection) / sourceLengthSqr;
        estimatedT = Mathf.Clamp01(estimatedT);

        Vector3 closestPointOnSourceLine = sourceStart + sourceDirection * estimatedT;
        Vector3 projectionPosition = playerCollider != null ? playerCollider.ClosestPoint(closestPointOnSourceLine) : playerTransform.position;

        if (pendingProjectionCompensation.TryGetValue(playerId, out Vector3 compensation))
        {
            projectionPosition -= compensation;
            pendingProjectionCompensation.Remove(playerId);
        }

        float projectedT = Vector3.Dot(projectionPosition - sourceStart, sourceDirection) / sourceLengthSqr;
        projectedT = Mathf.Clamp01(projectedT);

        Vector3 destinationPosition = Vector3.Lerp(destinationStart, destinationEnd, projectedT);

        Vector3 nudgeDirection = new Vector3(exitNudgeWorldDirection.x, 0f, exitNudgeWorldDirection.z);
        if (nudgeDirection.sqrMagnitude <= Mathf.Epsilon)
        {
            nudgeDirection = Vector3.left;
        }
        else
        {
            nudgeDirection.Normalize();
        }

        destinationPosition.y = playerTransform.position.y;

        Rigidbody rigidbody = playerTransform.GetComponent<Rigidbody>();
        if (rigidbody != null)
        {
            rigidbody.position = destinationPosition;
            rigidbody.rotation = playerTransform.rotation;
            rigidbody.linearVelocity = Vector3.zero;
            rigidbody.angularVelocity = Vector3.zero;
        }
        else
        {
            playerTransform.SetPositionAndRotation(destinationPosition, playerTransform.rotation);
        }

        appliedNudgeOffset = exitNudgeDistance > 0f ? nudgeDirection * exitNudgeDistance : Vector3.zero;
        StartExitNudge(playerTransform, rigidbody, appliedNudgeOffset);
        return true;
    }

    private void StartExitNudge(Transform playerTransform, Rigidbody rigidbody, Vector3 nudgeOffset)
    {
        if (nudgeOffset.sqrMagnitude <= Mathf.Epsilon || exitNudgeDuration <= 0f)
        {
            return;
        }

        int playerId = playerTransform.gameObject.GetInstanceID();
        if (activeNudges.TryGetValue(playerId, out Coroutine runningNudge) && runningNudge != null)
        {
            StopCoroutine(runningNudge);
        }

        activeNudges[playerId] = StartCoroutine(ApplyExitNudge(playerTransform, rigidbody, nudgeOffset, playerId));
    }

    private IEnumerator ApplyExitNudge(Transform playerTransform, Rigidbody rigidbody, Vector3 nudgeOffset, int playerId)
    {
        Vector3 startPosition = rigidbody != null ? rigidbody.position : playerTransform.position;
        Vector3 targetPosition = startPosition + nudgeOffset;

        float elapsed = 0f;
        while (elapsed < exitNudgeDuration)
        {
            if (playerTransform == null)
            {
                activeNudges.Remove(playerId);
                yield break;
            }

            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / exitNudgeDuration);
            float smoothT = t * t * (3f - 2f * t);
            Vector3 nudgedPosition = Vector3.Lerp(startPosition, targetPosition, smoothT);

            if (rigidbody != null)
            {
                rigidbody.position = nudgedPosition;
            }
            else
            {
                playerTransform.position = nudgedPosition;
            }

            yield return null;
        }

        if (playerTransform != null)
        {
            if (rigidbody != null)
            {
                rigidbody.position = targetPosition;
            }
            else
            {
                playerTransform.position = targetPosition;
            }
        }

        activeNudges.Remove(playerId);
    }

    private void OnDrawGizmos()
    {
        if (sourceLineStart != null && sourceLineEnd != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(sourceLineStart.position, sourceLineEnd.position);
            Gizmos.DrawSphere(sourceLineStart.position, 0.1f);
            Gizmos.DrawSphere(sourceLineEnd.position, 0.1f); 
        }

        if (destinationLineStart != null && destinationLineEnd != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(destinationLineStart.position, destinationLineEnd.position);
            Gizmos.DrawSphere(destinationLineStart.position, 0.1f);
            Gizmos.DrawSphere(destinationLineEnd.position, 0.1f);
        }
    }
}