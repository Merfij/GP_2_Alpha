using System;
using UnityEngine;

public class StaticVariables:MonoBehaviour
{
    private void Start()
    {
        PlayerCount = 0;
    }

    public static int PlayerCount;
}
