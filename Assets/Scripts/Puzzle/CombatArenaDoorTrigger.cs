using UnityEngine;
using UnityEngine.Events;

public class CombatArenaDoorTrigger : MonoBehaviour
{
    [Header("Defines if door will be opened or closed")]
    public UnityEvent NewDoorStatus;
    [Header("Defines if door is opened or closed on start")]
    public UnityEvent DoorStatusOnStart;
    [Header("If both players are currently within trigger area door will close")]
    public bool demonEntered;
    public bool exorcistEntered;
    private bool shouldClose;

    private void Start()
    {
        demonEntered = false;
        exorcistEntered = false;
        shouldClose = false;
        DoorStatusOnStart.Invoke();
    }

    private void Update()
    {
        if (demonEntered == true && exorcistEntered == true)
        {
            shouldClose = true;
        }

        if (shouldClose == true)
        {
            NewDoorStatus.Invoke();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Demon"))
        {
            demonEntered = true;
        }

        if (other.CompareTag("Exorcist"))
        {
            exorcistEntered = true;
        }
    }
}
