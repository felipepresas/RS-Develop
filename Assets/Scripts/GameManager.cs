using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private UIEndGameScreen uiEndGameScreen;
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
        uiEndGameScreen = FindObjectOfType<UIEndGameScreen>();
    }

    public void PlayerDied()
    {
        StartCoroutine(WaitForFirebaseManager());
    }

 public void GameOver()
{
    if (!isGameOver)
    {
        isGameOver = true;
        // Muestra la pantalla de Partida finalizada en el UI
        uiEndGameScreen.ShowEndGameScreen();

        // Actualiza la experiencia del jugador
        int experienceToAdd = 5;
        FirebaseManager firebaseManager = FindObjectOfType<FirebaseManager>();
        if (firebaseManager == null)
        {
            Debug.LogError("No se encontró un objeto FirebaseManager en la escena actual.");
        }
        else
        {
            ScoreboardManager scoreboardManager = GetScoreboardManagerInstance();
            if (scoreboardManager == null)
            {
                Debug.LogError("ScoreboardManager.instance es null");
            }
            else
            {
                string userId = FirebaseManager.instance.GetUserId();
                if (userId != null)
                {
                    GetScoreboardManagerInstance().UpdatePlayerExperience(userId, experienceToAdd);
                }
                else
                {
                    Debug.LogError("No se pudo obtener el ID de usuario.");
                }
            }
        }
        Debug.Log("ScoreboardManager.instance: " + GetScoreboardManagerInstance());
    }
}

    public void RestartGame()
    {
        // Reiniciar la escena actual
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);

        // Reiniciar el temporizador
        ResetTimer();

        // Agregar un evento para ejecutar una función después de que se haya cargado la nueva escena
        SceneManager.sceneLoaded += OnSceneRestarted;
    }

    private IEnumerator WaitForFirebaseManager()
    {
        while (FirebaseManager.instance == null)
        {
            yield return new WaitForSeconds(0.1f);
        }

        isGameOver = true;
        GameOver();
    }

    private void OnSceneRestarted(Scene scene, LoadSceneMode mode)
    {
        // Desuscribirse del evento sceneLoaded para evitar múltiples llamadas en reinicios posteriores
        SceneManager.sceneLoaded -= OnSceneRestarted;

        // Volver a encontrar el objeto UIEndGameScreen
        uiEndGameScreen = FindObjectOfType<UIEndGameScreen>();

        // Esconder la pantalla de fin de juego
        uiEndGameScreen.HideEndGameScreen();

        // Restablecer el estado del juego
        isGameOver = false;
    }

    public void LoadScoreboardScene()
    {
        SceneManager.LoadScene("MainMenu");
    }

    private void Update()
    {
        // Resta el tiempo del temporizador
        timer -= Time.deltaTime;

        // Comprueba que playerController no sea nulo antes de acceder a sus propiedades
        if (playerController != null)
        {
            if ((playerController.isDead || timer <= 0f) && !isGameOver)
            {
                StartCoroutine(WaitForFirebaseManager());
            }
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

    public void ResetTimer()
    {
        timer = gameTime;
    }
    private ScoreboardManager GetScoreboardManagerInstance()
{
    if (ScoreboardManager.instance == null)
    {
        ScoreboardManager scoreboardManager = FindObjectOfType<ScoreboardManager>();
        if (scoreboardManager != null)
        {
            ScoreboardManager.instance = scoreboardManager;
        }
        else
        {
            Debug.LogError("No se encontró un objeto ScoreboardManager en la escena actual.");
        }
    }

    return ScoreboardManager.instance;
}
}
