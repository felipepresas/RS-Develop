using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public static GameManager Instance { get; private set; }
    private float gameTime = 90f; // tiempo en segundos (1:30)
    private float timer = 0f;

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
        else if (Instance != this)
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

        timer = gameTime;
    }

    public void GameOver()
    {
        if (!isGameOver)
        {
            isGameOver = true;
            // Mostrar la pantalla de Game Over en el UI
            // uiManagerGame.ShowGameOverScreen();
            UIManager.instance.ShowGameOverScreen();
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
         // Resta el tiempo del temporizador
        timer -= Time.deltaTime;

        if (playerController.isDead && !isGameOver)
        {
            GameOver();
        }
           if (timer <= 0f)
        {
            // Fin del juego
            GameOver();
        }
        // Actualizar el estado del juego y otros objetos según sea necesario
        // Por ejemplo, verificar si todos los enemigos están muertos y avanzar al siguiente nivel
    }
     public float GetTimeRemaining()
    {
        return timer;
    }

    public float GetGameTime()
    {
        return gameTime;
    }
}
