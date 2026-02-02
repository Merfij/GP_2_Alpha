using UnityEngine;
using UnityEngine.Events;

public class InteractableLever : MonoBehaviour, IInteractable, IActivator
{
    [SerializeField] private Animator animator;
    [SerializeField] private UnityEvent onActivated;
    [SerializeField] private UnityEvent onDeactivated;
    private bool activated;

    public UnityEvent Activated => onActivated;
    public UnityEvent Deactivated => onDeactivated;
    public bool IsActive => activated;


    private void Start()
    {
        activated = false;
    }

    public void Interact()
    {
        if (!activated)
        {
            animator.SetBool("Down", true);
            onActivated.Invoke();            
            activated = true;
            Debug.Log("lever on");
        }
        else
        {
            animator.SetBool("Down", false);
            onDeactivated.Invoke();
            activated = false;
            Debug.Log("Lever off");
        }
        
    }


}
