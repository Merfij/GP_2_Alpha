using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum NextSpawnerCondition
{
    AfterSeconds,
    WhenPreviousSpawnerCleared,
    WhenExternalTriggerFired
}

[Serializable]
public class SpawnerStep
{
    [Tooltip("Indexes into the director's 'spawners' list. These spawners will be activated at the same time for this step.")]
    public List<int> spawnerIndexes = new List<int>();

    [Tooltip("What must happen before the director moves to the NEXT step:\n" +
             "- AfterSeconds: wait a fixed time\n" +
             "- WhenPreviousSpawnerCleared: wait until all enemies spawned by THIS step's spawners are dead\n" +
             "- WhenExternalTriggerFired: wait until a trigger sends the matching key")]
    public NextSpawnerCondition nextCondition = NextSpawnerCondition.WhenPreviousSpawnerCleared;

    [Tooltip("Used only when nextCondition = AfterSeconds. How many seconds to wait before proceeding.")]
    [Min(0f)]
    public float seconds = 5f;

    [Tooltip("Used only when nextCondition = WhenExternalTriggerFired.\n" +
             "A SpawnDirectorTrigger or interactable must call director.FireTrigger(triggerKey) with this exact string.")]
    public string triggerKey;
}

public enum LoopCondition
{
    AfterSeconds,
    WhenCleared
}

public class EnemySpawnDirector : MonoBehaviour
{
    [Header("Spawners in this room")]
    [Tooltip("All spawners the director can control for this room. Steps reference these by index.")]
    public List<SpawnAgent> spawners = new List<SpawnAgent>();

    [Header("Sequence Steps")]
    [Tooltip("Ordered list of steps. The director activates the spawners in step 0, waits its condition, then step 1, etc.")]
    public List<SpawnerStep> steps = new List<SpawnerStep>();

    [Header("Looping")]
    [Tooltip("If enabled, the director will loop a range of steps (loopStartStep..loopEndStep) until StopLooping() is called.")]
    public bool loopSteps = false;

    [Tooltip("First step index to loop back to (inclusive).")]
    [Min(0)]
    public int loopStartStep = 0;

    [Tooltip("Last step index to loop (inclusive). When the director passes this step, it will loop back to loopStartStep.")]
    [Min(0)]
    public int loopEndStep = 0;

    [Tooltip("How the loop behaves when reaching loopEndStep:\n" +
             "- AfterSeconds: wait loopSeconds then restart loop\n" +
             "- WhenCleared: wait until all enemies spawned by spawners in the loop range are dead, then restart loop")]
    public LoopCondition loopCondition = LoopCondition.WhenCleared;

    [Tooltip("Used only when loopCondition = AfterSeconds. How long to wait before restarting the loop.")]
    [Min(0f)]
    public float loopSeconds = 10f;

    [Header("Final Encounter Events")]
    public UnityEvent OnFinalSpawnerCleared;

    private Coroutine sequenceRoutine;
    private bool stopLooping;

    private readonly HashSet<string> firedTriggers = new HashSet<string>();

    [Tooltip("Starts the configured step sequence from step 0. Typically called by a SpawnDirectorTrigger when player enters the room.")]
    public void StartSequence()
    {
        StopSequence();
        stopLooping = false;
        sequenceRoutine = StartCoroutine(RunSequence());
    }

    [Tooltip("Stops the current sequence coroutine. Does NOT automatically disable spawners unless you call TurnOffAllSpawners().")]
    public void StopSequence()
    {
        if (sequenceRoutine != null)
        {
            StopCoroutine(sequenceRoutine);
            sequenceRoutine = null;
        }
    }

    [Tooltip("Stops looping at the next loop check. Sequence may still continue if loopSteps is false.")]
    public void StopLooping()
    {
        stopLooping = true;
    }

    [Tooltip("Fires an external trigger key. Steps waiting on this key (WhenExternalTriggerFired) will proceed.")]
    public void FireTrigger(string key)
    {
        if (!string.IsNullOrEmpty(key))
            firedTriggers.Add(key);
    }

