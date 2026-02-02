using System;
using UnityEngine;
public class SharedHealth : MonoBehaviour, IDamagable
{
    [SerializeField] private float maxHealth;
    public RespawnScript respawnScript;
    public float currentHealth;

    private void Start()
    {
        currentHealth = maxHealth;
    }
    //Alex Code
    public float getMaxHealth() { return maxHealth; }
    //

    void Update()
    {
        if (currentHealth <= 0)
        {
            Debug.Log("Player Died");
            respawnScript.Respawn();
            ResetHeath();
        }
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
    }

    public void Heal(float healAmount)
    {
        currentHealth += healAmount;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
    }

    public void ResetHeath()
    {
        if (currentHealth < maxHealth)
        {
            currentHealth = maxHealth;
        }
        else return;
    }
}
