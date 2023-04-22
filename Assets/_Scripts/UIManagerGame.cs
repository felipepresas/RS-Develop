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

    [SerializeField]
    private TextMeshProUGUI enemyHealthText;

    [SerializeField]
    // private TextMeshProUGUI pointsText;

    private void Start()
    {
        // Obtener referencias al controlador del jugador y enemigos
        playerController = GameObject
            .FindGameObjectWithTag("Player")
            .GetComponent<PlayerController>();
        enemyControllers = FindObjectsOfType<EnemyController>();

        // Obtener la referencia al slider de la barra de vida
        slider = healthBar.GetComponent<Slider>();

        // Actualizar el texto de los puntos en la pantalla
        UpdatePointsText(PlayerPrefs.GetInt("Puntos", 0));
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
        // pointsText.text = "Puntos: " + points;
    }

    private void Update() { }
}
