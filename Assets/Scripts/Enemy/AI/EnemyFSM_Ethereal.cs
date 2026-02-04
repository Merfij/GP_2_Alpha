using Unity.VisualScripting;
using UnityEngine;

public class EnemyFSM_Ethereal : EnemyBase
{
    [Header("Minimum stun time")]
    [SerializeField] private float minStunTime;
    [SerializeField] private float maxStunTime;
    [SerializeField] private float stunTime;

    [Header("Layers")]
    [SerializeField] private int canSeeLayer;
    [SerializeField] private int cantSeeLayer;
    private int currentLayerCache = -1;

    private new void Update()
    {
        base.Update();

        if (isDead) return;
        HandleStunTimer();
    }
    public override void TakeDamage(float damage)
    {
        if (gameObject.layer == cantSeeLayer)
        {
            return;
        }
        else
        {
            base.TakeDamage(damage);
        }
    }

    public override void AffectedByLaser()
    {
        if (!canBeStunned || isDead) return;

        //if (agent != null && agent.enabled && agent.isOnNavMesh)
        //{
        //    agent.isStopped = false;
        //}

        isStunned = true;
        if (stunTime <= 0)
        {
            stunTime += minStunTime;
        }
    }

    public override void ResumeEnemy()
    {
        base.ResumeEnemy();

        if (agent == null || !agent.enabled || !agent.isOnNavMesh)
        {
            return;
        }

        Transform target = GetClosestTarget();
        if (target != null)
        {
            this.agent.isStopped = false;
            this.agent.SetDestination(target.position);
        }
    }

    private void HandleStunTimer()
    {
        // The timer always ticks. It ticks UP if hit, DOWN if not hit.
        if (isStunned)
        {
            stunTime = Mathf.Clamp(stunTime + (Time.deltaTime * 2), 0, maxStunTime);

            // IMPORTANT: Reset isStunned to false immediately. 
            // If the laser hits again next frame, AffectedByLaser() will set it back to true.
            // This creates the "toggle" you want.
            isStunned = false;
        }
        else
        {
            stunTime = Mathf.Max(0, stunTime - Time.deltaTime);
        }

        // Layer swapping logic stays the same
        int targetLayer = stunTime > 0 ? canSeeLayer : cantSeeLayer;
        if (currentLayerCache != targetLayer)
        {
            SetLayerRecursively(this.gameObject, targetLayer);
            currentLayerCache = targetLayer;
        }
    }

    private void SetLayerRecursively(GameObject obj, int layer)
    {
        obj.layer = layer;
        foreach (Transform child in obj.transform)
            SetLayerRecursively(child.gameObject, layer);
    }
}


//------------------------Old Version-------------------------------------------
//using System;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.AI;

//public class EnemyFSM_Ethereal : MonoBehaviour, IDamagable, IAffectedByLaser
//{
//    [Header("FOV Settings")]
//    public float radius;
//    [UnityEngine.Range(0, 360)]
//    public float angle;
//    public LayerMask targetMask;
//    public LayerMask obstructionMask;
//    public bool canSeePlayer;
//    //public GameObject playerRef;

//    [Header("Enemy Target")]
//    public List<Transform> target;

//    [Header("Chase Range")]
//    //public float patrolSpeed;
//    //public float chaseRange;
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

//    public PlayerController playerExorcist;
//    public PlayerController playerDemon;
//    private ExorcistAbilities exorcistLaser;
//    private DemonAbilities demonLaser;

//    public SphereCollider handCollider;

//    public EnemyFSM_Ethereal script;
//    public CapsuleCollider capsuleCollider;

//    //int currentIndex = 1;
//    NavMeshAgent agent;

//    public EnemyState currentState;
//    private EnemyState stateBeforeStun;

//    public LayerMask layer;
//    private string etherealLayer = "Etherial";
//    private string enemyLayer = "EtherealSeen";
//    int canSeeLayer;
//    int cantSeeLayer;

//    public float stunTimer;
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
//        script = GetComponent<EnemyFSM_Ethereal>();
//        capsuleCollider = GetComponent<CapsuleCollider>();
//        handCollider.enabled = false;
//        animator = GetComponent<Animator>();
//        agent = GetComponent<NavMeshAgent>();
//        agent.stoppingDistance = attackRange;
//        agent.autoBraking = true;
//        canBeStunned = true;
//        canSeeLayer = LayerMask.NameToLayer(enemyLayer);
//        cantSeeLayer = LayerMask.NameToLayer(etherealLayer);

