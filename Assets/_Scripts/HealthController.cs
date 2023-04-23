using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthController : MonoBehaviour
{
    [SerializeField]
    private int maxHealth = 10;
    private int currentHealth;
    private UIManagerGame uiManager;
    private PlayerController player;

    void Start()
    {
        currentHealth = maxHealth;
        uiManager = FindObjectOfType<UIManagerGame>();
         player = FindObjectOfType<PlayerController>();
    }

    public void TakeDamage(int damage)
    {
       currentHealth -= damage;

    if (currentHealth <= 0) {
        currentHealth = 0;
        if (player.isDead) {
            GameManager.instance.GameOver();
        }
    }
    }

    private void Die()
    {
        gameObject.SetActive(false);
         PlayerController.instance.isDead = true;
        // ...
    }

    public void Heal(int amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
    }
}
