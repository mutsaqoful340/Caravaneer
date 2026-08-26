using UnityEngine;
using System.Collections;

public class Axe : MonoBehaviour
{
    [Header("Axe Properties")]
    public int damage = -1;
    public float launchSpeed = 10f;
    [Tooltip("Accuracy of the axe throw, higher values mean more accurate throws, min value 0.1f, max value is 1.0f.")]
    [Range(0.1f, 1.0f)]
    public float accuracy = 0.1f; // Accuracy of the axe throw, higher values mean more accurate throws, min value 0.1f, max value is 1.0f
    [SerializeField] private float maximumThrowDeviation = 35f; // Maximum deviation angle for the axe throw, in degrees
    public float lifetime = 5f; // Lifetime of the axe before it gets destroyed
    public Animator animator; // Reference to the Animator component for the axe
    [SerializeField] private Collider axeCollider;

    [Header("Debug")]
    [SerializeField] private PlayerComponent playerTarget;
    [SerializeField] private WagonComponent wagonTarget;
    [SerializeField] private bool isDamaging = false;
    [SerializeField] private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        axeCollider = GetComponent<Collider>();
        OnThrown();
        OnRandomAccuracy();
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
                axeCollider.enabled = false; // Disable the axe's collider to prevent further collisions
                OnCollided(); // Stop the axe's movement upon collision
                playerTarget.OnTakeDamage(damage);
                Debug.Log($"{gameObject.name} dealt {damage} damage to {playerTarget.gameObject.name}!");
                StartCoroutine(DestroyAfterLifetime()); // Destroy the axe after hitting the player}
            }
        }
            
        if (other.CompareTag("Wagon"))
        {
            wagonTarget = other.GetComponent<WagonComponent>();
            if (wagonTarget != null && !isDamaging)
            {
                isDamaging = true;
                axeCollider.enabled = false; // Disable the axe's collider to prevent further collisions
                OnCollided(); // Stop the axe's movement upon collision
                wagonTarget.OnTakeDamage(damage);
                Debug.Log($"{gameObject.name} dealt {damage} damage to {wagonTarget.gameObject.name}!");
                StartCoroutine(DestroyAfterLifetime()); // Destroy the axe after hitting the wagon
            }
        }
    }

    public void OnTriggerExit(Collider other)
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
        StartCoroutine(DestroyAfterLifetime());

        float deviation = Mathf.Lerp(maximumThrowDeviation, 0f, accuracy);
        float horizontalDeviation = Random.Range(-deviation, deviation);
        float verticalDeviation = Random.Range(-deviation, deviation);
        Vector3 throwDirection = Quaternion.Euler(
            verticalDeviation,
            horizontalDeviation,
            0f
        ) * transform.forward;

        transform.forward = throwDirection;
        rb.linearVelocity = throwDirection * launchSpeed;
    }

    private void OnCollided()
    {
        // Stop the axe's movement upon collision
        animator.SetTrigger("Stuck"); // Trigger the collision animation
        if (playerTarget) {playerTarget.OnTakeDamage(damage);}
        else if (wagonTarget) {wagonTarget.OnTakeDamage(damage);}
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.isKinematic = true; // Make the axe kinematic to stop physics interactions
        Debug.Log("Axe has collided and stopped moving.");
    }

    private void OnRandomAccuracy()
    {
        // Randomly adjust the accuracy of the axe throw within the specified range
        accuracy = Random.Range(0.1f, 1.0f);
    }

    #region Updates & Coroutines
    private void LateUpdate()
    {
        if (!isDamaging)
        {

        }
    }
    private IEnumerator DestroyAfterLifetime()
    {
        yield return new WaitForSeconds(lifetime);
        Destroy(gameObject);
    }
    #endregion
}