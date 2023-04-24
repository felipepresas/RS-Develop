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
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Start()
    {
        // Obtener referencias a otros controladores y managers
        playerController = FindObjectOfType<PlayerController>();
        enemyControllers = FindObjectsOfType<EnemyController>();

        timer = gameTime;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        playerController = FindObjectOfType<PlayerController>();
    }

    public void GameOver()
    {
      if (!isGameOver)
    {
        isGameOver = true;
        // Muestra la pantalla de Partida finalizada en el UI
        UIGame.Instance.ShowEndGameScreen();
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

    private void Update()
    {
        // Resta el tiempo del temporizador
        timer -= Time.deltaTime;

        if (playerController.isDead && !isGameOver)
        {
            GameOver();
        }
        else if (timer <= 0f && !isGameOver)
        {
            // Fin del juego
            GameOver();
        }
    }

    public void UpdatePlayerScore(int points)
    {
        // Actualiza la puntuación basada en los puntos obtenidos
        UIGame.Instance.UpdatePointsText(points);
    }

    public void UpdatePlayerKills(int kills)
    {
        // Actualiza la cantidad de muertes
        UIGame.Instance.UpdateKillsText(kills);
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