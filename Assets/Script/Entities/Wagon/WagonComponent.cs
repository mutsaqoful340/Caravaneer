using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WagonComponent : MonoBehaviour
{
    [Header("Wagon Settings")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private int HP = 5;

    [Header("Wagon References")]
    public Animator animator;

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            OnTakeDamage(1);
        }
    }

    private void OnTakeDamage(int damage)
    {
        HP -= damage;
        Debug.Log($"{gameObject.name} took {damage} damage! Remaining HP: {HP}");

        if (HP <= 0)
        {
            OnWagonDestroyed();
        }
    }

    private void OnWagonDestroyed()
    {
        // TODO - add wagon destruction logic here (e.g., play destruction animation, disable wagon, etc.)
        Debug.Log($"{gameObject.name} has been destroyed!");
        animator.SetTrigger("Destroy");
        // Optionally, you can destroy the wagon GameObject after a delay
        // Destroy(gameObject, 2f); // Adjust the delay as needed
    }
}