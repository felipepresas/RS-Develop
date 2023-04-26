using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreElement : MonoBehaviour
{
    public TextMeshProUGUI usernameText;
    public TextMeshProUGUI killsText;
    public TextMeshProUGUI deathsText;
    public TextMeshProUGUI xpText;

    public void NewScoreElement(string username, int kills, int deaths, int xp)
    {
        usernameText.text = username;
        killsText.text = kills.ToString();
        deathsText.text = deaths.ToString();
        xpText.text = xp.ToString();
    }
}
