using UnityEngine;
public class Vampire : EnemyBase
{
    [Header("Ethereal Settings")]
    [SerializeField] private float minStunTime = 2f;
    [SerializeField] private float maxStunTime = 5f;
    [SerializeField] private float stunTime;
    [SerializeField] private int canSeeLayer;
    [SerializeField] private int cantSeeLayer;


    public void TakeTrapDamage(float damage)
    {
        // Bypasses the teleport logic and just subtracts health
        this.health -= damage;
    }

    protected override void Update()
    {
        base.Update();
        HandleStunTimer();

        if (isStunned)
        {
            bool bothLasersActive = (exorcistLaser != null && exorcistLaser.isLaserActive) && (demonLaser != null && demonLaser.isLaserActive);

            if (!bothLasersActive)
            {
                ResumeEnemy();
            }
        }
    }
    public override void AffectedByLaser()
    {
        bool isDemonHitting = demonLaser != null && demonLaser.isLaserActive;
        bool isExorcistHitting = exorcistLaser != null && exorcistLaser.isLaserActive;

        // Only trigger the "Electrocuted" state if both are hitting and it's visible
        if (isExorcistHitting && isDemonHitting && gameObject.layer == canSeeLayer)
        {
            if (!isStunned && canBeStunned)
            {
                base.AffectedByLaser();
            }
        }
    }

    public override void TakeDamage(float damage)
    {
        // Requirement: Must be stunned (Exorcist) AND visible (Demon) to take damage
        if (isStunned && gameObject.layer == canSeeLayer)
        {
            base.TakeDamage(damage);
        }
        else
        {
            // If hit but conditions aren't met, teleport away
            Transform closestTarget = GetClosestTarget();
            if (closestTarget != null)
            {
                OnHitShiftToNewPos(closestTarget);
            }
        }
    }
    private void HandleStunTimer()
    {
        bool isDemonHitting = demonLaser != null && demonLaser.isLaserActive;

        if (isDemonHitting)
        {
            stunTime += Time.deltaTime * 2;
            if (stunTime > maxStunTime) stunTime = maxStunTime;

            SetLayerRecursively(this.gameObject, canSeeLayer);
        }
        else
        {
            stunTime -= Time.deltaTime;
            if (stunTime < 0) stunTime = 0;

            if (stunTime > 0)
                SetLayerRecursively(this.gameObject, canSeeLayer);
            else
                SetLayerRecursively(this.gameObject, cantSeeLayer);
        }
    }

    public override void ResumeEnemy()
    {
        if (this == null) return;
        if (!enabled || agent == null || !agent.isActiveAndEnabled || !agent.isOnNavMesh)
            return;

        base.ResumeEnemy();
    }

    public void OnHitShiftToNewPos(Transform target)
    {
        float randomfloat = Random.Range(-12, 12);
        Vector3 randomPOS = new Vector3(target.position.x + randomfloat, target.position.y, transform.position.z);

        if (agent != null && agent.isActiveAndEnabled && agent.isOnNavMesh)
        {
            agent.Warp(randomPOS);
        }
    }

    private void SetLayerRecursively(GameObject obj, int layer)
    {
        if (obj.layer == layer) return;
        obj.layer = layer;
        foreach (Transform child in obj.transform)
            SetLayerRecursively(child.gameObject, layer);
    }
}