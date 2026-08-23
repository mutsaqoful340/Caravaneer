using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput), typeof(CharacterController))]
public class PlayerComponent : MonoBehaviour
{
    private static readonly string[] AttackTriggers = { "Attack1", "Attack2", "Attack3" };

    [Header("Player Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private int HP = 3;
    [Tooltip("True for Mercenary, False for Driver")]
    [SerializeField] public bool isMercenary = false;
    [SerializeField] private float facingInputDeadZone = 0.1f;
    [SerializeField] private float groundY = 0f;
    [SerializeField] private float gravity = -18f;
    [Tooltip("The cooldown duration in seconds for the player's attack.")]
    public float attackCooldown = 1f; // Cooldown duration in seconds
    [Tooltip("The cooldown duration in seconds for resetting the attack sequence.") ]
    public float attackSequenceResetCooldown = 2f; // Cooldown duration in seconds

    [Header("Player Referenes")]
    public GameObject prevParent;
    public GameObject playerVisual;
    public Animator animator;
    public Sword sword;
    [Tooltip("The UI element that displays the player's health.")]
    public GameObject playerStatUI;
    [Tooltip("The prefab for the heart icon representing health.")]
    public GameObject heartPrefab;
    public PlayerInput playerInputComp;
    [Tooltip("The inventory component that manages the player's collective items and resources.")]
    public PlayerInventory inventory;


    [Header("Debug")]
    [Tooltip("DO NOT ASSIGN THIS MANUALLY. Reference to the wagon currently in range, if any.")]
    [SerializeField] private WagonComponent wagonInteractObject;
    [Tooltip("DO NOT ASSIGN THIS MANUALLY. This is a reference to the current enemy the player is interacting.")]
    [SerializeField] private Universal_Interact interactObject;
    // Locks the release event to whichever target actually received the press, so a target change mid-hold can't hijack the release.
    private enum PressedTarget { None, Generic, Wagon }
    private PressedTarget pressedTarget = PressedTarget.None;
    [Tooltip("DO NOT ASSIGN THIS MANUALLY. This is a reference to the current enemy the player is attacking.")]
    [SerializeField] private bool isAttacking = false;
    public bool isMounted = false;
    [SerializeField] private bool isMoveOpposDir = false;
    [SerializeField] private EnemyComponent currentEnemy;


    private Vector2 moveInput;
    private int currentAttackIndex;
    private float nextAttackTime;
    private float lastAttackTime = float.NegativeInfinity;
    private CharacterController characterController;
    private Vector3 verticalVelocity;
    // Release detection falls back to polling because the Input System's canceled message isn't reaching OnAction.
    private UnityEngine.InputSystem.InputAction actionInput;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        if (playerInputComp == null)
        {
            playerInputComp = GetComponent<PlayerInput>();
        }

        actionInput = playerInputComp?.actions?.FindAction("Action");

        prevParent = transform.parent != null ? transform.parent.gameObject : gameObject;
    }

    private void Start()
    {
        if (Manager_Input.Instance != null)
        {
            Manager_Input.Instance.RegisterPlayer(playerInputComp);
        }

        if (inventory == null)
        {
            inventory = PlayerInventory.Instance;
        }

        // OnUpdateHealthUI();
    }

    #region Input Callbacks
    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();

        if (moveInput.x < -facingInputDeadZone)
        {
            isMoveOpposDir = true;
        }
        else if (moveInput.x > facingInputDeadZone)
        {
            isMoveOpposDir = false;
        }

        if (playerVisual != null && !isMounted)
        {
            Vector3 visualScale = playerVisual.transform.localScale;
            float scaleX = Mathf.Abs(visualScale.x);
            visualScale.x = isMoveOpposDir ? -scaleX : scaleX;
            playerVisual.transform.localScale = visualScale;
        }

        if (animator != null)
        {
            animator.SetBool("IsMoving", moveInput.sqrMagnitude > 0f);
        }
    }

