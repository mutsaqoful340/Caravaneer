using System.Collections;
using UnityEngine.AI;
using UnityEngine;

public class EnemyComponent : MonoBehaviour
{
    [Header("Enemy Settings")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private int HP = 3;
    [SerializeField] private float knockbackForce = 5f;
    [SerializeField] private float knockbackDuration = 0.2f;
    [SerializeField] private float attackCooldown = 1f;
    [SerializeField] private float attackRange = 1.5f;

    [Header("Enemy References")]
    [SerializeField] private Animator animator;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private GameObject enemyVisual;

    [Header("Debug")]
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Transform targetTransform;
    [SerializeField] private GameObject sword;
    [SerializeField] private bool isDead = false;
    private Coroutine knockbackRoutine;
        private Coroutine attackCooldownRoutine;
    private Quaternion rootRotation;

    void Awake()
    {
        targetTransform = GameObject.FindGameObjectWithTag("Player")?.transform;
        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
        rootRotation = transform.rotation;

        if (agent != null)
        {
            agent.speed = moveSpeed;
            agent.updateRotation = false;
        }
    }

    // === Enemy Actions ===
    #region Enemy Actions
    private void OnAttack()
    {
            if (isDead || targetTransform == null ||
                Vector3.Distance(transform.position, targetTransform.position) > attackRange)
            {
                return;
            }

            animator.SetTrigger("Attack");
            Debug.Log($"{gameObject.name} attacked {targetTransform.name}!");
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
            if (attackCooldownRoutine != null)
            {
                StopCoroutine(attackCooldownRoutine);
                attackCooldownRoutine = null;
            }

            if (knockbackRoutine != null)
            {
                StopCoroutine(knockbackRoutine);
                knockbackRoutine = null;
            }

            if (agent != null && agent.enabled && agent.isOnNavMesh)
            {
                agent.isStopped = true;
                agent.ResetPath();
            }

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
    #endregion

    // === Method Helpers ===
    #region Method Helpers
    private void OnKnockback(Vector3 knockbackDirection, float knockbackForce)
    {
        if (knockbackDirection == Vector3.zero) return;

        if (agent != null && agent.enabled)
        {
            agent.isStopped = true;
            agent.ResetPath();
            agent.enabled = false;
        }

        if (rb != null)
        {
            rb.AddForce(knockbackDirection * knockbackForce, ForceMode.Impulse);
        }
        else
        {
            transform.position += knockbackDirection * knockbackForce * Time.deltaTime;
        }

        if (knockbackRoutine != null)
        {
            StopCoroutine(knockbackRoutine);
        }

        knockbackRoutine = StartCoroutine(ResumeNavigationAfterKnockback());
    }
    #endregion

    // === Updates & Coroutines ===
    #region Updates & Coroutines
    private void Update()
    {
        if (targetTransform == null)
        {
            targetTransform = GameObject.FindGameObjectWithTag("Player")?.transform;
        }
        else if (!isDead)
        {
            transform.rotation = rootRotation;

                float distanceToTarget = Vector3.Distance(transform.position, targetTransform.position);
                bool isInAttackRange = distanceToTarget <= attackRange;

                if (isInAttackRange)
            {
                    if (agent != null && agent.enabled && agent.isOnNavMesh)
                    {
                        agent.isStopped = true;
                        agent.ResetPath();
                    }

                    if (attackCooldownRoutine == null)
                    {
                        attackCooldownRoutine = StartCoroutine(OnAttackCooldown());
                    }
                }
                else if (agent != null && agent.enabled && agent.isOnNavMesh)
                {
                    agent.isStopped = false;
                    agent.SetDestination(targetTransform.position);
            }

            UpdateFacing();
        }
    }

    private void UpdateFacing()
    {
        if (enemyVisual == null) return;

        float horizontalDirection = agent != null && agent.enabled
            ? agent.desiredVelocity.x
            : targetTransform.position.x - transform.position.x;

        if (Mathf.Abs(horizontalDirection) <= 0.01f) return;

        Vector3 visualScale = enemyVisual.transform.localScale;
        float scaleX = Mathf.Abs(visualScale.x);
        visualScale.x = horizontalDirection < 0f ? -scaleX : scaleX;
        enemyVisual.transform.localScale = visualScale;
    }

    private IEnumerator OnAttackCooldown()
    {
        yield return new WaitForSeconds(attackCooldown);
            attackCooldownRoutine = null;

            if (!isDead && targetTransform != null &&
                Vector3.Distance(transform.position, targetTransform.position) <= attackRange)
            {
                OnAttack();
            }
    }

    private IEnumerator ResumeNavigationAfterKnockback()
    {
        yield return new WaitForSeconds(knockbackDuration);

        if (isDead || agent == null)
        {
            knockbackRoutine = null;
            yield break;
        }

        if (NavMesh.SamplePosition(transform.position, out NavMeshHit hit, 2f, NavMesh.AllAreas))
        {
            transform.position = hit.position;
            agent.enabled = true;
            agent.Warp(hit.position);
            agent.isStopped = false;
        }

        knockbackRoutine = null;
    }
    #endregion
}