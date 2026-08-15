using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Universal_Interact : MonoBehaviour
{
    [Header("Interaction Settings")]
    [Tooltip("The animator component for the interactable object.")]
    public Animator animator;
    [Tooltip("Determines if the object can be interacted with.")]
    public bool isInteractable = true;
    [Tooltip("Determines if the interaction requires holding the action button.")]
    public bool isHoldInteract;
    public float holdDuration = 2f; // Duration in seconds for hold interaction
    [Tooltip("If enabled, this object destroys itself after successful interaction.")]
    public bool destroyAfterInteract;
    [Tooltip("Delay before destroying this object after interaction.")]
    public float destroyDelay = 0f;
    public UnityEvent onInteract;

    [Header("Debug")]
    [Tooltip("DO NOT MANUALLY TICK - Indicates whether the object is currently being interacted with.")]
    public bool isInteracting = false;

    private Coroutine holdInteractionRoutine;
    private float interactionCurrentTime;

    public void Interact()
    {
        BeginInteraction();
    }

    public void BeginInteraction()
    {
        if (!isInteractable || isInteracting)
        {
            return;
        }

        isInteracting = true;
        interactionCurrentTime = 0f;

        if (isHoldInteract)
        {
            holdInteractionRoutine = StartCoroutine(HoldInteractionRoutine());
            Debug.Log($"{gameObject.name} is being held for interaction.");
            return;
        }

        onInteract.Invoke();
        TryDestroyAfterInteract();
        Debug.Log($"{gameObject.name} was interacted with.");
    }

    public void EndInteraction()
    {
        if (!isInteracting)
        {
            return;
        }

        if (holdInteractionRoutine != null)
        {
            StopCoroutine(holdInteractionRoutine);
            holdInteractionRoutine = null;
        }

        isInteracting = false;
        interactionCurrentTime = 0f;

        if (isHoldInteract)
        {
            Debug.Log($"{gameObject.name} hold interaction ended.");
        }
    }

    private IEnumerator HoldInteractionRoutine()
    {
        while (isInteracting && interactionCurrentTime < holdDuration)
        {
            interactionCurrentTime += Time.deltaTime;
            Debug.Log($"{gameObject.name} hold timer: {interactionCurrentTime:F2}s / {holdDuration:F2}s");
            yield return null;
        }

        if (!isInteracting || !isInteractable)
        {
            holdInteractionRoutine = null;
            yield break;
        }

        onInteract.Invoke();
        TryDestroyAfterInteract();
        Debug.Log($"{gameObject.name} hold interaction completed.");
        interactionCurrentTime = 0f;
        isInteracting = false;
        holdInteractionRoutine = null;
    }

    private void TryDestroyAfterInteract()
    {
        if (!destroyAfterInteract)
        {
            return;
        }

        isInteractable = false;
        float delay = Mathf.Max(0f, destroyDelay);
        Destroy(gameObject, delay);
    }
}