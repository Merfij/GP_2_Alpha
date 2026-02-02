using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDamage : MonoBehaviour
{
    public float givenDamage;
    public List<PlayerController> target;

    public bool canAttack;

    private void Start()
    {
        target.Add(GameObject.FindGameObjectWithTag("Exorcist").GetComponent<PlayerController>());
        target.Add(GameObject.FindGameObjectWithTag("Demon").GetComponent<PlayerController>());

        canAttack = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Exorcist") || other.CompareTag("Demon") && canAttack == true)
        {
            PlayerHealth health = other.GetComponent<PlayerHealth>();
            health.TakeDamage(givenDamage);
            Debug.Log("Damage given to player");
            canAttack = false;
            StartCoroutine(ResetAttack());
        }
    }

    IEnumerator ResetAttack()
    {
        yield return new WaitForSeconds(1);
        canAttack = true;
    }
}
