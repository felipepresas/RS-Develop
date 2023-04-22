using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthController : MonoBehaviour
{
    public int maxHealth = 10;
    public int currentHealth;
    
    void Start()
    {
        currentHealth = maxHealth;
    }
    
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        
        if (currentHealth <= 0)
        {
            Die();
        }
           else
    {
        UIManagerGame uiManager = FindObjectOfType<UIManagerGame>();
        uiManager.UpdateHealthBar(currentHealth, maxHealth);
    }
    }
    
    private void Die()
    {
        gameObject.SetActive(false);
        // ...
    }
    
    public void Heal(int amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
    }
}
