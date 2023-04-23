using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public static GameManager Instance { get; private set; }

    // Variables para almacenar referencias a otros controladores y managers
    private UIManagerGame uiManagerGame;
    private PlayerController playerController;
    private EnemyController[] enemyControllers;

    // Variables para el estado del juego
    private bool isGameOver = false;

    private void Awake()
    {
        // Asegurar que solo haya una instancia de GameManager
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        // No destruir este objeto al cargar una nueva escena
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        // Obtener referencias a otros controladores y managers
        uiManagerGame = FindObjectOfType<UIManagerGame>();
        playerController = FindObjectOfType<PlayerController>();
        enemyControllers = FindObjectsOfType<EnemyController>();
    }

    public void GameOver()
    {
        if (!isGameOver)
        {
            isGameOver = true;
            // Mostrar la pantalla de Game Over en el UI
            // uiManagerGame.ShowGameOverScreen();
        }
    }

    public void RestartGame()
    {
        // Reiniciar la escena actual
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void LoadScoreboardScene()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void AddPoints(int points)
    {
        // Agregar puntos al jugador y actualizar el UI
        // int currentPoints = PlayerPrefs.GetInt("Puntos", 0);
        // PlayerPrefs.SetInt("Puntos", currentPoints + points);
        // uiManagerGame.UpdatePointsText(currentPoints + points);
    }

    private void Update()
    {
        if (playerController.isDead && !isGameOver)
{
    GameOver();
}
        // Actualizar el estado del juego y otros objetos según sea necesario
        // Por ejemplo, verificar si todos los enemigos están muertos y avanzar al siguiente nivel
    }
}
