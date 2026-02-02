using UnityEngine;
using UnityEngine.Events;

public interface IAbilityTarget
{
    void AbilityStart(AbilityType ability);
    void AbilityEnd(AbilityType ability);
}

public interface IInteractable
{
    void Interact();    
}

public interface IActivator
{
    bool IsActive { get; }
    UnityEvent Activated { get; }
    UnityEvent Deactivated { get; }
}
