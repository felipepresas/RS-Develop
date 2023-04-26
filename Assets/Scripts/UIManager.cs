using UnityEngine;
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
    }

    public void RegisterScreen() // Register button
    {
        ClearScreen();
        registerUI.SetActive(true);
    }

    public void UserDataScreen() //Logged in
    {
        ClearScreen();
        userDataUI.SetActive(true);
    }

    public void ScoreboardScreen() //Scoreboard button
    {
        ClearScreen();
        scoreboardUI.SetActive(true);
    }
}
