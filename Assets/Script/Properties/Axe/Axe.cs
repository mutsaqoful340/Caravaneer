using UnityEngine;

public class Axe : MonoBehaviour
{
    [Header("Axe Properties")]
    public int damage = -1;
    public float launchSpeed = 10f;

    [Header("Debug")]
    [SerializeField] private PlayerComponent playerTarget;
    [SerializeField] private WagonComponent wagonTarget;
    [SerializeField] private bool isDamaging = false;
    [SerializeField] private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        OnThrown();
    }

    private void OnDisable()
    {
        playerTarget = null;
        wagonTarget = null;
        isDamaging = false;
    }
    
        public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerTarget = other.GetComponent<PlayerComponent>();
            if (playerTarget != null && !isDamaging)
            {
                isDamaging = true;
                playerTarget.OnTakeDamage(damage);
                Debug.Log($"{gameObject.name} dealt {damage} damage to {playerTarget.gameObject.name}!");
            }
        }
        else if (other.CompareTag("Wagon"))
        {
            wagonTarget = other.GetComponent<WagonComponent>();
            if (wagonTarget != null && !isDamaging)
            {
                isDamaging = true;
                wagonTarget.OnTakeDamage(damage);
                Debug.Log($"{gameObject.name} dealt {damage} damage to {wagonTarget.gameObject.name}!");
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerTarget = null;
            isDamaging = false;
        }
        else if (other.CompareTag("Wagon"))
        {
            wagonTarget = null;
            isDamaging = false;
        }
    }

    private void OnThrown()
    {
        rb.linearVelocity = transform.forward * launchSpeed;
    }
}