using UnityEngine;

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

    private void OnTriggerEnter(Collider other)
    {
        if (oneShot && triggered) return;
        if (!other.GetComponent<PlayerController>()) return;

        triggered = true;

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
}
