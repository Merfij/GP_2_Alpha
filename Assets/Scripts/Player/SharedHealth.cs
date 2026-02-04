using System;
using UnityEngine;
using Object = UnityEngine.Object;
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
            EnemyBase[] allEnemies = Object.FindObjectsByType<EnemyBase>(FindObjectsSortMode.None);
            foreach (EnemyBase enemy in allEnemies)
            {
                enemy.ForceResetState();
            }
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
