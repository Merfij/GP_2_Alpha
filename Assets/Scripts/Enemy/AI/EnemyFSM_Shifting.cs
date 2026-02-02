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
        float randomfloat = Random.Range(-12, 12);
        Vector3 randomPOS = new Vector3(target.position.x + randomfloat, target.position.y, transform.position.z);
        transform.position = randomPOS;
    }
}

//using System;
//using System.Collections.Generic;
//using Unity.VisualScripting;
//using UnityEngine;
//using UnityEngine.AI;
//using Random = UnityEngine.Random;

//public class EnemyFSM_Shifting : MonoBehaviour, IAffectedByLaser, IDamagable
//{
//    [Header("FOV Settings")]
//    public float radius;
//    [UnityEngine.Range(0, 360)]
//    public float angle;
//    public LayerMask targetMask;
//    public LayerMask obstructionMask;
//    public bool canSeePlayer;
//    //public GameObject playerRef;
//    public event Action OnDied;
//    [Header("Enemy Target")]
//    public List<Transform> target;

//    [Header("Chase Range")]
//    public float patrolSpeed;
//    public float chaseRange;
//    public float chaseSpeed;
//    public float attackRange;
//    public float waypointTolerance;

//    [Header("Animator")]
//    [SerializeField] private Animator animator;

//    [Header("Health")]
//    public float health;
//    public float deathTimer;

//    public bool canBeStunned;
//    public bool isStunned;

//    //public Transform[] checkpoints;

//    public PlayerController playerExorcist;
//    public PlayerController playerDemon;
//    private ExorcistAbilities exorcistLaser;
//    private DemonAbilities demonLaser;

//    public SphereCollider handCollider;

//    //int currentIndex = 0;
//    NavMeshAgent agent;

//    public EnemyState currentState;
//    private EnemyState stateBeforeStun;

//    public CapsuleCollider capsuleCollider;
//    public EnemyFSM_Shifting script;
//    private bool hasDied;

//    // Start is called once before the first execution of Update after the MonoBehaviour is created
//    void Start()
//    {
//        target.Add(GameObject.FindGameObjectWithTag("Exorcist").GetComponent<Transform>());
//        target.Add(GameObject.FindGameObjectWithTag("Demon").GetComponent<Transform>());
//        playerExorcist = GameObject.FindGameObjectWithTag("Exorcist").GetComponent<PlayerController>();
//        playerDemon = GameObject.FindGameObjectWithTag("Demon").GetComponent<PlayerController>();
//        exorcistLaser = playerExorcist.GetComponent<ExorcistAbilities>();
//        demonLaser = playerDemon.GetComponent<DemonAbilities>();
//        handCollider.enabled = false;
//        animator = GetComponent<Animator>();
//        agent = GetComponent<NavMeshAgent>();
//        agent.stoppingDistance = attackRange;
//        agent.autoBraking = true;
//        canBeStunned = true;

//        currentState = EnemyState.Chasing;

//    }
//    private void OnDrawGizmos()
//    {
//        Gizmos.color = Color.red;
//        Gizmos.DrawWireSphere(transform.position, radius);
//        Vector3 fovLine1 = Quaternion.AngleAxis(angle / 2, transform.up) * transform.forward * radius;
//        Vector3 fovLine2 = Quaternion.AngleAxis(-angle / 2, transform.up) * transform.forward * radius;
//        Gizmos.color = Color.yellow;
//        Gizmos.DrawRay(transform.position, fovLine1);
//        Gizmos.DrawRay(transform.position, fovLine2);
//        if (canSeePlayer)
//        {
//            Gizmos.color = Color.green;
//            //Gizmos.DrawRay(transform.position, (target.position - transform.position).normalized * radius);
//        }
//    }
//    // Update is called once per frame
//    void Update()
//    {
//        switch (currentState)
//        {
//            //case EnemyState.Patrolling:
//            //    animator.SetInteger("AIState", 0);
//            //    UpdatePatrol();
//            //    break;
//            case EnemyState.Chasing:
//                animator.SetInteger("AIState", 1);
//                UpdateChase();
//                break;
//            case EnemyState.Attacking:
//                animator.SetInteger("AIState", 2);
//                UpdateAttack();
//                break;
//            case EnemyState.Electrocuted:
//                animator.SetInteger("AIState", 3);
//                UpdateInBeam();
//                break;
//        }

//        FieldOfViewCheck();

//        if (health <= 0 && !hasDied)
//        {
//            hasDied = true;
//            GetComponent<DeathNotifier>().NotifyDied();
//            capsuleCollider.enabled = false;
//            agent.updatePosition = false;
//            agent.updateRotation = false;
//            agent.enabled = false;
//            animator.enabled = false;
//            script.enabled = false;

