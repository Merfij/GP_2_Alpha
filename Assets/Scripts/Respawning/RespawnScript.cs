using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;

public class RespawnScript : MonoBehaviour
{
    [Header("Player Respawn Points")]
    public Transform demonSpawn;
    public Transform exorcistSpawn;

    public GameObject demon;
    public GameObject exorcist;

    void Awake()
    {
        if (demon == null)
            demon = GameObject.FindGameObjectWithTag("Demon");
        if (exorcist == null)
            exorcist = GameObject.FindGameObjectWithTag("Exorcist");
    }

    public void Respawn()
    {
        // Ensure required references exist
        if (demon == null || exorcist == null)
        {
            Debug.LogWarning("Respawn: missing `demon` or `exorcist` reference. Attempting to find by tag.");
            if (demon == null)
                demon = GameObject.FindGameObjectWithTag("Demon");
            if (exorcist == null)
                exorcist = GameObject.FindGameObjectWithTag("Exorcist");
        }

        if (demon == null || exorcist == null || demonSpawn == null || exorcistSpawn == null)
        {
            Debug.LogError("Respawn: missing references (demon/exorcist/spawn). Aborting respawn.");
            return;
        }

        CharacterController demonCC = null;
        CharacterController exorcistCC = null;

        if (demon.TryGetComponent<CharacterController>(out var cc1))
            demonCC = cc1;
        if (exorcist.TryGetComponent<CharacterController>(out var cc2))
            exorcistCC = cc2;

        if (demonCC != null) demonCC.enabled = false;
        if (exorcistCC != null) exorcistCC.enabled = false;

        demon.transform.position = demonSpawn.position;
        exorcist.transform.position = exorcistSpawn.position;

        if (demonCC != null) demonCC.enabled = true;
        if (exorcistCC != null) exorcistCC.enabled = true;
    }
}
