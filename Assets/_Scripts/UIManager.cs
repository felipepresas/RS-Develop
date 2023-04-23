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
    public GameObject pointsText;

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
    }
       // Nueva función para mostrar la pantalla de Game Over
    public void ShowGameOverScreen()
    {
        ClearScreen();
        gameOverUI.SetActive(true);
    }
      // Nueva función para actualizar el texto de los puntos en la pantalla
    // public void UpdatePointsText(int points)
    // {
    //     pointsText.text = "Puntos: " + points;
    // }

      public void UpdateScoreText(int points)
    {
        pointsText.GetComponent<TextMeshProUGUI>().text = "Puntos: " + points;
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
