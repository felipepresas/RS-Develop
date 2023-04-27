using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class HealthController : MonoBehaviour
{
    [SerializeField]
    private int maxHealth = 10;
    private int currentHealth;

    [SerializeField]
    private float damageCooldown = 0.5f;
    private float lastDamageTime = Mathf.NegativeInfinity;
    private UIGame uiManager;
    private PlayerController player;
    public delegate void DeathAction();
    public event DeathAction OnDeath;
    public event Action OnHealthChanged;

    void Start()
    {
        currentHealth = maxHealth;
        uiManager = FindObjectOfType<UIGame>();
        player = FindObjectOfType<PlayerController>();
    }

    public void TakeDamage(int damage)
    {
        if (Time.time >= lastDamageTime + damageCooldown)
        {
            currentHealth -= damage;
            lastDamageTime = Time.time;

            if (currentHealth <= 0)
            {
                if (OnDeath != null)
                {
                    OnDeath();
                }
                Die();
            }
            OnHealthChanged?.Invoke();
        }
    }

    private void Die()
    {
        gameObject.SetActive(false);

        if (gameObject.CompareTag("Player"))
        {
            GameManager.Instance.GameOver();
        }
    }

    public void Heal(int amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        OnHealthChanged?.Invoke();
    }

    public int GetHealth()
    {
        return currentHealth;
    }

    public int GetMaxHealth()
    {
        return maxHealth;
    }
}