    public void OnAction(InputValue value)
    {
        Debug.Log($"[WagonInteract] OnAction invoked, isPressed={value.isPressed}, t={Time.time:F2}");

        RefreshInteractObjectReference();

        if (value.isPressed)
        {
            if (interactObject)
            {
                pressedTarget = PressedTarget.Generic;
                interactObject.BeginInteraction(this);
            }
            else if (wagonInteractObject)
            {
                pressedTarget = PressedTarget.Wagon;
                wagonInteractObject.BeginInteraction(this);
            }
            else
            {
                pressedTarget = PressedTarget.None;
            }

            OnPerformAction();

            Debug.Log($"{gameObject.name} performed an action!");
            return;
        }

        switch (pressedTarget)
        {
            case PressedTarget.Generic:
                interactObject?.EndInteraction();
                break;
            case PressedTarget.Wagon:
                Debug.Log($"[WagonInteract] {gameObject.name} released action at t={Time.time:F2}");
                wagonInteractObject?.EndInteraction(this);
                break;
        }

        pressedTarget = PressedTarget.None;
    }

    private void HandleActionReleasedByPolling()
    {
        if (pressedTarget == PressedTarget.None) return;

        Debug.Log($"[WagonInteract] {gameObject.name} release detected by polling at t={Time.time:F2}");

        switch (pressedTarget)
        {
            case PressedTarget.Generic:
                interactObject?.EndInteraction();
                break;
            case PressedTarget.Wagon:
                wagonInteractObject?.EndInteraction(this);
                break;
        }

        pressedTarget = PressedTarget.None;
    }

    public void OnPause(InputValue value)
    {
        if (value.isPressed)
        {
            if (Manager_Game.Instance.currentGameState == GameState.Gameplay)
            {
                Manager_Game.Instance.SetState(GameState.UI);
                Manager_UI.Instance.OnShowPanel("Store");
                return;
            }
        }
    }

    public void OnUINavigate(InputValue value)
    {
        if (Manager_Game.Instance.currentGameState == GameState.UI)
        {
            Vector2 navigationInput = value.Get<Vector2>();
            // Handle UI navigation logic here
        }
    }

    public void OnUISubmit(InputValue value)
    {
        if (Manager_Game.Instance.currentGameState == GameState.UI && value.isPressed)
        {
            // Handle UI submit logic here
            Debug.Log($"{gameObject.name} submitted UI action.");
        }
    }

    public void OnUICancel(InputValue value)
    {
        if (Manager_Game.Instance.currentGameState == GameState.UI && value.isPressed)
        {
            // Handle UI cancel logic here
            Debug.Log($"{gameObject.name} canceled UI action.");
        }
    }

    public void OnUIPause(InputValue value)
    {
        if (Manager_Game.Instance.currentGameState == GameState.UI && value.isPressed)
        {
            Manager_Game.Instance.SetState(GameState.Gameplay);
            Manager_UI.Instance.OnCloseAllPanels();
            Debug.Log($"{gameObject.name} confirmed UI action.");
        }
    }

    public void OnUISkip(InputValue value)
    {
        if (Manager_Game.Instance.currentGameState == GameState.UI && value.isPressed)
        {
            // Handle UI skip logic here
            Debug.Log($"{gameObject.name} skipped UI action.");
        }
    }
    #endregion

    #region Game Mechanics
    public void OnTriggerEnter(Collider other)
    {
        if (isMercenary && sword.isEnemySword) return; // Ignore if the sword is an enemy sword

        if (other.CompareTag("Wagon"))
        {
            WagonComponent wagon = other.GetComponent<WagonComponent>();
            if (wagon != null && wagonInteractObject == null)
            {
                wagonInteractObject = wagon;
            }
            return;
        }

        if (other.CompareTag("Interactable"))
        {
            Universal_Interact newInteractable = other.GetComponent<Universal_Interact>();

            if (newInteractable != null &&
                (interactObject == null || !interactObject.isActiveAndEnabled))
            {
                interactObject = newInteractable;
            }
        }
    }
    
    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Wagon"))
        {
            if (wagonInteractObject != null && other.gameObject == wagonInteractObject.gameObject)
            {
                wagonInteractObject.EndInteraction(this);
                wagonInteractObject = null;
                if (pressedTarget == PressedTarget.Wagon) pressedTarget = PressedTarget.None;
            }
            return;
        }

