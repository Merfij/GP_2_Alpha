using UnityEngine;
using UnityEngine.Events;

public enum DirectorTriggerAction
{
    StartSequence,
    TurnOffAllSpawners,
    FireTriggerKey,
    StopLooping
}

public class SpawnDirectorTrigger : MonoBehaviour
{
    [Tooltip("The room director that this trigger will send commands to.")]
    public EnemySpawnDirector director;

    [Tooltip("What this trigger does when the player enters it:\n" +
             "- StartSequence: starts the director's step sequence\n" +
             "- TurnOffAllSpawners: disables all spawners (stops new spawns)\n" +
             "- FireTriggerKey: fires a named key to advance steps waiting for that key\n" +
             "- StopLooping: tells the director to stop looping")]
    public DirectorTriggerAction action = DirectorTriggerAction.FireTriggerKey;

    [Tooltip("Used only when action = FireTriggerKey. Must match the step's triggerKey exactly.")]
    public string triggerKey;

    [Tooltip("If true, this trigger only fires once and then ignores future entries.")]
    public bool oneShot = true;

    private bool triggered;

    [Header("Events")]
    [Tooltip("Functions that can be called by any player")]
    public UnityEvent TriggeredByAnyPlayer;
    [Tooltip("Functions that need to be callde by both players")]
    public UnityEvent TriggeredByBothPlayers;

    public bool exorcistInside = false;
    public bool demonInside = false;

    private void Update()
    {
        if (demonInside && exorcistInside)
        {
            TriggeredByBothPlayers?.Invoke();
            // Reset triggered to allow re-triggering if needed
            //triggered = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (oneShot && triggered) return;
        if (!other.GetComponent<PlayerController>()) return;

        triggered = true;
        TriggeredByAnyPlayer?.Invoke();

        if (director == null)
        {
            Debug.LogWarning($"{name}: No director assigned.");
            return;
        }

        switch (action)
        {
            case DirectorTriggerAction.StartSequence:
                director.StartSequence();
                break;

            case DirectorTriggerAction.TurnOffAllSpawners:
                director.TurnOffAllSpawners();
                break;

            case DirectorTriggerAction.FireTriggerKey:
                director.FireTrigger(triggerKey);
                break;

            case DirectorTriggerAction.StopLooping:
                director.StopLooping();
                break;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        // Check if both players are inside the trigger
        if (other.CompareTag("Demon"))
        {
            demonInside = true;
        }

        if (other.CompareTag("Exorcist"))
        {
            exorcistInside = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // When they leave, flip the switch back
        if (other.CompareTag("Demon"))
        {
            demonInside = false;
        }

        if (other.CompareTag("Exorcist"))
        {
            exorcistInside = false;
        }
    }
}
