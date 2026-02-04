using UnityEngine;

public class EnemyFSM_Shifting_New : EnemyBase
{
    public override void TakeDamage(float damage)
    {
        if (isStunned == false)
        {
            Transform closestTarget = GetClosestTarget();
            OnHitShiftToNewPos(closestTarget);
        }
        else base.TakeDamage(damage);
    }

    public override void AffectedByLaser()
    {
        base.AffectedByLaser();
    }

    public void OnHitShiftToNewPos(Transform target)
    {
        float min = 10;
        float max = 25;

        float sign = (Random.value > 0.5f) ? 1f : -1f;

        float randomfloat = Random.Range(min, max) * sign;
        Debug.Log("Random float generated: " + randomfloat);
        Vector3 randomPOS = new Vector3(target.position.x + randomfloat, target.position.y, transform.position.z);
        transform.position = randomPOS;
    }
}
