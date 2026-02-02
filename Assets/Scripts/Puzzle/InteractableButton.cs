using System;
using UnityEngine;
using UnityEngine.Events;

public class InteractableButton : MonoBehaviour, IInteractable, IActivator
{
    [SerializeField] private Animator animator;
    [SerializeField] private UnityEvent onPressed;
    [SerializeField] private UnityEvent onPressedAgain;
    private bool activated;
    public UnityEvent Activated => onPressed;
    public UnityEvent Deactivated => onPressedAgain;
    public bool IsActive => activated;


    private void Start()
    {
        activated = false;
    }

    public void Interact()
    {
        if (!activated)
        {
            onPressed.Invoke();
            animator.SetTrigger("Press");
            activated = true;
        }
        else
        {
            onPressedAgain.Invoke();
            animator.SetTrigger("Press");
            activated = false;
        }
        
    }
    
}
