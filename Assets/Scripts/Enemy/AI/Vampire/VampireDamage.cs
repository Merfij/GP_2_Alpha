using UnityEngine;

public class VampireDamage : MonoBehaviour
{
    public float damageToVampire = 10f;

    private void OnTriggerStay(Collider other)
    {
        if (other.GetComponentInParent<Vampire>())
        {
            Debug.Log("Vampire found via tag");
        }
        else
        {
            Debug.Log("Nothing found");
        }
        Vampire vampire = other.GetComponentInParent<Vampire>();

        if (vampire != null)
        {
            // Check the state via the method we added to EnemyBase
            if (vampire.GetCurrentState() == EnemyState.Electrocuted)
            {
                vampire.TakeTrapDamage(damageToVampire * Time.deltaTime);
                Debug.Log("Vampire is in trap and STUNNED. Dealing damage.");
            }
            else
            {
                Debug.Log("Vampire in trap, but NOT stunned.");
            }
        }
    }
}
