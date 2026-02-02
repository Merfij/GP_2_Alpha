using System;
using UnityEngine;
using UnityEngine.Events;

public class PressureSwitch : MonoBehaviour, IActivator
{
    [SerializeField] private Animator animator;
    [SerializeField] private UnityEvent onPressed;
    [SerializeField] private UnityEvent onReleased;
    [SerializeField] private ActivationMode activationMode;
    private bool activated = false;
    public UnityEvent Activated => onPressed;
    public UnityEvent Deactivated => onReleased;
    public bool IsActive => activated;

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Demon") || other.CompareTag("Exorcist"))
        {
            animator.SetBool("Down", true);
            onPressed.Invoke();
            GameEvents.OnPressureplatePress?.Invoke(transform);
            activated = true;
        }        
    }

    private void OnTriggerExit(Collider other)
    {
        if (activationMode == ActivationMode.Instant)
            return;

        animator.SetBool("Down", false);        
        onReleased.Invoke();
        GameEvents.OnPressureplateRelease?.Invoke(transform);
        activated = false;
    }

}
