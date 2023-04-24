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

    [SerializeField]
    private GameObject endGameScreen;
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
        playerHealthController.OnHealthChanged += () =>
        {
            UpdateHealthBar(
                playerHealthController.GetHealth(),
                playerHealthController.GetMaxHealth()
            );
        };

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

    public void UpdateHealthBar(int currentHealth, int maxHealth)
    {
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

    public void ShowEndGameScreen()
    {
        // Asumiendo que ya tienes un GameObject para la pantalla de Partida finalizada en tu UI
        endGameScreen.SetActive(true);
    }
}
