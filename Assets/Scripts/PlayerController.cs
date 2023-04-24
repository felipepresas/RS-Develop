using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Firebase;
using Firebase.Database;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;

    [SerializeField]
    private float speed = 12.0f;

    [SerializeField]
    private float jumpForce = 5.0f;

    [SerializeField]
    private float timeToStop = 1.0f;

    [SerializeField]
    private float powerUpForce;

    [SerializeField]
    private float timePowerUp;

    [SerializeField]
    private float damageRadius = 5.0f;

    [SerializeField]
    private float abilityCooldown = 10f;

    [SerializeField]
    private int maxHealth = 10;
    private int kills = 0; // Añadir esta variable para llevar la cuenta de las muertes

    [SerializeField]
    private GameObject deathEffect;

    [SerializeField]
    private Image abilityImage;

    [SerializeField]
    private TextMeshProUGUI abilityCooldownText;

    [SerializeField]
    private AudioClip pressureReleaseClip;

    [SerializeField]
    private AudioSource secondAudioSource;

    [SerializeField]
    private GameObject playerParticles;

    [SerializeField]
    private GameObject damageEffect;

    [SerializeField]
    private LayerMask groundLayer;

    [SerializeField]
    private Collider damageArea;

    [SerializeField]
    private GameObject[] powerUpIndicators;

    public bool isDead;
    private string userId;
    private string username;
    private int currentHealth;
    private bool hasPowerUp;
    private bool isGrounded;
    private bool canDoubleJump;
    private bool canUseAbility = true;
    private float lastAbilityTime = Mathf.NegativeInfinity;
    private GameManager gameManager;
    private Rigidbody rb;
    private DatabaseReference databaseReference;
    private HealthController healthController;
    private Vector3 rotationDirection = Vector3.zero;

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        string ruta = "kills";
        databaseReference = FirebaseDatabase.DefaultInstance.GetReference(ruta);

        rb = GetComponent<Rigidbody>();
        rb.maxAngularVelocity = Mathf.Infinity;
        rb.angularDrag = 0;
        rb.drag = 0;
        rb.freezeRotation = true;
        currentHealth = maxHealth;

        healthController = GetComponent<HealthController>();
        healthController.OnDeath += HandlePlayerDeath;
    }

    void Update()
    {
        HandleMovement();
        HandleJump();
        HandleAbility();

        if (rb.velocity.magnitude > 0)
        {
            rb.velocity -= rb.velocity * timeToStop * Time.deltaTime;
        }
    }

    private void HandleMovement()
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");

        Vector3 movement = new Vector3(horizontalInput, 0, verticalInput);
        movement.Normalize();

        if (verticalInput <= 0) // si el jugador presiona hacia atras
        {
            rb.AddForce(transform.forward * -speed); // se aplica una fuerza en sentido contrario para reducir la velocidad
        }
        else // si el jugador presiona hacia adelante
        {
            rb.AddForce(transform.forward * speed); // se aplica la fuerza normal
        }

        transform.Rotate(rotationDirection * Time.deltaTime * 90f);

        if (horizontalInput != 0)
        {
            rotationDirection = Vector3.up * (horizontalInput > 0 ? 1 : -1);
        }

        playerParticles.SetActive(rb.velocity.magnitude > 0);
    }

    private void HandleJump()
    {
        if (isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
        }
    }

    private void HandleAbility()
    {
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
            secondAudioSource.PlayOneShot(pressureReleaseClip);
            lastAbilityTime = Time.time;
            canUseAbility = false;
        }
        if (Time.time >= lastAbilityTime + abilityCooldown)
        {
            canUseAbility = true;
        }

        UpdateAbilityUI();
    }

    private void HandlePlayerDeath()
    {
        isDead = true;
        Instantiate(deathEffect, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    private void UpdateAbilityUI()
    {
        abilityImage.enabled = canUseAbility;
        abilityCooldownText.enabled = !canUseAbility;

        if (!canUseAbility)
        {
            abilityCooldownText.text =
                "CD " + (lastAbilityTime + abilityCooldown - Time.time).ToString("F1");
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
        else if (other.CompareTag("Enemy"))
        {
            healthController.TakeDamage(1);
            UpdatePlayerKills();
        }
        else if (other.CompareTag("PickUp"))
        {
            other.gameObject.SetActive(false);
            UpdatePlayerScore(10);
            Destroy(other.gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
{
    if (collision.gameObject.CompareTag("Ground"))
    {
        isGrounded = true;
    }
    else if (collision.gameObject.CompareTag("Enemy"))
    {
        if (hasPowerUp)
        {
            HealthController enemyHealthController =
                collision.gameObject.GetComponent<HealthController>();
            enemyHealthController.TakeDamage(2);

            Rigidbody enemyRigidbody = collision.gameObject.GetComponent<Rigidbody>();
            Vector3 awayFromPlayer =
                collision.gameObject.transform.position - this.transform.position;
            enemyRigidbody.AddForce(awayFromPlayer * powerUpForce, ForceMode.Impulse);
        }
        else
        {
            healthController.TakeDamage(1);
            AudioSource audioSource = GetComponent<AudioSource>();
            if (audioSource && audioSource.enabled)
            {
                audioSource.Play();
            }
        }
    }
}

    private void UpdatePlayerScore(int points)
    {
        // Actualiza la puntuación basada en los puntos obtenidos
        UIGame.Instance.UpdatePointsText(points);
    }

    private void UpdatePlayerKills()
    {
        // Actualiza la cantidad de muertes
        kills += 1;
        UIGame.Instance.UpdateKillsText(kills);
    }

    IEnumerator PowerUpCountdown()
    {
        yield return new WaitForSeconds(timePowerUp);
        powerUpIndicators[0].gameObject.SetActive(false);
        hasPowerUp = false;
    }
}
