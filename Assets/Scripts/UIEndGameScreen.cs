using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIEndGameScreen : MonoBehaviour
{
    [SerializeField]
    private GameObject content;

    private void Start()
    {
        content.SetActive(false);
    }

    public void ShowEndGameScreen()
    {
        content.SetActive(true);
    }

    public void HideEndGameScreen()
    {
        content.SetActive(false);
    }
}

