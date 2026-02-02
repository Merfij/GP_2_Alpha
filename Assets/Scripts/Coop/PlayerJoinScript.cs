using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerJoinScript : MonoBehaviour
{
    public Transform SpawnPoint1, SpawnPoint2;
    public GameObject PlayerPrefab1, PlayerPrefab2;

    public GameObject health;

    public enum DisplayMode
    {
        SplitScreen,   // both players on Display 1 using viewports
        DualMonitor    // player 1 on Display 1, player 2 on Display 2
    }
    [Header("Mode Toggle")]
    [SerializeField] private DisplayMode mode = DisplayMode.SplitScreen;

    private void Awake()
    {
        //if health reference is missing, skip it
        health = GameObject.FindGameObjectWithTag("Respawn");
        if (health != null)
        {
            health.SetActive(true);
        }

        var pads = Gamepad.all;

        PlayerInput p1 = null;
        PlayerInput p2 = null;

        //for development, if no gamepads are connected, use keyboard and mouse for both players
        if (pads.Count < 1)
        {
            p1 = PlayerInput.Instantiate(PlayerPrefab1, controlScheme: "Keyboard&Mouse");
            p2 = PlayerInput.Instantiate(PlayerPrefab2, controlScheme: "Keyboard&Mouse");
        }
        else if (pads.Count == 1)
        {
            p1 = PlayerInput.Instantiate(PlayerPrefab1, controlScheme: "Keyboard&Mouse");
            p2 = PlayerInput.Instantiate(PlayerPrefab2, controlScheme: "Gamepad", pairWithDevice: pads[0]);
        }
        else
        {
            p1 = PlayerInput.Instantiate(PlayerPrefab1, controlScheme: "Gamepad", pairWithDevice: pads[0]);
            p2 = PlayerInput.Instantiate(PlayerPrefab2, controlScheme: "Gamepad", pairWithDevice: pads[1]);
        }

        // Spawn positions
        p1.transform.SetPositionAndRotation(SpawnPoint1.position, SpawnPoint1.rotation);

 
        p2.transform.SetPositionAndRotation(SpawnPoint2.position, SpawnPoint2.rotation);
 

        if (mode == DisplayMode.DualMonitor && p2 != null)
        {
            if (Display.displays.Length > 1 && Display.displays[1].active == false)
                Display.displays[1].Activate();
            //disable split screen and assign p2 to display 2
            GetComponent<PlayerInputManager>().splitScreen = false;
            p2.GetComponentInChildren<Camera>().targetDisplay = 1;
        }
    }
}
