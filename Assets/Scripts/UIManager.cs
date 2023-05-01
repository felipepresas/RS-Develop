using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    // Screen object variables
    public GameObject loginUI;
    public GameObject registerUI;
    public GameObject userDataUI;
    public GameObject scoreboardUI;
    public GameObject gameOverUI;

    public GameObject[] screens;
    public FirebaseManager firebaseManager;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.LogError("More than one UIManager instance found! Destroying duplicate.");
            Destroy(this);
            return;
        }
        Debug.Log("UIManager.instance set to " + instance.gameObject.name);
    }

    // Función para mostrar la pantalla de Game Over
    public void ShowGameOverScreen()
    {
        ClearScreen();
        gameOverUI.SetActive(true);
    }

    public void ClearScreen() //Turn off all screens
    {
        for (int i = 0; i < screens.Length; i++)
        {
            screens[i].SetActive(false);
        }
    }

    public void LoginScreen() //Back button
    {
        ClearScreen();
        loginUI.SetActive(true);
        Debug.Log("Función LoginScreen llamada.");
    }

    public void RegisterScreen() // Register button
    {
        ClearScreen();
        registerUI.SetActive(true);
        Debug.Log("Función RegisterScreen llamada.");
    }

    public void UserDataScreen() //Logged in
    {
        ClearScreen();
        userDataUI.SetActive(true);
        Debug.Log("Función UserDataScreen llamada.");
        // Llama a la función LoadUserData de FirebaseManager
        firebaseManager.StartCoroutine(firebaseManager.LoadUserData());
    }

    public void ScoreboardScreen() //Scoreboard button
    {
        Debug.Log("Botón de marcador presionado - Cargando datos del marcador");
        StartCoroutine(ScoreboardManager.instance.LoadScoreboardData());
        ClearScreen();
        scoreboardUI.SetActive(true);
        Debug.Log("Función ScoreboardScreen llamada.");
    }

    public void RestartButton()
    {
        GameManager.Instance.RestartGame();
    }

    public void MainMenuButton()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
