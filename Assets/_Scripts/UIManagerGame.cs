using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIManagerGame : MonoBehaviour
{
    public static UIManagerGame Instance { get; private set; }

    [SerializeField]
    private TextMeshProUGUI healthText;
    public GameObject healthBar;
    public Slider slider;
    private PlayerController playerController;
    private EnemyController[] enemyControllers;
    private float gameTime;
    private float timeRemaining;

    [SerializeField]
    private TextMeshProUGUI enemyHealthText;

    [SerializeField]
    private TextMeshProUGUI pointsText;

    [SerializeField]
    private TextMeshProUGUI userNameText;

    [SerializeField]
    private TextMeshProUGUI timeRemainingText;

    private void Start()
    {
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
        enemyControllers = FindObjectsOfType<EnemyController>();

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
}
