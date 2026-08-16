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
    [SerializeField] private bool isMercenary = false;
    [SerializeField] private float facingInputDeadZone = 0.1f;
    [Tooltip("The cooldown duration in seconds for the player's attack.")]
    public float attackCooldown = 1f; // Cooldown duration in seconds
    [Tooltip("The cooldown duration in seconds for resetting the attack sequence.") ]
    public float attackSequenceResetCooldown = 2f; // Cooldown duration in seconds

    [Header("Player Referenes")]
    public GameObject playerVisual;
    public Animator animator;
    [Tooltip("The UI element that displays the player's health.")]
    public GameObject playerStatUI;
    [Tooltip("The prefab for the heart icon representing health.")]
    public GameObject heartPrefab;
    public PlayerInput playerInputComp;


    [Header("Debug")]
    [Tooltip("DO NOT ASSIGN THIS MANUALLY. This is a reference to the current interactable object the player is interacting with.")]
    [SerializeField] private Universal_Interact currentInteractable;
    [Tooltip("DO NOT ASSIGN THIS MANUALLY. This is a reference to the current enemy the player is interacting.")]
    [SerializeField] private Universal_Interact interactObject;
    [Tooltip("DO NOT ASSIGN THIS MANUALLY. This is a reference to the current enemy the player is attacking.")]
    [SerializeField] private bool isAttacking = false;
    [SerializeField] private bool isMoveOpposDir = false;
    [SerializeField] private EnemyComponent currentEnemy;


    private Vector2 moveInput;
    private int currentAttackIndex;
    private float nextAttackTime;
    private float lastAttackTime = float.NegativeInfinity;
    private CharacterController characterController;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        if (playerInputComp == null)
        {
            playerInputComp = GetComponent<PlayerInput>();
        }
    }

    private void Start()
    {
        if (Manager_Input.Instance != null)
        {
            Manager_Input.Instance.RegisterPlayer(playerInputComp);
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

        if (playerVisual != null)
        {
            Vector3 visualScale = playerVisual.transform.localScale;
            float scaleX = Mathf.Abs(visualScale.x);
            visualScale.x = isMoveOpposDir ? -scaleX : scaleX;
            playerVisual.transform.localScale = visualScale;
        }
    }

    public void OnAction(InputValue value)
    {
        RefreshInteractObjectReference();

        if (value.isPressed)
        {
            if (interactObject)
            {
                interactObject.BeginInteraction();
            }

            OnPerformAction();

            Debug.Log($"{gameObject.name} performed an action!");
            return;
        }

        if (interactObject)
        {
            interactObject.EndInteraction();
        }
    }

    public void OnPause(InputValue value)
    {
        if (value.isPressed)
        {
            Manager_Game.Instance.SetState(Manager_Game.Instance.currentGameState == GameState.UI ? GameState.Gameplay : GameState.UI);
            // Debug.Log($"{gameObject.name} toggled game state to {Manager_Game.Instance.currentGameState}.");
        }
    }
    #endregion

    public void OnTriggerEnter(Collider other)
    {
        // if (other.CompareTag("Enemy"))
        // {
        //     OnTakeDamage(1);
        // }

        if (other.CompareTag("Interactable"))
        {
            interactObject = other.GetComponent<Universal_Interact>();
        }
    }
    
    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Interactable"))
        {
            if (interactObject != null && other.gameObject == interactObject.gameObject)
            {
                interactObject.EndInteraction();
                interactObject = null;
            }
        }
    }

    public void OnTakeDamage(int damage)
    {
        HP -= damage;
        // TODO - add a knockback effect here
        Debug.Log($"{gameObject.name} took {damage} damage! Remaining HP: {HP}");
        OnUpdateHealthUI();

        if (HP <= 0)
        {
            OnDie();
        }
    }

    private void RefreshInteractObjectReference()
    {
        if (!interactObject || !interactObject.isActiveAndEnabled)
        {
            interactObject = null;
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
        if (!isMercenary || interactObject) return;

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

    private void OnDestroy()
    {
        if (Manager_Input.Instance != null){
            Manager_Input.Instance.UnregisterPlayer(playerInputComp);}
    }
    
    private void Update()
    {
        Vector3 movement = new Vector3(moveInput.x, 0f, moveInput.y) * Time.deltaTime * moveSpeed;
        characterController.Move(movement);
    }
}