//            Destroy(gameObject, deathTimer);
//        }

//        if (!exorcistLaser.isLaserActive && !demonLaser.isLaserActive && currentState == EnemyState.Electrocuted)
//        {
//            ResumeEnemy();
//        }

//    }

//    private void UpdateInBeam()
//    {
//        agent.isStopped = true;
//    }

//    private void FieldOfViewCheck()
//    {
//        Collider[] rangeChecks = Physics.OverlapSphere(transform.position, radius, targetMask);

//        if (rangeChecks.Length != 0)
//        {
//            Transform target = rangeChecks[0].transform;
//            Vector3 directionToTarget = (target.position - transform.position).normalized;
//            if (Vector3.Angle(transform.forward, directionToTarget) < angle / 2)
//            {
//                float distanceToTarget = Vector3.Distance(transform.position, target.position);
//                if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, obstructionMask))
//                    canSeePlayer = true;
//                else
//                    canSeePlayer = false;
//            }
//            else
//                canSeePlayer = false;
//        }
//        else if (canSeePlayer)
//            canSeePlayer = false;
//    }

//    private void UpdateAttack()
//    {
//        agent.isStopped = true;
//        agent.ResetPath();

//        Transform closestTarget = ReturnClosestEnemy();

//        // Face the player
//        Vector3 lookDir = closestTarget.position - transform.position;
//        lookDir.y = 0;
//        transform.rotation = Quaternion.Slerp(
//            transform.rotation,
//            Quaternion.LookRotation(lookDir),
//            Time.deltaTime * 5f
//        );

//        if (Vector3.Distance(transform.position, closestTarget.position) > attackRange + 0.5f)
//        {
//            currentState = EnemyState.Chasing;
//            return;
//        }
//    }

//    private void UpdateChase()
//    {
//        Transform closestTarget = ReturnClosestEnemy();

//        if (!canSeePlayer || closestTarget == null)
//        {
//            Debug.LogWarning("No target");
//        }

//        agent.isStopped = false;

//        float currentSpeed = Vector3.Distance(transform.position, closestTarget.transform.position);
//        if (currentSpeed > chaseSpeed)
//        {
//            currentSpeed = chaseSpeed;
//        }
//        agent.speed = currentSpeed;
//        agent.SetDestination(closestTarget.position);

//        if (agent.pathPending) return;
//        if (!agent.hasPath) return;

//        float distanceToTarget = Vector3.Distance(transform.position, closestTarget.position);

//        if (distanceToTarget <= attackRange)
//        {
//            currentState = EnemyState.Attacking;
//        }
//    }

//    public void ActivateEnemyCollider()
//    {
//        handCollider.enabled = true;
//    }

//    public void DisaleEnemyCollider()
//    {
//        handCollider.enabled = false;
//    }

//    public void AffectedByLaser()
//    {
//        if (!canBeStunned || isStunned) return;
//        agent.speed = 0;
//        Transform stopPos = this.transform;
//        agent.SetDestination(stopPos.position);
//        isStunned = true;
//        stateBeforeStun = currentState;   // Save previous state
//        currentState = EnemyState.Electrocuted;
//    }

//    public void ResumeEnemy()
//    {
//        //if (canBeStunned == true)
//        //{
//        //    isStunned = false;
//        //}

//        if (!isStunned) return;

//        Debug.Log("Enemy Resumed");

//        isStunned = false;
//        agent.isStopped = false;
//        currentState = stateBeforeStun;
//    }

//    public void TakeDamage(float damage)
//    {
//        if (isStunned == false)
//        {
//            Transform target = ReturnClosestEnemy();
//            OnHitShiftToNewPos(target);
//        }
//        else
//        {
//            health -= damage;
//        }
//    }

//    public void OnHitShiftToNewPos(Transform target)
//    {
//        float randomfloat = Random.Range(-12, 12);
//        Vector3 randomPOS = new Vector3(target.position.x + randomfloat, target.position.y, transform.position.z);
//        transform.position = randomPOS;
//    }

//    private Transform ReturnClosestEnemy()
//    {
//        Transform closestEnemy = null;
//        float closestDistance = Mathf.Infinity;

//        foreach (Transform enemy in target)
//        {
//            float distance = Vector3.Distance(transform.position, enemy.position);

//            if (distance < closestDistance)
//            {
//                closestDistance = distance;
//                closestEnemy = enemy;
//            }
//        }

//        return closestEnemy;
//    }
//}
