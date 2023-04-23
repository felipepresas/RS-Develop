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

    [SerializeField]
    private int maxHealth = 2; // Vida máxima del enemigo
    private int currentHealth; // Vida actual del enemigo
    private Transform playerTransform;
    private HealthController healthController; // Referencia al HealthController del enemigo

    [SerializeField]
    private GameObject explosionPrefab; // Prefab de la explosión que se instancia cuando el enemigo muere

    private void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        healthController = GameObject
            .FindGameObjectWithTag("Player")
            .GetComponent<HealthController>();

        currentHealth = maxHealth; // Inicializar la vida actual del enemigo
    }

    private void Update()
    {
        if (currentHealth <= 0)
        {
            Die();
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
            healthController.TakeDamage(1);
            currentHealth -= 1;
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Die();
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