    [Tooltip("Disables ALL spawners in the 'spawners' list. This stops NEW spawns. It does NOT kill enemies already spawned.")]
    public void TurnOffAllSpawners()
    {
        for (int i = 0; i < spawners.Count; i++)
        {
            if (spawners[i] != null)
                spawners[i].gameObject.SetActive(false);
        }
    }

    private IEnumerator RunSequence()
    {
        if (steps == null || steps.Count == 0) yield break;

        int stepIndex = 0;

        while (true)
        {
            SpawnerStep step = steps[stepIndex];
            List<SpawnAgent> activated = ActivateStep(step);
            //--------------------Added---------------------
            bool isFinalStep = stepIndex == steps.Count - 1;

            if (isFinalStep && activated.Count > 0)
            {
                int remainingFinalSpawners = activated.Count;

                foreach (var sp in activated)
                {
                    if (sp == null)
                    {
                        remainingFinalSpawners--;
                        continue;
                    }

                    sp.OnSpawnerCleared += () =>
                    {
                        remainingFinalSpawners--;

                        if (remainingFinalSpawners <= 0)
                        {
                            Debug.Log("Final spawner cleared");
                            OnFinalSpawnerCleared?.Invoke();
                        }
                    };
                }
            }
            //-------------------------stop----------------------
            yield return WaitForStepCondition(step, activated);

            stepIndex++;

            if (stepIndex >= steps.Count)
            {
                if (!loopSteps || stopLooping) yield break;
                stepIndex = Mathf.Clamp(loopStartStep, 0, steps.Count - 1);
            }

            if (loopSteps && !stopLooping)
            {
                int start = Mathf.Clamp(loopStartStep, 0, steps.Count - 1);
                int end = Mathf.Clamp(loopEndStep, 0, steps.Count - 1);

                if (stepIndex > end)
                {
                    if (loopCondition == LoopCondition.AfterSeconds)
                        yield return new WaitForSeconds(loopSeconds);
                    else
                        yield return WaitUntilAllLoopSpawnersCleared(start, end);

                    if (stopLooping) yield break;

                    stepIndex = start;
                }
            }
        }
    }

    private List<SpawnAgent> ActivateStep(SpawnerStep step)
    {
        var activated = new List<SpawnAgent>();

        foreach (int idx in step.spawnerIndexes)
        {
            if (idx < 0 || idx >= spawners.Count) continue;
            SpawnAgent sp = spawners[idx];
            if (sp == null) continue;

            sp.OnSpawnerCleared = null;
            sp.OnSpawnerFinishedSpawning = null;

            sp.gameObject.SetActive(true);
            activated.Add(sp);
        }

        return activated;
    }

    private IEnumerator WaitForStepCondition(SpawnerStep step, List<SpawnAgent> activated)
    {
        switch (step.nextCondition)
        {
            case NextSpawnerCondition.AfterSeconds:
                yield return new WaitForSeconds(step.seconds);
                break;

            case NextSpawnerCondition.WhenExternalTriggerFired:
                yield return new WaitUntil(() => firedTriggers.Contains(step.triggerKey));
                break;

            case NextSpawnerCondition.WhenPreviousSpawnerCleared:
            default:
                if (activated.Count == 0) yield break;

                int remaining = activated.Count;
                foreach (var sp in activated)
                {
                    if (sp == null) { remaining--; continue; }
                    sp.OnSpawnerCleared += () => remaining--;
                }

                yield return new WaitUntil(() => remaining <= 0);
                break;
        }
    }

    private IEnumerator WaitUntilAllLoopSpawnersCleared(int startStep, int endStep)
    {
        yield return new WaitUntil(() =>
        {
            for (int s = startStep; s <= endStep; s++)
            {
                if (s < 0 || s >= steps.Count) continue;

                foreach (int idx in steps[s].spawnerIndexes)
                {
                    if (idx < 0 || idx >= spawners.Count) continue;
                    var sp = spawners[idx];
                    if (sp != null && sp.AliveCount > 0) return false;
                }
            }
            return true;
        });
    }
}
