using UnityEngine;

public class CheckpointScript : MonoBehaviour
{
    private RespawnScript respawn;
    public Transform demonSpawn;
    public Transform exorcistSpawn;
    public bool spawnHere = false;

    private void Awake()
    {

        respawn = GameObject.FindGameObjectWithTag("Respawn").GetComponent<RespawnScript>();
    }

    private void Start()
    {
        if(spawnHere)
        {
            respawn.demonSpawn = demonSpawn.transform;
            respawn.exorcistSpawn = exorcistSpawn.transform;
            respawn.Respawn();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Demon") || other.gameObject.CompareTag("Exorcist"))
        {
            Debug.Log("Demon or Exorcist went through checkpoint");
            respawn.demonSpawn = demonSpawn.transform;
            respawn.exorcistSpawn = exorcistSpawn.transform;
        }
    }
}
