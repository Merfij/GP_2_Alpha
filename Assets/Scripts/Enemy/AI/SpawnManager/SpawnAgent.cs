using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public enum SpawnLimitMode
{
    SpawnCount,
    OnClear,
    Infinite
}

[Serializable]
public class SpawnEntry
{
    [Tooltip("Enemy prefab to spawn.")]
    public GameObject prefab;

    [Tooltip("How many times this prefab should be spawned by this spawner (in SpawnCount mode).")]
    [Min(0)]
    public int count = 1;
}
public class SpawnAgent : MonoBehaviour
{
    [Header("Spawn Plan")]
    [Tooltip("Exact spawn plan: each entry spawns its prefab 'count' times, in list order.")]
    public List<SpawnEntry> spawnPlan = new List<SpawnEntry>();

    [Tooltip("If true, the spawner will shuffle the planned spawns once at start (still respects counts).")]
    public bool shufflePlan = false;

    [Header("Limits")]
    [Tooltip("SpawnCount: spawn the plan once then stop.\nInfinite: repeat the plan forever until disabled.")]
    public SpawnLimitMode limitMode = SpawnLimitMode.SpawnCount;

    [Header("Spawn timing")]
    [Tooltip("Seconds between spawns.")]
    [Min(0f)]
    public float spawnDelay = 1f;

    [Header("Spawn area")]
    [Tooltip("Size of the random spawn area (X = width, Y = depth) centered on this spawner.")]
    public Vector2 spawnAreaSize = new Vector2(10f, 10f);

    public System.Action OnSpawnerFinishedSpawning;
    public System.Action OnSpawnerCleared;

    private int aliveCount;
    private bool hasSpawnedAnything;
    private Coroutine routine;

    public int AliveCount => aliveCount;

    private void OnEnable()
    {
        aliveCount = 0;
        hasSpawnedAnything = false;

        routine = StartCoroutine(SpawnAgents());
    }

    private void OnDisable()
    {
        if (routine != null)
        {
            StopCoroutine(routine);
            routine = null;
        }
    }

    private IEnumerator SpawnAgents()
    {
        // Build a concrete spawn queue from the plan
        List<GameObject> queue = BuildSpawnQueue();

        if (queue.Count == 0)
        {
            Debug.LogWarning($"{name}: Spawn plan is empty (or all counts are 0). Spawner will finish immediately.");
            OnSpawnerFinishedSpawning?.Invoke();
            gameObject.SetActive(false);
            yield break;
        }

        if (limitMode == SpawnLimitMode.SpawnCount)
        {
            for (int i = 0; i < queue.Count; i++)
            {
                SpawnOne(queue[i]);
                yield return new WaitForSeconds(spawnDelay);
            }

            OnSpawnerFinishedSpawning?.Invoke();
            gameObject.SetActive(false);
            yield break;
        }

        if(limitMode == SpawnLimitMode.OnClear)
        {
            //wait until all spawned enemies are dead, then spawn again, loop forever
            int indx = 0;
            while (true)
            {
                yield return new WaitForSeconds(spawnDelay);
                SpawnOne(queue[indx]);
                indx++;
                if (indx >= queue.Count)
                {
                    //wait until all enemies are dead
                    while (aliveCount > 0)
                    {
                        yield return null;
                    }
                    indx = 0;
                    // Optional: reshuffle each loop if you want variety while keeping counts
                    if (shufflePlan)
                        Shuffle(queue);
                }
            }
        }

        // Infinite: repeat the plan forever
        int index = 0;
        while (true)
        {
            SpawnOne(queue[index]);
            yield return new WaitForSeconds(spawnDelay);

            index++;
            if (index >= queue.Count)
            {
                index = 0;

                // Optional: reshuffle each loop if you want variety while keeping counts
                if (shufflePlan)
                    Shuffle(queue);
            }
        }
    }

    private List<GameObject> BuildSpawnQueue()
    {
        var queue = new List<GameObject>();

        if (spawnPlan == null) return queue;

        for (int i = 0; i < spawnPlan.Count; i++)
        {
            var entry = spawnPlan[i];
            if (entry == null || entry.prefab == null) continue;
            if (entry.count <= 0) continue;

            for (int c = 0; c < entry.count; c++)
                queue.Add(entry.prefab);
        }

        if (shufflePlan)
            Shuffle(queue);

        return queue;
    }

    private void SpawnOne(GameObject prefab)
    {
        Vector3 offset = new Vector3(
            Random.Range(-spawnAreaSize.x * 0.5f, spawnAreaSize.x * 0.5f),
            0.5f,
            Random.Range(-spawnAreaSize.y * 0.5f, spawnAreaSize.y * 0.5f)
        );

        Vector3 spawnPos = transform.position + offset;

        GameObject enemy = Instantiate(prefab, spawnPos, Quaternion.identity);
        EnemyCountDebug.instance.IncrementEnemyCount();
        hasSpawnedAnything = true;
        aliveCount++;

        var fsm = enemy.GetComponent<DeathNotifier>();
        if (fsm != null)
        {
            System.Action handler = null;
            handler = () =>
            {
                fsm.OnDied -= handler;
                HandleEnemyDied();
            };
            fsm.OnDied += handler;
        }
        else
        {
            Debug.LogWarning($"{name}: Spawned enemy '{enemy.name}' has no DeathNotifier. 'Cleared' condition may never fire.");
        }
    }

    private void HandleEnemyDied()
    {
        aliveCount = Mathf.Max(0, aliveCount - 1);

        if (hasSpawnedAnything && aliveCount == 0)
            OnSpawnerCleared?.Invoke();
    }

    private static void Shuffle(List<GameObject> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int j = Random.Range(i, list.Count);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(transform.position, new Vector3(spawnAreaSize.x, 0.1f, spawnAreaSize.y));
    }
}
