using FMOD.Studio;
using FMODUnity;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Battery : MonoBehaviour, IAbilityTarget, IActivator
{
    [SerializeField] private AbilityType requieredAbility;
    [SerializeField] public UnityEvent onActivated;
    [SerializeField] public UnityEvent onDeactivated;
    [SerializeField] private float maxCharge = 1;
    [SerializeField] private float drainMultiplier = 0.5f;
    [SerializeField] private bool drainBeforeFull;
    [SerializeField] private bool drainAfterFull;
    [SerializeField] private float drainDelay;
    [SerializeField] private Slider slider;

    public UnityEvent Activated => onActivated;
    public UnityEvent Deactivated => onDeactivated;
    public bool IsActive => activated;


    private float currentCharge;
    private bool charging;
    private bool draining;
    private bool activated;
    private Renderer batteryRenderer;
    private bool delaying;
    private bool delayDone;

    private EventInstance chargeInstance;
    private EventInstance drainInstance;

    private void Start()
    {
        currentCharge = Mathf.Clamp(currentCharge, 0f, maxCharge);
        currentCharge = 0;
        charging = false;
        activated = false;
        delaying = false;
        delayDone = true;
        batteryRenderer = gameObject.GetComponent<Renderer>();
        if (slider != null)
        {
            slider.maxValue = maxCharge;
            slider.value = currentCharge;
        }        
    }

    public void AbilityStart(AbilityType ability)
    {
        if (ability != requieredAbility)
            return;

        charging = true;
        draining = false;

        if (drainInstance.isValid())
        {
            drainInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            drainInstance.release();
        }

        if (chargeInstance.isValid())
            return;

        EventReference evt = AudioManager.Instance.GetEvent("BatteryCharge");
        chargeInstance = RuntimeManager.CreateInstance(evt);
        chargeInstance.start();
    }

    public void AbilityEnd(AbilityType ability)
    {
        if (ability != requieredAbility)
            return;

        charging = false; 
        
        if(currentCharge > 0f)
        {
            if(drainAfterFull && activated || drainBeforeFull && !activated)
            {
                draining = true;
            }            
        }

        if (chargeInstance.isValid())
        {
            chargeInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            chargeInstance.release();
        }
    }

    private void Update()
    {
        if(charging)
        {
            if (currentCharge < maxCharge)
            {
                currentCharge += Time.deltaTime;
                if (currentCharge >= maxCharge)
                {
                    if (!activated)
                    {
                        onActivated.Invoke();
                        GameEvents.OnBatteryFull?.Invoke(transform);
                        activated = true;                        
                    }
                    
                    batteryRenderer.material.SetColor("_BaseColor", Color.green);
                    
                    delayDone = false;
                    if (!delaying && drainAfterFull)
                        StartCoroutine(DrainDelay());
                }
            }            
        }
        else if (draining) 
        {
            if (drainBeforeFull && !activated)
            {
                currentCharge -= Time.deltaTime * drainMultiplier;
                batteryRenderer.material.SetColor("_BaseColor", Color.yellow);
            }              

            if (delayDone && currentCharge > 0f)
            {
                currentCharge -= Time.deltaTime * drainMultiplier;
                batteryRenderer.material.SetColor("_BaseColor", Color.yellow);
            }

            if (delayDone && !drainInstance.isValid())
            {
                EventReference evt = AudioManager.Instance.GetEvent("BatteryDrain");
                drainInstance = RuntimeManager.CreateInstance(evt);

                if (!drainInstance.isValid()) 
                    return;
                drainInstance.start();
            }

            if (currentCharge <= 1f)
            {
                if (drainInstance.isValid())
                {
                    drainInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                    drainInstance.release();
                }
            }

            if (currentCharge <= 0f && activated)
            {
                onDeactivated.Invoke();
                batteryRenderer.material.SetColor("_BaseColor", Color.gray);
                activated = false;
                draining = false;    
            }
        }   

        if(slider != null)
            slider.value = currentCharge;

        if (chargeInstance.isValid())
        {
            chargeInstance.set3DAttributes(RuntimeUtils.To3DAttributes(transform));
        }
        if (drainInstance.isValid())
        {
            drainInstance.set3DAttributes(RuntimeUtils.To3DAttributes(transform));
        }
    }
    private IEnumerator DrainDelay()
    {
        delaying = true;
        yield return new WaitForSeconds(drainDelay);
        delayDone = true;
        delaying = false;
    }

}
