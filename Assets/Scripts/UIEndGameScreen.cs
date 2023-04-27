using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIEndGameScreen : MonoBehaviour
{
    [SerializeField]
    private GameObject endGameScreen;

    public void ShowEndGameScreen()
    {
        endGameScreen.SetActive(true);
    }
}
