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
    public int currentHealth; // Vida actual del enemigo
    private Transform playerTransform;
    private HealthController healthController; // Referencia al HealthController del enemigo
    public GameObject explosionPrefab;
    

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
        // Verificar si el enemigo ha perdido toda su vida
        if (currentHealth <= 0)
        {
            Destroy(gameObject); // Destruir el objeto del enemigo
            return;
        }

        // Calcular la dirección hacia el jugador
        Vector3 direction = (playerTransform.position - transform.position).normalized;

        // Rotar hacia el jugador
        Quaternion lookRotation = Quaternion.LookRotation(direction, Vector3.up);
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            lookRotation,
            Time.deltaTime * rotationSpeed
        );

        // Mover hacia el jugador dentro del rango establecido
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
            // Restar una cantidad de vida al enemigo cuando colisiona con el jugador
            healthController.TakeDamage(1);
            // Restar una unidad de vida al enemigo
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
        Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
