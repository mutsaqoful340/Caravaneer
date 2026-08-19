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
    [SerializeField] private int startHPFunctional = 5;
    [SerializeField] private int startHPBroken = 8;
    public WagonState currentWagonState = WagonState.Functional;

    [Header("Wagon References")]
    public Animator animator;
    public Canvas wagonHPCanvas;
    public GameObject heartFunctionalPrefab;
    public GameObject heartBrokenPrefab;

    [Header("Player References")]
    public PlayerComponent mechanic;
    public Transform slotMechanic;
    public bool isMechanicMounted = false;
    public PlayerComponent mercenary;
    public Transform slotMercenary;
    public bool isMercenaryMounted = false;

    [Header("Debug")]
    [SerializeField] private int currHPBroken = 8;
    [SerializeField] private int currHPFunctional = 5;
    [SerializeField] private bool isBroken = false;
    [SerializeField] private bool isDestroyed = false;
    [SerializeField] private CharacterController mechanicCC;
    [SerializeField] private Rigidbody mechanicRB;
    [SerializeField] private CharacterController mercenaryCC;
    [SerializeField] private Rigidbody mercenaryRB;

    private Universal_Interact interactComponent;

    void Awake()
    {
        interactComponent = GetComponent<Universal_Interact>();
        if (interactComponent != null) interactComponent.enabled = false;
        if (animator == null) animator = GetComponent<Animator>();
        currHPFunctional = startHPFunctional;
        currHPBroken = startHPBroken;
        // UpdateWagonStateAnimation();
        UpdateHPUI();
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
                mechanicCC = mechanic.GetComponent<CharacterController>();
                mechanicRB = mechanic.GetComponent<Rigidbody>();
            }
            else if (other.name.Contains("Player_Mercenary"))
            {
                mercenary = other.GetComponent<PlayerComponent>();
                mercenaryCC = mercenary.GetComponent<CharacterController>();
                mercenaryRB = mercenary.GetComponent<Rigidbody>();
            }
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (other.name.Contains("Player_Mechanic") && !isMechanicMounted)
            {
                mechanic = null;
                mechanicCC = null;
                mechanicRB = null;
            }
            else if (other.name.Contains("Player_Mercenary") && !isMercenaryMounted)
            {
                mercenary = null;
                mercenaryCC = null;
                mercenaryRB = null;
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
            else if (!CanOperate())
            {
                Debug.LogWarning($"{gameObject.name} cannot mount until it is fully repaired.");
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
            else if (!CanOperate())
            {
                Debug.LogWarning($"{gameObject.name} cannot mount until it is fully repaired.");
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

    private bool CanOperate()
    {
        return currentWagonState == WagonState.Functional
            && !isBroken
            && currHPFunctional > 0;
    }

    public void OnTakeDamage(int damage)
    {
        if (isDestroyed || damage <= 0)
        {
            return;
        }

        CameraConstraint.Instance?.CameraShake();

        if (currentWagonState == WagonState.Functional)
        {
            currHPFunctional = Mathf.Max(0, currHPFunctional - damage);
            if (currHPFunctional <= 0)
            {
                currHPFunctional = 1;
                currHPBroken = 6;
                isBroken = true;
                SetWagonState(WagonState.Broken);
            }

            UpdateHPUI();
        }
        else if (currentWagonState == WagonState.Broken)
        {
            currHPBroken = Mathf.Max(0, currHPBroken - damage);
            if (currHPBroken <= 0)
            {
                OnWagonDestroyed();
            }

            UpdateHPUI();
        }
    }

    public void OnRepair(int repairAmount)
    {
        if (isDestroyed || repairAmount <= 0)
        {
            return;
        }

        if (currentWagonState == WagonState.Broken)
        {
            currHPBroken = Mathf.Min(currHPBroken + repairAmount, startHPBroken);

            if (currHPBroken < startHPBroken)
            {
                return;
            }

            currHPFunctional = Mathf.Clamp(currHPFunctional, 1, startHPFunctional);

            currHPFunctional = Mathf.Clamp(
                currHPFunctional + repairAmount,
                1,
                startHPFunctional);

            if (currHPFunctional >= startHPFunctional)
            {
                SetWagonState(WagonState.Functional);
                isBroken = false;
            }

            UpdateHPUI();
            return;
        }

        if (currentWagonState == WagonState.Functional && !isBroken)
        {
            currHPFunctional = Mathf.Clamp(
                currHPFunctional + repairAmount,
                1,
                startHPFunctional);
            UpdateHPUI();
        }
    }

    private void OnMechanicMount()
    {
        isMechanicMounted = true;
        mechanic.playerVisual.SetActive(false);
        if (mechanicCC != null && mechanicRB != null)
        {
            mechanicCC.enabled = false;
            mechanicRB.isKinematic = true; // Make the Rigidbody kinematic to prevent physics interactions
            mechanic.isMounted = true;
        }
        else
        {
            Debug.LogWarning($"{mechanic.gameObject.name} does not have a CharacterController & RigidBody component.");
        }
        // mechanicController.enabled = true; // Re-enable the CharacterController to reset its state
        mechanic.gameObject.transform.SetParent(slotMechanic);
        mechanic.gameObject.transform.localPosition = Vector3.zero;
    }

    private void OnMercenaryMount()
    {
        isMercenaryMounted = true;
        mercenary.playerVisual.SetActive(false);
        if (mercenaryCC != null && mercenaryRB != null)
        {
            mercenaryCC.enabled = false;
            mercenaryRB.isKinematic = true; // Make the Rigidbody kinematic to prevent physics interactions
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
        mechanic.playerVisual.SetActive(true);
        if (mechanicCC != null && mechanicRB != null)
        {
            mechanicCC.enabled = true;
            mechanicRB.isKinematic = false; // Make the Rigidbody non-kinematic to allow physics interactions
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
        mercenary.playerVisual.SetActive(true);
        if (mercenaryCC != null && mercenaryRB != null)
        {
            mercenaryCC.enabled = true;
            mercenaryRB.isKinematic = false; // Make the Rigidbody non-kinematic to allow physics interactions
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
        float forwardInput = Mathf.Max(0f, input.x);

        Vector3 movement =
            Vector3.right * forwardInput * moveSpeed * Time.deltaTime;

        transform.position += movement;
    }

    private void OnWagonDestroyed()
    {
        if (isDestroyed) return; // Prevent multiple destruction calls
        isDestroyed = true;
        // TODO - add wagon destruction logic here (e.g., play destruction animation, disable wagon, etc.)
        Debug.Log($"{gameObject.name} has been destroyed!");
        if (isMechanicMounted && mechanic != null)
        {
            OnMechanicDismount();
        }

        if (isMercenaryMounted && mercenary != null)
        {
            OnMercenaryDismount();
        }
        animator.SetTrigger("OnDestroyed");
    }

    private void SetWagonState(WagonState newState)
    {
        if (currentWagonState == newState)
        {
            return;
        }

        currentWagonState = newState;
        UpdateWagonStateAnimation();
    }

    private void UpdateWagonStateAnimation()
    {
        if (animator == null)
        {
            return;
        }

        switch (currentWagonState)
        {
            case WagonState.Functional:
                animator.SetTrigger("OnFunctional");
                break;
            case WagonState.Broken:
                animator.SetTrigger("OnBroken");
                break;
            default:
                Debug.LogWarning($"{gameObject.name} has an unknown wagon state.");
                break;
        }
    }

    private void UpdateHPUI()
    {
        if (wagonHPCanvas == null)
        {
            Debug.LogWarning($"{gameObject.name} is missing a wagon HP Canvas reference.");
            return;
        }

        bool repairingBrokenHP = currentWagonState == WagonState.Broken
            && currHPBroken < startHPBroken;
        GameObject heartPrefab = repairingBrokenHP
            ? heartBrokenPrefab
            : heartFunctionalPrefab;
        int displayedHP = repairingBrokenHP ? currHPBroken : currHPFunctional;

        if (heartPrefab == null)
        {
            Debug.LogWarning($"{gameObject.name} is missing the required wagon HP heart prefab.");
            return;
        }

        Transform uiRoot = wagonHPCanvas.transform;
        for (int i = uiRoot.childCount - 1; i >= 0; i--)
        {
            Destroy(uiRoot.GetChild(i).gameObject);
        }

        for (int i = 0; i < displayedHP; i++)
        {
            Instantiate(heartPrefab, uiRoot);
        }
    }

    // Supposedly call by the destroy animation event
    public void DestroyWagon(){Destroy(gameObject);}

    private void Update()
    {
        if (!isMechanicMounted || mechanic == null || !CanOperate()) return;
        
        ApplyDriverInput(mechanic.GetMoveInput());
    }
}