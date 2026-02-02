

using UnityEngine;

public class PreciseWall : MonoBehaviour
{
    [SerializeField]
    private Transform spawnPoint;

    [SerializeField]
    private GameObject preciseObject;

    private Rigidbody preciseRigidbody;

    private void Awake()
    {
        // Validate preciseObject
        if (preciseObject == null)
        {
            Debug.LogWarning("PreciseWall: `preciseObject` is not assigned in the Inspector. Collisions will be ignored until assigned.");
        }
        else
        {
            preciseRigidbody = preciseObject.GetComponent<Rigidbody>();
        }

        // Ensure spawnPoint is never null to avoid NullReferenceException
        if (spawnPoint == null)
        {
            if (preciseObject != null)
            {
                // Create a child GameObject to act as the spawn point and place it at the object's current position
                var spawnGo = new GameObject("SpawnPoint");
                spawnGo.transform.SetParent(transform, worldPositionStays: true);
                spawnGo.transform.position = preciseObject.transform.position;
                spawnPoint = spawnGo.transform;
            }
            else
            {
                // Fallback: use this object's transform so code doesn't crash
                spawnPoint = transform;
            }
        }
    }

    private void Start()
    {
        // If both spawnPoint and preciseObject are available, align the spawn point exactly with the object's current position.
        if (spawnPoint != null && preciseObject != null)
        {
            spawnPoint.position = preciseObject.transform.position;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (preciseObject == null)
        {
            // Nothing to do if the precise object isn't assigned
            return;
        }

        // Check by direct reference first, then by tag for flexibility
        if (collision.gameObject == preciseObject || collision.gameObject.CompareTag("Precise"))
        {
            Debug.Log("Precise Object collided with Precise Wall. Resetting to: " + spawnPoint.position);
            // Reset position
            preciseObject.transform.position = spawnPoint.position;

            // If the object has a Rigidbody, clear its velocity so it doesn't continue moving
            if (preciseRigidbody != null)
            {
                preciseRigidbody.linearVelocity = Vector3.zero;
                preciseRigidbody.angularVelocity = Vector3.zero;
            }
        }
    }
}