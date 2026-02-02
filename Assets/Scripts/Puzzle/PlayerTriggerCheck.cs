using UnityEngine;
using UnityEngine.Events;

public class PlayerTriggerCheck : MonoBehaviour, IAbilityTarget
{
    [SerializeField] private bool demonStanding = false;
    [SerializeField] private AbilityType requieredAbility;
    [SerializeField] private ActivationMode activationMode;
    [SerializeField] private UnityEvent onActivated;
    [SerializeField] private UnityEvent onDeactivated;

    private bool isActive;

    public void AbilityStart(AbilityType ability)
    {
        if (ability != requieredAbility)
            return;

        if (demonStanding)
            return;

        if (activationMode == ActivationMode.Instant)
        {
            if (isActive) return;
            isActive = true;
            onActivated.Invoke();
        }
        else
        {
            if (isActive && demonStanding) return;
            isActive = true;
            onActivated.Invoke();
            demonStanding = false;
        }
    }

    public void AbilityEnd(AbilityType ability)
    {
        if (ability != requieredAbility)
            return;

        if (activationMode == ActivationMode.Hold && isActive)
        {
            isActive = false;
            onDeactivated.Invoke();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Demon"))
            demonStanding = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Demon"))
            demonStanding = false;
    }
}
