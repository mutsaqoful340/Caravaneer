using UnityEngine.AI;
using UnityEngine;

public class EnemyComponent : MonoBehaviour
{
    [Header("Enemy Settings")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private int HP = 3;
    [SerializeField] private float knockbackForce = 5f;

    [Header("Enemy References")]
    [SerializeField] private Animator animator;
    [SerializeField] private Rigidbody rb;

    [Header("Debug")]
    [SerializeField] private Transform targetTransform;
    [SerializeField] private GameObject sword;
    [SerializeField] private bool isDead = false;

    void Awake()
    {
        targetTransform = GameObject.FindGameObjectWithTag("Player")?.transform;
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (targetTransform == null)
        {
            targetTransform = GameObject.FindGameObjectWithTag("Player")?.transform;
        }
        else if (!isDead)
        {
            Vector3 direction = (targetTransform.position - transform.position).normalized;
            transform.Translate(direction * moveSpeed * Time.deltaTime, Space.World);
            OnAttack();
        }
    }
    
    private void OnKnockback(Vector3 knockbackDirection, float knockbackForce)
    {
        if (knockbackDirection == Vector3.zero) return;

        if (rb != null)
        {
            rb.AddForce(knockbackDirection * knockbackForce, ForceMode.Impulse);
        }
        else
        {
            transform.position += knockbackDirection * knockbackForce * Time.deltaTime;
        }
    }

    // public void OnTriggerEnter(Collider other)
    // {
    //     if (other.CompareTag("Sword"))
    //     {
    //         sword = other.gameObject;
    //         OnTakeDamage(1);
    //     }
    // }

    private void OnAttack()
    {
        if (targetTransform != null)
        {
            if (Vector3.Distance(transform.position, targetTransform.position) >= 1.5f) return;
            
            animator.SetTrigger("Attack");
            Debug.Log($"{gameObject.name} attacked {targetTransform.name}!");
        }
    }

    public void OnTakeDamage(int damage)
    {
        HP -= damage;

        Vector3 knockbackDirection = Vector3.zero;

        if (sword != null)
        {
            knockbackDirection = (transform.position - sword.transform.position).normalized;
        }
        else if (targetTransform != null)
        {
            knockbackDirection = (transform.position - targetTransform.position).normalized;
        }

        OnKnockback(knockbackDirection, knockbackForce);
        sword = null;

        if (HP <= 0)
        {
            animator.SetTrigger("Die");
            isDead = true;
            return;
        }

        Debug.Log($"{gameObject.name} took {damage} damage! Remaining HP: {HP}");
    }

    public void OnDie()
    {
        Destroy(gameObject);
        Debug.Log($"{gameObject.name} has died!");
    }

    public void OnDespawn()
    {
        Destroy(gameObject);
    }
}