using Unity.VisualScripting;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private SharedHealth sharedHealth;
    [SerializeField] private RespawnScript spawnScript;
    public float setHealth;
    public static float health; 
    //Alex code
    public delegate void OnPlayerDamage();
    public OnPlayerDamage onPlayerDamage;
    //
    void Start()
    {
        health = setHealth;
        sharedHealth = GameObject.FindGameObjectWithTag("Health").GetComponent<SharedHealth>();
        //spawnScript = GameObject.FindGameObjectWithTag("Respawn").GetComponent<RespawnScript>();
    }

    private void Update()
    {
        //if (health <= 0)
        //{
        //    spawnScript.Respawn();
        //    ResetHeath();
        //}
    }

    public void TakeDamage(float damage)
    {
        sharedHealth.TakeDamage(damage);
        //health -= damage;
        Debug.Log("Should take damage" + health);
        //Alex code
        onPlayerDamage.Invoke();
        //

        // play damage sound
        if (gameObject.name == "Exorcist(Clone)")
        {
            GameEvents.OnExorcistHit?.Invoke();
        }
        else if (gameObject.name == "Demon(Clone)")
        {
            GameEvents.OnDemonHit?.Invoke();
        }
    }

    public void Heal(float healAmount)
    {
        sharedHealth.Heal(healAmount);
        //health += healAmount;
        Debug.Log("Should heal" + health);
    }

    public void ResetHeath()
    {
        if (health < setHealth)
        {
            health = setHealth;
        }
        else return;
    }

}
