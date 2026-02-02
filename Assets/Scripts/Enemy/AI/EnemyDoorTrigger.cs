using UnityEngine;
using UnityEngine.Events;

public class EnemyDoorTrigger : MonoBehaviour
{
    public GameObject door;

    private void Awake()
    {
        door = GameObject.FindGameObjectWithTag("Door");
    }

    private void Start()
    {
        Door doorscript = door.GetComponent<Door>();
        doorscript.Open();
    }
}
