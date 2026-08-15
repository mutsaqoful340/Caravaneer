using UnityEngine;

public class Sword : MonoBehaviour
{
    public int damage = 1;

    [Header("Debug")]
    [SerializeField] private EnemyComponent currentEnemy;

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            currentEnemy = other.GetComponent<EnemyComponent>();
            if (currentEnemy != null)
            {
                currentEnemy.OnTakeDamage(damage);
                Debug.Log($"{gameObject.name} dealt {damage} damage to {currentEnemy.gameObject.name}!");
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            currentEnemy = null;
        }
    }
}
