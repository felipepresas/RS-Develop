using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class IntroManager : MonoBehaviour
{
     public float introLength = 5f; // Duración en segundos de la introducción
    
    private float timer = 0f; // Temporizador para contar el tiempo transcurrido

    // Update se llama una vez por frame
    void Update()
    {
        // Si ha pasado el tiempo de introducción, carga la siguiente escena
        if (timer >= introLength)
        {
            SceneManager.LoadScene("MainMenu"); // Nombre de la escena que quieres cargar
        }

        // Incrementa el temporizador con el tiempo transcurrido desde el último frame
        timer += Time.deltaTime;
    }
}
