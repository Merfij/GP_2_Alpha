using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Door : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private List<MonoBehaviour> activators;
    [SerializeField] private int requiredActive;

    public bool onceActiveStaysActive = false;
    
    private HashSet<IActivator> activeActivators = new HashSet<IActivator>();
    
    private bool isOpen;

    private void Awake()
    {
        foreach (var mono in activators)
        {
            var activator = mono as IActivator;
            if (activator == null) continue;

            activator.Activated.AddListener(() => ActivatorOn(activator));
            activator.Deactivated.AddListener(() => ActivatorOff(activator));

            if(activator.IsActive)
                activeActivators.Add(activator); 
        }
        EvaluateDoor();
    }

    void ActivatorOn(IActivator activator)
    {
        if (!activeActivators.Add(activator))
            return;

        EvaluateDoor();
    }

    void ActivatorOff(IActivator activator)
    {
        if (!activeActivators.Remove(activator))
            return;

        EvaluateDoor();
    }

    void EvaluateDoor()
    {
        if(activeActivators.Count >= requiredActive && !isOpen)
        {
            Open();
        }
        else if(activeActivators.Count < requiredActive && isOpen)
        {
            if (!onceActiveStaysActive)
            {
                Close();
            }
        }
    }

    public void Open()
    {
        animator.SetBool("Open", true);
        GameEvents.OnDoorOpen?.Invoke(transform);
        Debug.Log("door open");
        isOpen = true;
    }

    public void Close()
    {
        animator.SetBool("Open", false);
        GameEvents.OnDoorClose?.Invoke(transform);
        Debug.Log("Door closed");
        isOpen = false;
    }
}
