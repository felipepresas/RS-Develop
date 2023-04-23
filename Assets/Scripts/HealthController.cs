using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthController : MonoBehaviour
{
    [SerializeField]
    private int maxHealth = 10;
    private int currentHealth;
    private UIGame uiManager;
    private PlayerController player;
    public delegate void DeathAction();
    public event DeathAction OnDeath;

    void Start()
    {
        currentHealth = maxHealth;
        uiManager = FindObjectOfType<UIGame>();
        player = FindObjectOfType<PlayerController>();
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            if (OnDeath != null)
            {
                OnDeath();
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
