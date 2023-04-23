using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
public static UIManager instance;

    //Screen object variables
    public GameObject loginUI;
    public GameObject registerUI;
    public GameObject userDataUI;
    public GameObject scoreboardUI;
      // Agregar referencias a los nuevos objetos de la pantalla
    public GameObject gameOverUI;
    public GameObject killText;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != null)
        {
            Debug.Log("Instancia exite, destruyendo objeto!");
            Destroy(this);
        }
        Debug.Log("UIManager.instance set to " + instance.gameObject.name);
    }
       // Gunción para mostrar la pantalla de Game Over
    public void ShowGameOverScreen()
    {
        ClearScreen();
        gameOverUI.SetActive(true);
    }

      public void UpdateScoreText(int kills)
    {
        killText.GetComponent<TextMeshProUGUI>().text = "kills: " + kills;
    }

    public void ClearScreen() //Turn off all screens
    {
        loginUI.SetActive(false);
        registerUI.SetActive(false);
        userDataUI.SetActive(false);
        scoreboardUI.SetActive(false);
    }

    public void LoginScreen() //Back button
    {
        ClearScreen();
        loginUI.SetActive(true);
    }
    public void RegisterScreen() // Regester button
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
