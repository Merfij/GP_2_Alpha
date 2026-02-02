using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class ShotgunPellet : MonoBehaviour
{
    public float debugRadius = 0.3f;
    public bool drawDebugSphere = true;

    private void OnDrawGizmos()
    {
        if (!drawDebugSphere) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, debugRadius);
    }

    public float speed = 15f;
    public float lifeTime = 1.31f;

    public float damage;
    
    private Rigidbody rb;

    private float spawnTime;

    void Start()
    {
        spawnTime = Time.time;
        rb = GetComponent<Rigidbody>();
        Destroy(gameObject, lifeTime);
    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    IDamagable enemy = other.GetComponent<IDamagable>();
    //    if (enemy != null)
    //    {
    //        Debug.Log("Hit via Trigger");
    //        enemy.TakeDamage(damage);
    //        Destroy(gameObject);
    //    }
    //}

    private void OnCollisionEnter(Collision collision)
    {
        IDamagable enemy = collision.collider.GetComponentInParent<IDamagable>();
        if (enemy != null) enemy.TakeDamage(damage);

        if (Time.time - spawnTime < 0.02f)
        {
            Destroy(gameObject, 0.016f);
            return;
        }

        // Ignore other pellets + players
        if (collision.gameObject.CompareTag("Pellet") ||
            collision.gameObject.CompareTag("Demon") ||
            collision.gameObject.CompareTag("Exorcist"))
            return;

        // Damage if possible

        Destroy(gameObject);
    }

}
