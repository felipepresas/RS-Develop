using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed = 5f;

    [SerializeField]
    private float rotationSpeed = 5f;

    [SerializeField]
    private float moveRange = 10f;

    private Transform playerTransform;
    private HealthController playerHealthController;
    private HealthController enemyHealthController;

    [SerializeField]
    private GameObject explosionPrefab; // Prefab de la explosión que se instancia cuando el enemigo muere

    private void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        playerHealthController = GameObject
            .FindGameObjectWithTag("Player")
            .GetComponent<HealthController>();

        enemyHealthController = GetComponent<HealthController>();

        enemyHealthController.OnDeath += Die;
    }

    private void Update()
    {
                // Verifica si playerTransform es null antes de acceder a su posición
        if (playerTransform == null)
        {
            return;
        }
        Vector3 direction = (playerTransform.position - transform.position).normalized;

        Quaternion lookRotation = Quaternion.LookRotation(direction, Vector3.up);
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            lookRotation,
            Time.deltaTime * rotationSpeed
        );

        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
        if (distanceToPlayer <= moveRange)
        {
            transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            enemyHealthController.TakeDamage(1);
        }
    }

    void Die()
    {
        if (explosionPrefab != null)
        {
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        }
        Destroy(gameObject);
    }
}
