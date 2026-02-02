using UnityEngine;

public class SpikeTrap : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject spikes;
    [SerializeField] private float damage;
    [SerializeField] private bool damagePlayer;
    [SerializeField] private bool damageEnemy;    
    [SerializeField] private float resetDelay = 0.1f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (spikes != null)
        {
            spikes.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (animator.GetBool("Spikes")) 
            return;
        bool didDamage = false;

        if (damagePlayer && (other.CompareTag("Demon")||other.CompareTag("Exorcist")))
        {
            PlayerHealth health = other.GetComponentInParent<PlayerHealth>();
            if (health != null)
            {
                health.TakeDamage(damage);
                didDamage = true;
            } 
        }

        if (damageEnemy)
        {
            IDamagable enemy = other.GetComponentInParent<IDamagable>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
                didDamage = true;
            }
        }

        if(didDamage)
        {
            animator.SetBool("Spikes", true);
            Debug.Log("triggered");
            Invoke(nameof(ResetSpikes), resetDelay);
        }
    }

    private void ResetSpikes()
    {
        animator.SetBool("Spikes", false);
    }
}