        if (other.CompareTag("Interactable"))
        {
            if (interactObject != null && other.gameObject == interactObject.gameObject)
            {
                interactObject.EndInteraction();
                interactObject = null;
                if (pressedTarget == PressedTarget.Generic) pressedTarget = PressedTarget.None;
            }
        }
    }

    public void OnTakeDamage(int damage)
    {
        if (damage <= 0)
        {
            return;
        }

        CameraConstraint.Instance?.CameraShake();
        HP -= damage;
        AddVerticalImpulse(1.5f);
        Debug.Log($"{gameObject.name} took {damage} damage! Remaining HP: {HP}");
        OnUpdateHealthUI();

        if (HP <= 0)
        {
            OnDie();
        }
    }

    public void AddVerticalImpulse(float amount)
    {
        verticalVelocity.y = Mathf.Max(verticalVelocity.y, amount);
    }

    private void RefreshInteractObjectReference()
    {
        if (!interactObject || !interactObject.isActiveAndEnabled)
        {
            interactObject = null;
        }

        if (!wagonInteractObject)
        {
            wagonInteractObject = null;
        }
    }

    private void OnUpdateHealthUI()
    {
        if (playerStatUI == null || heartPrefab == null)
        {
            Debug.LogWarning($"{gameObject.name} is missing playerStatUI or heartPrefab reference.");
            return;
        }

        Transform uiRoot = playerStatUI.transform;

        for (int i = uiRoot.childCount - 1; i >= 0; i--)
        {
            Destroy(uiRoot.GetChild(i).gameObject);
        }

        int currentHealth = Mathf.Max(0, HP);

        for (int i = 0; i < currentHealth; i++)
        {
            Instantiate(heartPrefab, uiRoot);
        }

        Debug.Log($"{gameObject.name} has {HP} HP remaining.");
    }

    private void OnPerformAction()
    {
        if (!isMercenary || interactObject || Manager_Game.Instance.currentGameScene == GameScene.MainMenu) return;

        if (animator == null)
        {
            Debug.LogWarning($"{gameObject.name} is missing an Animator reference.");
            return;
        }

        if (Time.time < nextAttackTime) return;

        if (Time.time - lastAttackTime > attackSequenceResetCooldown)
        {
            currentAttackIndex = 0;
        }

        string triggerName = AttackTriggers[currentAttackIndex];
        animator.SetTrigger(triggerName);

        currentAttackIndex = (currentAttackIndex + 1) % AttackTriggers.Length;
        lastAttackTime = Time.time;
        nextAttackTime = Time.time + attackCooldown;

        Debug.Log($"{gameObject.name} performed mercenary combo step {currentAttackIndex}.");
    }

    private void OnDie()
    {
        // TODO - add death logic here (e.g., play death animation, disable player controls, etc.)
        Debug.Log($"{gameObject.name} has died!");
    }
    #endregion

    public Vector2 GetMoveInput()
    {
        return moveInput;
    }

    private void OnDestroy()
    {
        if (Manager_Input.Instance != null){
            Manager_Input.Instance.UnregisterPlayer(playerInputComp);}
    }
    
    private void Update()
    {
        if (actionInput != null && pressedTarget != PressedTarget.None && !actionInput.IsPressed())
        {
            HandleActionReleasedByPolling();
        }

        if (isMounted) return;

        if (characterController.isGrounded && verticalVelocity.y < 0f)
        {
            verticalVelocity.y = 0f;
        }

        verticalVelocity.y += gravity * Time.deltaTime;

        Vector3 horizontalMovement = new Vector3(moveInput.x, 0f, moveInput.y) * moveSpeed;
        Vector3 movement = horizontalMovement * Time.deltaTime;
        movement.y = verticalVelocity.y * Time.deltaTime;

        characterController.Move(movement);

        if (transform.position.y < groundY)
        {
            Vector3 groundedPosition = transform.position;
            groundedPosition.y = groundY;
            transform.position = groundedPosition;
            verticalVelocity.y = 0f;
        }
    }
}