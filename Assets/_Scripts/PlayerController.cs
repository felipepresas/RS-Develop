using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Firebase;
using Firebase.Database;
using UnityEngine.SceneManagement;
using TMPro;

public class PlayerController : MonoBehaviour
{
    public string userId;
    public string username;
    public float speed = 5.0f;
    public float jumpForce = 5.0f;
    public float timeToStop = 1.0f;
    public float powerUpForce;
    public float timePowerUp;
    public float damageRadius = 5.0f;
    public float abilityCooldown = 10f;
    private float lastAbilityTime = 0f;
    public int maxHealth = 10;
    public int currentHealth;
    public int score = 0;
    public bool hasPowerUp;
    private bool isGrounded;
    private bool canDoubleJump;
    private bool canUseAbility = true;
    public AudioSource pressureReleaseAudio;
    public TextMeshProUGUI scoreText; // Agregar esta línea para referenciar el objeto de texto de TextMeshPro.

    public GameObject playerParticles;
    public GameObject damageEffect;
    public GameObject deathEffect;
    public LayerMask groundLayer;
    public Collider damageArea;
    private Rigidbody rb;
    private DatabaseReference databaseReference;
    public HealthController healthController;
    private Vector3 rotationDirection = Vector3.zero;
    public GameObject[] powerUpIndicators;

    void Start()
    {
        string ruta = "kills";
        databaseReference = FirebaseDatabase.DefaultInstance.GetReference(ruta);

        rb = GetComponent<Rigidbody>();
        rb.maxAngularVelocity = Mathf.Infinity;
        rb.angularDrag = 0;
        rb.drag = 0;
        rb.freezeRotation = true;
        currentHealth = maxHealth;
    }

    void Update()
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");

        Vector3 movement = new Vector3(horizontalInput, 0, verticalInput);
        movement.Normalize();

        if (isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
        }

        if (canUseAbility && Input.GetKeyDown(KeyCode.E))
        {
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, damageRadius);
            foreach (Collider hitCollider in hitColliders)
            {
                if (hitCollider.CompareTag("Enemy"))
                {
                    Destroy(hitCollider.gameObject);
                }
            }
            Instantiate(damageEffect, transform.position, Quaternion.identity);
            ActivarHabilidad();
            lastAbilityTime = Time.time;
            canUseAbility = false;
        }

        if (!canUseAbility && Time.time - lastAbilityTime >= abilityCooldown)
        {
            canUseAbility = true;
        }

        if (horizontalInput != 0)
        {
            rotationDirection = Vector3.up * (horizontalInput > 0 ? 1 : -1);
        }

        if (rb.velocity.magnitude > 0)
        {
            rb.velocity -= rb.velocity * timeToStop * Time.deltaTime;
        }

        CheckAndSendGameData();
    }

    public void ActivarHabilidad()
    {
        GetComponent<AudioSource>().Play();
        pressureReleaseAudio.Play();
    }

    private void CheckAndSendGameData()
    {
        if (currentHealth <= 0 && !gameObject.activeSelf)
        {
            Instantiate(deathEffect, transform.position, Quaternion.identity);
        }
    }

    void FixedUpdate()
    {
        rb.AddForce(transform.forward * speed);
        transform.Rotate(rotationDirection * Time.fixedDeltaTime * 90f);

        if (rb.velocity.magnitude > 0)
        {
            playerParticles.SetActive(true);
        }
        else
        {
            playerParticles.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PowerUp"))
        {
            hasPowerUp = true;
            powerUpIndicators[0].gameObject.SetActive(true);
            StartCoroutine(PowerUpCountdown());
            Destroy(other.gameObject);
        }
        if (other.CompareTag("Enemy"))
        {
            healthController.TakeDamage(1);
        }
        if (other.CompareTag("Enemy"))
        {
            // Sumar un punto
            int puntosActuales = PlayerPrefs.GetInt("Puntos", 0);
            puntosActuales++;
            PlayerPrefs.SetInt("Puntos", puntosActuales);
        }
        if (other.gameObject.CompareTag("PickUp"))
        {
            other.gameObject.SetActive(false);
            score += 10; // Aumentar la puntuación en 10 puntos por cada objeto recogido.
            UpdateScoreText(); // Llamada a la función para actualizar el objeto de texto con la puntuación.
            Destroy(other.gameObject);
        }
    }

    void UpdateScoreText()
    {
        scoreText.text = "Puntuación: " + score.ToString();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }

        if (collision.gameObject.CompareTag("Enemy") && hasPowerUp)
        {
            EnemyController enemyController = collision.gameObject.GetComponent<EnemyController>();

            // Restar 2 puntos de vida al enemigo
            enemyController.TakeDamage(2);

            Rigidbody enemyRigidbody = collision.gameObject.GetComponent<Rigidbody>();
            Vector3 awayFromPlayer =
                collision.gameObject.transform.position - this.transform.position;
            enemyRigidbody.AddForce(awayFromPlayer * powerUpForce, ForceMode.Impulse);
        }
        else if (collision.gameObject.CompareTag("Enemy"))
        {
            healthController.TakeDamage(1);
        }
    }

    public void OnSliderValueChanged(float value)
    {
        if (healthController != null)
        {
            healthController.currentHealth = (int)value;
            UIManagerGame.Instance.UpdateHealthBar(
                healthController.currentHealth,
                healthController.maxHealth
            );
        }
    }

    IEnumerator PowerUpCountdown()
    {
        yield return new WaitForSeconds(timePowerUp);
        powerUpIndicators[0].gameObject.SetActive(false);
        hasPowerUp = false;
    }
}
