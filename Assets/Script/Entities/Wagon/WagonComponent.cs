using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum WagonState // Two-stage wagon state: Functional and Broken
{
    Functional,
    Broken
}

public class WagonComponent : MonoBehaviour
{
    [Header("Wagon Settings")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private int HPFunctional = 5;
    [SerializeField] private int HPBroken = 8;

    [Header("Wagon References")]
    public Animator animator;
    public PlayerComponent mechanic;
    public Transform slotMechanic;
    public bool isMechanicMounted = false;
    public PlayerComponent mercenary;
    public Transform slotMercenary;
    public bool isMercenaryMounted = false;

    private Universal_Interact interactComponent;

    void Awake()
    {
        interactComponent = GetComponent<Universal_Interact>();
        if (interactComponent != null) interactComponent.enabled = false;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (interactComponent != null)
            {
                interactComponent.enabled = true;
            }

            if (other.name.Contains("Player_Mechanic"))
            {
                mechanic = other.GetComponent<PlayerComponent>();
            }
            else if (other.name.Contains("Player_Mercenary"))
            {
                mercenary = other.GetComponent<PlayerComponent>();
            }
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (other.name.Contains("Player_Mechanic"))
            {
                mechanic = null;
            }
            else if (other.name.Contains("Player_Mercenary"))
            {
                mercenary = null;
            }

            if (interactComponent != null && mechanic == null && mercenary == null)
            {
                interactComponent.enabled = false;
            }
        }
    }

    public void OnPlayerMount()
    {
        PlayerComponent interactor = interactComponent.CurrentInteractor;

        if (interactor == null)
        {
            Debug.LogWarning($"{gameObject.name} was interacted with without a player interactor.");
        }
        else if (interactor == mechanic)
        {
            if (isMechanicMounted)
            {
                OnMechanicDismount();
            }
            else
            {
                OnMechanicMount();
            }
        }
        else if (interactor == mercenary)
        {
            if (isMercenaryMounted)
            {
                OnMercenaryDismount();
            }
            else
            {
                OnMercenaryMount();
            }
        }
        else
        {
            Debug.LogWarning($"{interactor.gameObject.name} is not assigned to this wagon.");
        }
    }

    public void OnTakeDamage(int damage)
    {
        HPBroken -= damage;
        Debug.Log($"{gameObject.name} took {damage} damage! Remaining HP: {HPBroken}");

        if (HPBroken <= 0)
        {
            OnWagonDestroyed();
        }
    }

    private void OnMechanicMount()
    {
        isMechanicMounted = true;
        CharacterController mechanicController = mechanic.GetComponent<CharacterController>();
        Rigidbody mechanicRigidbody = mechanic.GetComponent<Rigidbody>();
        GameObject prviousParent = mechanic.gameObject.transform.parent != null ? mechanic.gameObject.transform.parent.gameObject : null;
        if (mechanicController != null && mechanicRigidbody != null)
        {
            mechanicController.enabled = false;
            mechanicRigidbody.isKinematic = true; // Make the Rigidbody kinematic to prevent physics interactions
            mechanic.isMounted = true;
        }
        else
        {
            Debug.LogWarning($"{mechanic.gameObject.name} does not have a CharacterController component.");
        }
        // mechanicController.enabled = true; // Re-enable the CharacterController to reset its state
        mechanic.gameObject.transform.SetParent(slotMechanic);
        mechanic.gameObject.transform.localPosition = Vector3.zero;
    }

    private void OnMercenaryMount()
    {
        isMercenaryMounted = true;
        CharacterController mercenaryController = mercenary.GetComponent<CharacterController>();
        Rigidbody mercenaryRigidbody = mercenary.GetComponent<Rigidbody>();
        if (mercenaryController != null)
        {
            mercenaryController.enabled = false;
            mercenaryRigidbody.isKinematic = true; // Make the Rigidbody kinematic to prevent physics interactions
            mercenary.isMounted = true;
        }
        else
        {
            Debug.LogWarning($"{mercenary.gameObject.name} does not have a CharacterController component.");
        }
        mercenary.gameObject.transform.SetParent(slotMercenary);
        mercenary.gameObject.transform.localPosition = Vector3.zero;
    }

    private void OnMechanicDismount()
    {
        isMechanicMounted = false;
        CharacterController mechanicController = mechanic.GetComponent<CharacterController>();
        Rigidbody mechanicRigidbody = mechanic.GetComponent<Rigidbody>();
        if (mechanicController != null)
        {
            mechanicController.enabled = true;
            mechanicRigidbody.isKinematic = false; // Make the Rigidbody non-kinematic to allow physics interactions
            mechanic.isMounted = false;
        }
        else
        {
            Debug.LogWarning($"{mechanic.gameObject.name} does not have a CharacterController component.");
        }

        if (mechanic.prevParent != null)
        {
            mechanic.gameObject.transform.SetParent(mechanic.prevParent.transform, true);
        }
        else
        {
            mechanic.gameObject.transform.SetParent(null);
        }
    }

    private void OnMercenaryDismount()
    {
        isMercenaryMounted = false;
        CharacterController mercenaryController = mercenary.GetComponent<CharacterController>();
        Rigidbody mercenaryRigidbody = mercenary.GetComponent<Rigidbody>();
        if (mercenaryController != null)
        {
            mercenaryController.enabled = true;
            mercenaryRigidbody.isKinematic = false; // Make the Rigidbody non-kinematic to allow physics interactions
            mercenary.isMounted = false;
        }
        else
        {
            Debug.LogWarning($"{mercenary.gameObject.name} does not have a CharacterController component.");
        }

        if (mercenary.prevParent != null)
        {
            mercenary.gameObject.transform.SetParent(mercenary.prevParent.transform, true);
        }
        else
        {
            mercenary.gameObject.transform.SetParent(null);
        }
    }

    public void ApplyDriverInput(Vector2 input)
    {
        float x = Mathf.Max(0f, input.x);
        float z = input.y;

        float maxLateralOffset = x * 0.75f;
        z = Mathf.Clamp(z, -maxLateralOffset, maxLateralOffset);

        if (x <= 0f)
        {
            z = 0f;
        }

        Vector3 movement = new Vector3(x, 0f, z) * moveSpeed * Time.deltaTime;
        transform.position += movement;
    }

    private void OnWagonDestroyed()
    {
        // TODO - add wagon destruction logic here (e.g., play destruction animation, disable wagon, etc.)
        Debug.Log($"{gameObject.name} has been destroyed!");
        animator.SetTrigger("Destroy");
    }

    private void DestroyWagon()
    {
        Destroy(gameObject);
    }

    private void Update()
    {
        if (!isMechanicMounted || mechanic == null)
        {
            return;
        }

        ApplyDriverInput(mechanic.GetMoveInput());
    }
}