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
    private float speed = 5.0f;

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

    private string userId;
    private string username;
    private int currentHealth;
    private int score = 0;
    private bool hasPowerUp;
    private bool isGrounded;
    private bool canDoubleJump;
    public bool isDead;
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
        
        if (UIManager.instance == null)
        {
            Debug.LogError("UIManager.instance is null");
        }
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

        rb.AddForce(transform.forward * speed);
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
            UpdatePlayerScore(1);
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
                EnemyController enemyController =
                    collision.gameObject.GetComponent<EnemyController>();
                enemyController.TakeDamage(2);

                Rigidbody enemyRigidbody = collision.gameObject.GetComponent<Rigidbody>();
                Vector3 awayFromPlayer =
                    collision.gameObject.transform.position - this.transform.position;
                enemyRigidbody.AddForce(awayFromPlayer * powerUpForce, ForceMode.Impulse);
            }
            else
            {
                healthController.TakeDamage(1);
                GetComponent<AudioSource>().Play();
            }
        }
    }

    private void UpdatePlayerScore(int points)
    {
        if (UIManager.instance != null)
        {
            UIManager.instance.UpdateScoreText(score);
        }
        else
        {
            Debug.LogError("UIManager.instance is null");
        }
    }

    IEnumerator PowerUpCountdown()
    {
        yield return new WaitForSeconds(timePowerUp);
        powerUpIndicators[0].gameObject.SetActive(false);
        hasPowerUp = false;
    }
}
