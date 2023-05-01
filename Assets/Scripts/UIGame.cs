using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIGame : MonoBehaviour
{
    public static UIGame Instance { get; private set; }
    public GameObject healthBar;
    public Slider slider;

    private PlayerController playerController;
    private EnemyController[] enemyControllers;
    private float gameTime;
    private float timeRemaining;

    [SerializeField]
    private TextMeshProUGUI pointsText;

    [SerializeField]
    private TextMeshProUGUI killText; // Añadido killText

    [SerializeField]
    private TextMeshProUGUI userNameText;

    [SerializeField]
    private TextMeshProUGUI timeRemainingText;

    private void Start()
    {
        // Asegurar que solo haya una instancia de UIGame
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
        // Obtener una referencia al UIManagerGame
        Instance = this;
        // Obtener el nombre de usuario del UIManager
        string userName = PlayerPrefs.GetString("UserName", "Usuario desconocido");
        // Mostrar el nombre de usuario en el TextMeshProUGUI
        userNameText.text = "Jugador: " + userName;

        // Obtener referencias al controlador del jugador y enemigos
        playerController = GameObject
            .FindGameObjectWithTag("Player")
            .GetComponent<PlayerController>();
        HealthController playerHealthController = playerController.GetComponent<HealthController>();
        enemyControllers = FindObjectsOfType<EnemyController>();
        // Suscribirse al evento OnHealthChanged y actualizar la barra de vida
        playerHealthController.OnHealthChanged += UpdateHealthBar;
        {
            UpdateHealthBar();
        }
        ;
        // Suscribirse al evento OnDeath de todos los enemigos en la escena
        foreach (EnemyController enemyController in enemyControllers)
        {
            HealthController enemyHealthController =
                enemyController.GetComponent<HealthController>();
            enemyHealthController.OnDeath += HandleEnemyDeath;
        }
        // Suscribirse al evento OnHealthChanged y actualizar la barra de vida
        playerHealthController.OnHealthChanged += UpdateHealthBar;
        {
            UpdateHealthBar();
        }
        ;

        // Inicializar el valor de la barra de vida
        slider.value = playerHealthController.GetHealth();
        slider.maxValue = playerHealthController.GetMaxHealth();

        // Obtener la referencia al slider de la barra de vida
        slider = healthBar.GetComponent<Slider>();

        // Actualizar el texto de los puntos en la pantalla
        UpdatePointsText(PlayerPrefs.GetInt("Puntos", 0));
        // Obtener el tiempo del GameManager
        gameTime = GameManager.Instance.GetGameTime();

        // Actualizar el tiempo restante en la pantalla
        timeRemaining = gameTime;
        UpdateTimeRemainingText();
    }

    public void UpdateHealthBar()
    {
        HealthController playerHealthController = playerController.GetComponent<HealthController>();
        int currentHealth = playerHealthController.GetHealth();
        int maxHealth = playerHealthController.GetMaxHealth();
        // Actualizar el valor y rango del slider de la barra de vida
        slider.value = currentHealth;
        slider.maxValue = maxHealth;
    }

    // Actualizar el texto de los puntos en la pantalla
    public void UpdatePointsText(int points)
    {
        pointsText.text = "Puntos: " + points;
    }

    private void UpdateTimeRemainingText()
    {
        // Convertir el tiempo restante a minutos y segundos
        int minutes = Mathf.FloorToInt(timeRemaining / 60f);
        int seconds = Mathf.FloorToInt(timeRemaining % 60f);

        // Actualizar el texto del tiempo restante en la pantalla
        timeRemainingText.text = "Tiempo: " + string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    private void Update()
    {
        // Restar el tiempo del temporizador
        timeRemaining = GameManager.Instance.GetTimeRemaining();
        UpdateTimeRemainingText();
    }

    public void UpdateKillsText(int kills)
    {
        killText.text = "Kills: " + kills;
    }

    private void HandleEnemyDeath()
    {
        // Verifica si el texto en killText contiene "Kills:"
        if (killText.text.Contains("Kills:"))
        {
            // Incrementar las muertes y actualizar el texto
            int currentKills = int.Parse(killText.text.Substring(6));
            UpdateKillsText(currentKills + 1);
        }
        else
        {
            // Si no se encuentra "Kills:", inicializa el contador de muertes en 1
            UpdateKillsText(1);
        }
    }

    public void SaveGameData()
    {
        PlayerPrefs.SetInt("Puntos", int.Parse(pointsText.text.Substring(8)));
        PlayerPrefs.SetFloat("TimeRemaining", timeRemaining);
        PlayerPrefs.Save();
    }
}
