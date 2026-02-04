using UnityEngine;

public class EnemyCountDebug : MonoBehaviour
{
    public static EnemyCountDebug instance;

    public int enemyCount = 0;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public void IncrementEnemyCount()
    {
        enemyCount++;
        Debug.Log("Enemy Count: " + enemyCount);
    }

    public void DecrementEnemyCount()
    {
        enemyCount--;
        Debug.Log("Enemy Count: " + enemyCount);
    }

}