//        SetLayerRecursively(this.gameObject, cantSeeLayer);

//        currentState = EnemyState.Chasing;
//        if (target.Count <= 0)
//        {
//            Debug.LogWarning("Enemies dead or no target set");
//        }
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
//            case EnemyState.Chasing:
//                animator.SetInteger("AIState", 1);
//                UpdateChase();
//                break;
//            case EnemyState.Attacking:
//                animator.SetInteger("AIState", 2);
//                UpdateAttack();
//                break;
//            //case EnemyState.Electrocuted:
//            //    animator.SetInteger("AIState", 3);
//            //    UpdateInBeam();
//            //    break;
//        }

//        FieldOfViewCheck();

//        if (!hasDied && health <= 0)
//        {
//            Debug.Log("Enemy Killed");
//            GetComponent<DeathNotifier>().NotifyDied();
//            hasDied = true;
//            capsuleCollider.enabled = false;
//            agent.updatePosition = false;
//            agent.updateRotation = false;
//            agent.enabled = false;
//            animator.enabled = false;
//            script.enabled = false;

//            Destroy(gameObject, deathTimer);
//        }

//        if (!exorcistLaser.isLaserActive && !demonLaser.isLaserActive)
//        {
//            ResumeEnemy();
//        }

//        if (stunTimer > 0)
//        {
//            SetLayerRecursively(this.gameObject, canSeeLayer);
//        } else
//        {
//            SetLayerRecursively(this.gameObject, cantSeeLayer);
//        }

//        if (isStunned == true)
//        {
//            stunTimer += Time.deltaTime * 2;
//            SetLayerRecursively(this.gameObject, canSeeLayer);
//        }
//        else
//        {
//            stunTimer -= Time.deltaTime;
//            //SetLayerRecursively (this.gameObject, cantSeeLayer);
//            if (stunTimer < 0)
//            {
//                stunTimer = 0;
//            }
//        }
//    }

//    //private void UpdateInBeam()
//    //{
//    //    agent.isStopped = true;
//    //}

//    void SetLayerRecursively(GameObject obj, int layer)
//    {
//        obj.layer = layer;

//        foreach (Transform child in obj.transform)
//        {
//            SetLayerRecursively(child.gameObject, layer);
//        }
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
//            lookDir.y = 0;
//            transform.rotation = Quaternion.Slerp(
//                transform.rotation,
//                Quaternion.LookRotation(lookDir),
//                Time.deltaTime * 5f
//            );

//            if (Vector3.Distance(transform.position, closestTarget.position) > attackRange + 0.5f)
//            {
//                currentState = EnemyState.Chasing;
//                return;
//            }
//    }

//    private void UpdateChase()
//    {
//        Transform closestTarget = ReturnClosestEnemy();

//        if(!canSeePlayer || closestTarget == null)
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

//        //if (canSeePlayer)
//        //{
//        //        agent.isStopped = false;
//        //        float currentSpeed = Vector3.Distance(transform.position, closestTarget.transform.position);
//        //        if (currentSpeed > chaseSpeed)
//        //        {
//        //            currentSpeed = chaseSpeed;
//        //        }
//        //        agent.speed = currentSpeed;
//        //        agent.SetDestination(closestTarget.position);
//        //}

//        //if (agent.remainingDistance <= agent.stoppingDistance)
//        //{
//        //    currentState = EnemyState.Attacking;
//        //    return;
//        //}
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
//        //agent.speed = 0;
//        //Transform stopPos = this.transform;
//        //agent.SetDestination(stopPos.position);
//        isStunned = true;
//        //stateBeforeStun = currentState;   // Save previous state
//        //currentState = EnemyState.Electrocuted;
//    }

//    public void ResumeEnemy()
//    {
//        if (!isStunned) return;

//        isStunned = false;
//        //agent.isStopped = false;
//        //currentState = stateBeforeStun;
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

//    public void TakeDamage(float damage)
//    {
//        if (gameObject.layer == cantSeeLayer)
//        {
//            return;
//        }
//        else
//        {
//            health -= damage;
//        }
//    }
//}
