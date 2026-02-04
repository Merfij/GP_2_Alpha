using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDamage : MonoBehaviour
{
    public float givenDamage;

    private bool canAttack;

    private void Start()
    {

        canAttack = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Exorcist") && canAttack == true || other.CompareTag("Demon") && canAttack == true)
        {
            if (other.TryGetComponent<PlayerHealth>(out PlayerHealth health))
            {
                health.TakeDamage(givenDamage);
                Debug.Log("Damage given to player");
                canAttack = false;
                StartCoroutine(ResetAttack());
            }
            else
            {
                Debug.Log("No PlayerHealth component found on the player.");
            }
        }
    }

    IEnumerator ResetAttack()
    {
        yield return new WaitForSeconds(0.5f);
        canAttack = true;
    }
}
