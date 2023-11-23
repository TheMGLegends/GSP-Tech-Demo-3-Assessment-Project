using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AggroDetection : MonoBehaviour
{
    [SerializeField] private float aggroRadius;
    private CircleCollider2D aggroRange;
    private float stoppingDistanceFromTarget;

    private EnemyController enemyController;

    public float GetStoppingDistanceFromTarget() => stoppingDistanceFromTarget;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, aggroRadius);
    }

    private void Start()
    {
        aggroRange = GetComponentInChildren<CircleCollider2D>();
        aggroRange.radius = aggroRadius;
        stoppingDistanceFromTarget = aggroRadius * 0.75f; //75% size of aggro radius

        enemyController = GetComponentInParent<EnemyController>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (enemyController != null)
            {
                enemyController.SetTarget(collision.gameObject);
                enemyController.SetCanAttack(true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (enemyController != null)
            {
                enemyController.SetCanAttack(false);
            }
        }
    }
}
