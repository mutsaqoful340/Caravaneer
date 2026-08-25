using UnityEngine;

public class EnemyWeapon : MonoBehaviour
{
    public EnemyComponent enemyParent;

    private void Start()
    {
        if (enemyParent == null)
        {
            enemyParent = GetComponentInParent<EnemyComponent>();
        }
    }

    private void LateUpdate()
    {
        if (enemyParent != null && enemyParent.Target != null)
        {
            // Rotate the weapon to face the target
            Vector3 directionToTarget = enemyParent.Target.transform.position - transform.position;
            Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
        }
    }
}
