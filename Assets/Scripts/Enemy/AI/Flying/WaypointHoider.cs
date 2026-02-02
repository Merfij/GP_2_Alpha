using System.Collections.Generic;
using System;
using UnityEngine;

public class WaypointHolder : MonoBehaviour
{
    public Transform[] waypoints;
    private void OnValidate()
    {
        RefreshWaypoints();
    }
    public void RefreshWaypoints()
    {
        List<Transform> wpList = new List<Transform>();
        foreach (Transform child in transform)
        {
            wpList.Add(child);
        }
        waypoints = wpList.ToArray();
    }

}
