using System;
using UnityEngine;

public class DeathNotifier : MonoBehaviour
{
    public event Action OnDied;

    // Call this from your enemy's death logic
    public void NotifyDied()
    {
        OnDied?.Invoke();
    }
}
