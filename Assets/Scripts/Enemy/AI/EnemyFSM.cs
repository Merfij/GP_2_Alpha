//using Unity.VisualScripting;
//using UnityEngine;
//using UnityEngine.AI;
//using System.Collections.Generic;
//using System;

public enum EnemyState
{
    Patrolling,
    Chasing,
    Attacking,
    Electrocuted,
    Circling,
    SweepingAttack,
    ReturnToCircle
}

//public class EnemyFSM : MonoBehaviour
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
//    public float patrolSpeed;
//    public float chaseRange;
//    public float chaseSpeed;
//    public float attackRange;
//    public float waypointTolerance;

//    [Header("Animator")]
//    [SerializeField] private Animator animator;

//    [Header("Health")]
//    public float health;

//    private bool hasDied;
//    //[Header("Enemy Type")]
//    //public bool canBeStunned;
//    //private bool isStunned;
//    //public bool isEthereal;

//    //[Header("Materials")]
//    //public List<Material> materials;
//    //public SkinnedMeshRenderer skinnedMeshRenderer;

//    public Transform[] checkpoints;

//    public PlayerController playerExorcist;
//    public PlayerController playerDemon;
//    private ExorcistAbilities exorcistLaser;
//    private DemonAbilities demonLaser;

//    public SphereCollider handCollider;

//    int currentIndex = 0;
//    NavMeshAgent agent;

//    public EnemyState currentState;

//    private void Awake()
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
//        //skinnedMeshRenderer = GameObject.FindGameObjectWithTag("EnemySkin").GetComponent<SkinnedMeshRenderer>();
//    }

//    // Start is called once before the first execution of Update after the MonoBehaviour is created
//    void Start()
//    {
//        currentState = EnemyState.Patrolling;
//        if (checkpoints.Length > 0)
//        {
//            agent.SetDestination(checkpoints[currentIndex].position);
//        }

//        //skinnedMeshRenderer.material = materials[1];
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
//        //if (isEthereal == true)
//        //{
//        //    skinnedMeshRenderer.material = materials[1];
//        //}
//        //else
//        //{
//        //    skinnedMeshRenderer.material = materials[0];
//        //}

//        switch (currentState)
//        {
//            case EnemyState.Patrolling:
//                animator.SetInteger("AIState", 0);
//                UpdatePatrol();
//                break;
//            case EnemyState.Chasing:
//                animator.SetInteger("AIState", 1);
//                UpdateChase();
//                break;
//            case EnemyState.Attacking:
//                animator.SetInteger("AIState", 2);
//                UpdateAttack();
//                break;
//        }

//        FieldOfViewCheck();

//        if (!hasDied && health <= 0)
//        {
//            hasDied = true;         
//            Destroy(gameObject);
//        }
//    }

//    //private void UpdateElectrocuted()
//    //{
//    //    agent.isStopped = true;
//    //    isStunned = true;
//    //}

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

//        //if (player.health <= 0)
//        //{
//        //    currentState = EnemyState.Patrolling;
//        //}
//    }

//    private void UpdateChase()
//    {
//        Transform closestTarget = ReturnClosestEnemy();
//        if (canSeePlayer)
//        {
//            agent.isStopped = false;
//            float currentSpeed = Vector3.Distance(transform.position, closestTarget.transform.position);
//            if (currentSpeed > chaseSpeed)
//            {
//                currentSpeed = chaseSpeed;
//            }
//            agent.speed = currentSpeed;
//            agent.SetDestination(closestTarget.position);
//        }

//        if (agent.remainingDistance <= agent.stoppingDistance)
//        {
//            currentState = EnemyState.Attacking;
//            return;
//        }
//    }

//    private void UpdatePatrol()
//    {
//        agent.speed = patrolSpeed;

//        if (!agent.pathPending && agent.remainingDistance <= waypointTolerance)
//        {
//            currentIndex = (currentIndex + 1) % checkpoints.Length;
//            agent.SetDestination(checkpoints[currentIndex].position);
//        }

//        //if (player.health > 0)
//        //{
//            //if (Vector3.Distance(transform.position, target.position) < chaseRange || canSeePlayer == true)
//            //{
//            //    currentState = EnemyState.Chasing;
//            //    return;
//            //}

//        if (canSeePlayer == true)
//        {
//            currentState = EnemyState.Chasing;
//            return;
//        }
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

//    private Transform ReturnClosestEnemy()
//    {
//        Transform closestEnemy = null;
//        float closestDistance = Mathf.Infinity;

//        if (target.Count > 0)
//        {
//            foreach (Transform enemy in target)
//            {
//                float distance = Vector3.Distance(transform.position, enemy.position);

//                if (distance < closestDistance)
//                {
//                    closestDistance = distance;
//                    closestEnemy = enemy;
//                }
//            }
//        }
//        else Debug.LogWarning("No targets");

//            return closestEnemy;
//    }

    //public void StunEnemy()
    //{
    //    if (canBeStunned == true)
    //    {
    //        Debug.Log("Enemy Stunned");
    //        currentState = EnemyState.Electrocuted;
    //    } 
    //}

    //public void ResumeEnemy()
    //{
    //    if (canBeStunned == true)
    //    {
    //        currentState = EnemyState.Patrolling;
    //    }
    //}

    //public void EtherealEnemy()
    //{
    //    if (isEthereal == true)
    //    {
    //        skinnedMeshRenderer.material = materials[0];
    //    }
    //}

    //public void TakeDamage(int damage)
    //{
    //    if (canBeStunned == true)
    //    {
    //        if (isStunned == true)
    //        {
    //            health -= damage;
    //        }
    //        else return;
    //    }
    //    else
    //    {
    //        health -= damage;
    //    }
    //}
//}