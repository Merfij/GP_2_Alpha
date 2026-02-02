using UnityEngine;

public class HealthPack : MonoBehaviour
{
    //two types of health packs: small and large
    public enum HealthPackType { Small, Large }

    public HealthPackType healthPackType;
    public int smallHealthAmount = 2;
    public int largeHealthAmount = 5;


    void Start()
    {
        if(HealthPackType.Large == healthPackType)
        {
            transform.localScale *= 1.5f;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Demon") || other.gameObject.CompareTag("Exorcist"))
        {
            PlayerHealth healthScript = other.gameObject.GetComponent<PlayerHealth>();
            if (healthScript != null)
            {
                int healthAmount = healthPackType == HealthPackType.Small ? smallHealthAmount : largeHealthAmount;
                healthScript.Heal(healthAmount);
                Destroy(gameObject);
            }
        }
    }
}
