using UnityEngine;

public class ExplodingBarrel : MonoBehaviour, IDamagable
{
    [SerializeField] private float barrelHealth;
    [SerializeField] private float explosionDamage;
    [SerializeField] private float blastRadius;
    [SerializeField] private LayerMask layersToExplode;
    [SerializeField] private bool ethereal;

    private bool exploded;

    private void Awake()
    {
        if (!ethereal)
        {
            GetComponent<EtherialObject>().enabled = false;
            GetComponent<PlayerTriggerCheck>().enabled = false;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {        
        exploded = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (barrelHealth <= 0 && !exploded)
        {
            Explode();

        }
    }

    public void TakeDamage(float damage)
    {
        barrelHealth -= damage;
    }

    private void Explode()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, blastRadius, layersToExplode);

        foreach (var collider in hitColliders)
        {
            IDamagable units = collider.GetComponentInParent<IDamagable>();
            if (units != null) units.TakeDamage(explosionDamage);
        }
        exploded = true;
        GameEvents.OnBarrelExplosion?.Invoke(transform);
        Destroy(gameObject);
    }
}
