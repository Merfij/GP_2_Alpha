using UnityEngine;

public class LightControl : MonoBehaviour
{
    public Light sceneLight;

    private void Start()
    {
        sceneLight = this.GetComponent<Light>();
        sceneLight.enabled = false;
    }

    public void TurnOnLight()
    {
        if (sceneLight != null)
        {
            sceneLight.enabled = true;
        }
    }

    public void TurnOffLight()
    {
        if (sceneLight != null)
        {
            sceneLight.enabled = false;
        }
    }

    public void RedLight()
    {
        if(sceneLight != null)
        {
            sceneLight.color = Color.red;
        }
    }

    public void GreenLight()
    {
        if(sceneLight != null)
        {
            sceneLight.color = Color.green;
        }
    }
}
