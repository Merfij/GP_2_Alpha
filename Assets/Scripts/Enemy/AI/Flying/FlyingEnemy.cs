using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FlyingEnemy : MonoBehaviour, IDamagable, IAffectedByLaser
{
    [Header("FOV Settings")]
    public float radius = 12f;
    [Range(0, 360)] public float angle = 120f;
    public LayerMask targetMask;
    public LayerMask obstructionMask;
    public bool canSeePlayer;

    [Header("Enemy Target")]
    public List<Transform> target = new List<Transform>();

    [Header("Flying Movement")]
    public float moveSpeed = 6f;
    public float rotationSpeed = 6f;
    public float hoverHeight = 3f;

    [Header("Attack Pattern")]
    public float attackRange = 5f;
    public float circleRadius = 4f;
    public float circleDuration = 3f;
    public float circleSpeed = 120f;
    public float sweepSpeed = 14f;

    [Header("Return To Circle")]
    public float returnSpeed = 7f;
    public float returnThreshold = 0.2f;

    private Vector3 returnTarget;

    private float circleTimer;
    private Vector3 sweepTarget;

    private Vector3 frozenPosition;
    private Quaternion frozenRotation;
    //private Vector3 frozenOrbitDirection;
    //private Vector3 orbitDirection;

    [Header("Components")]
    [SerializeField] private Animator animator;
    public CapsuleCollider capsuleCollider;
    public SphereCollider damageCollider;
    //private Rigidbody rb;

    [Header("Health")]
    public float health = 100f;

    public bool canBeStunned = true;
    public bool isStunned;
    //private bool hasDoneInitialAttack = false;

    private EnemyState currentState;
    private EnemyState stateBeforeStun;

    private PlayerController playerExorcist;
    private PlayerController playerDemon;
    private ExorcistAbilities exorcistLaser;
    private DemonAbilities demonLaser;
    private bool isAttacking = false;

    void Start()
    {
        target.Add(GameObject.FindGameObjectWithTag("Exorcist").transform);
        target.Add(GameObject.FindGameObjectWithTag("Demon").transform);

        playerExorcist = target[0].GetComponent<PlayerController>();
        playerDemon = target[1].GetComponent<PlayerController>();

        exorcistLaser = playerExorcist.GetComponent<ExorcistAbilities>();
        demonLaser = playerDemon.GetComponent<DemonAbilities>();

        animator = GetComponent<Animator>();
        capsuleCollider = GetComponentInChildren<CapsuleCollider>();
        damageCollider.enabled = false;

        currentState = EnemyState.Chasing;
    }

    void Update()
    {
        if (isStunned)
        {
            transform.position = frozenPosition;
            transform.rotation = frozenRotation;
            animator.speed = 0;
            if (health <= 0)
            {
                Die();
                return;
            }

            if (!exorcistLaser.isLaserActive && !demonLaser.isLaserActive)
            {
                ResumeEnemy();
            }
            return;
        }
        else
        {
            animator.speed = 1;
            switch (currentState)
            {
                case EnemyState.Chasing:
                    animator.SetInteger("AIState", 0);
                    UpdateChase();
                    break;

                case EnemyState.Circling:
                    UpdateCircling();
                    break;

                case EnemyState.SweepingAttack:
                    animator.SetInteger("AIState", 1);
                    UpdateSweepAttack();
                    break;
                case EnemyState.ReturnToCircle:
                    animator.SetInteger("AIState", 0);
                    UpdateReturnToCircle();
                    break;

                case EnemyState.Electrocuted:
                    //animator.SetInteger("AIState", 4);
                    break;
            }
        }
        FieldOfViewCheck();

        if (health <= 0)
            Die();
        Debug.Log(exorcistLaser.isLaserActive + " " + demonLaser.isLaserActive);

        //if (!exorcistLaser.isLaserActive && !demonLaser.isLaserActive)
        //{
        //    ResumeEnemy();
        //}
    }

    private void UpdateChase()
    {
        if (isStunned) return;


        Transform closestTarget = ReturnClosestEnemy();
        if (closestTarget == null) return;

        Vector3 targetPos = closestTarget.position + Vector3.up * hoverHeight;
        MoveTowards(targetPos);

        if (Vector3.Distance(transform.position, closestTarget.position) <= attackRange)
        {
            sweepTarget = closestTarget.position;
            currentState = EnemyState.SweepingAttack;
        }
    }

    private void UpdateCircling()
    {
        if (isStunned) return;

        Transform target = ReturnClosestEnemy();
        if (target == null) return;

        circleTimer -= Time.deltaTime;

        // Get horizontal direction (ignore Y)
        Vector3 flatDir = transform.position - target.position;
        flatDir.y = 0f;

        // If directly above player, force a sideways direction
        if (flatDir.sqrMagnitude < 0.01f)
            flatDir = transform.right;

        flatDir.Normalize();

        // Rotate direction around player
        flatDir = Quaternion.AngleAxis(
            circleSpeed * Time.deltaTime,
            Vector3.up
        ) * flatDir;

        // Apply position
        Vector3 newPos =
            target.position +
            flatDir * circleRadius +
            Vector3.up * hoverHeight;

        transform.position = newPos;

        RotateTowards(target.position);

        if (circleTimer <= 0f)
        {
            sweepTarget = target.position;
            currentState = EnemyState.SweepingAttack;
        }
    }

    private void UpdateSweepAttack()
    {
        if (isStunned || isAttacking) return;

        transform.position = Vector3.MoveTowards(
            transform.position,
            sweepTarget,
            sweepSpeed * Time.deltaTime
        );

        RotateTowards(sweepTarget);
        if (Vector3.Distance(transform.position, sweepTarget) < 1.5f)
        {
            animator.SetInteger("AIState", 1);
        }
    }

    private void UpdateReturnToCircle()
    {
        if (isStunned) return;

        Transform target = ReturnClosestEnemy();
        if (target == null) return;

        transform.position = Vector3.MoveTowards(
            transform.position,
            returnTarget,
            returnSpeed * Time.deltaTime
        );

        RotateTowards(target.position);

        if (Vector3.Distance(transform.position, returnTarget) <= returnThreshold)
        {
            circleTimer = circleDuration;
            currentState = EnemyState.Circling;
        }
    }

    private void MoveTowards(Vector3 targetPos)
    {
        Vector3 dir = (targetPos - transform.position).normalized;
        transform.position += dir * moveSpeed * Time.deltaTime;
        RotateTowards(targetPos);
    }

    private void RotateTowards(Vector3 targetPos)
    {
        Vector3 dir = (targetPos - transform.position).normalized;
        Quaternion lookRot = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            lookRot,
            rotationSpeed * Time.deltaTime
        );
    }

    private Transform ReturnClosestEnemy()
    {
        Transform closest = null;
        float dist = Mathf.Infinity;

        foreach (Transform target in target)
        {
            float distanceFromPlayer = Vector3.Distance(transform.position, target.position);
            if (distanceFromPlayer < dist)
            {
                dist = distanceFromPlayer;
                closest = target;
            }
        }

        return closest;
    }

    private void FieldOfViewCheck()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, radius, targetMask);

        if (hits.Length == 0)
        {
            canSeePlayer = false;
            return;
        }

        Transform t = hits[0].transform;
        Vector3 dir = (t.position - transform.position).normalized;

        if (Vector3.Angle(transform.forward, dir) < angle / 2 &&
            !Physics.Raycast(transform.position, dir,
            Vector3.Distance(transform.position, t.position), obstructionMask))
        {
            canSeePlayer = true;
        }
        else
        {
            canSeePlayer = false;
        }
    }
    public void TakeDamage(float damage)
    {
        health -= damage;
    }

    public void AffectedByLaser()
    {
        if (!canBeStunned || isStunned) return;

        isStunned = true;
        stateBeforeStun = currentState;
        //currentState = EnemyState.Electrocuted;

        frozenPosition = transform.position;
        frozenRotation = transform.rotation;

        //if (!canBeStunned || isStunned) return;

        //isStunned = true;
        //stateBeforeStun = currentState;

        //frozenPosition = transform.position;
        //frozenRotation = transform.rotation; 
    }

    public void ResumeEnemy()
    {
        if (!isStunned) return;

        isStunned = false;
        currentState = stateBeforeStun;
        animator.speed = 1;
        //if (!isStunned) return;

        //isStunned = false;
        //currentState = stateBeforeStun;
    }

    private void Die()
    {
        //capsuleCollider.enabled = false;
        animator.enabled = false;
        enabled = false;
        gameObject.AddComponent<Rigidbody>();
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null) 
        {
            rb.useGravity = true;
            rb.AddForce(Vector3.down * 2f, ForceMode.Impulse);
        }

        // Optional: destroy after delay
        GetComponent<DeathNotifier>().NotifyDied();
        Destroy(gameObject, 1f);
    }

    public void ActivateEnemyCollider()
    {
        damageCollider.enabled = true;
        isAttacking = true;
    }

    public void DisableEnemyCollider()
    {
        damageCollider.enabled = false;
        isAttacking = false; 

        PrepareReturnToCircle();
    }

    private void PrepareReturnToCircle()
    {
        //hasDoneInitialAttack = true;
        Transform target = ReturnClosestEnemy();
        if (target == null) return;

        Vector3 flatDir = transform.position - target.position;
        flatDir.y = 0f;
        if (flatDir.sqrMagnitude < 0.01f) flatDir = transform.right;

        flatDir.Normalize();
        returnTarget = target.position + flatDir * circleRadius + Vector3.up * hoverHeight;

        currentState = EnemyState.ReturnToCircle;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}

