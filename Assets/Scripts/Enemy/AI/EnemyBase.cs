using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class EnemyBase : MonoBehaviour, IDamagable, IAffectedByLaser
{
    [Header("FOV")]
    public float radius;
    [Range(0, 360)] public float angle;
    public LayerMask targetMask;
    public LayerMask obstructionMask;
    protected bool canSeePlayer;

    [Header("Combat")]
    public float chaseSpeed;
    public float attackRange;
    public float health;

    [Header("References")]
    protected Animator animator;
    protected NavMeshAgent agent;
    protected CapsuleCollider capsuleCollider;
    public SphereCollider handCollider;
    public EnemyBase script;

    protected List<Transform> targets = new();
    protected EnemyState currentState;
    protected EnemyState stateBeforeStun;

    protected PlayerController playerExorcist;
    protected PlayerController playerDemon;
    protected ExorcistAbilities exorcistLaser;
    protected DemonAbilities demonLaser;

    protected bool isStunned;
    protected bool canBeStunned = true;
    public GameObject healthPack;
    protected bool isDead = false;

    protected virtual void Start()
    {
        playerExorcist = GameObject.FindGameObjectWithTag("Exorcist").GetComponent<PlayerController>();
        playerDemon = GameObject.FindGameObjectWithTag("Demon").GetComponent<PlayerController>();
        exorcistLaser = playerExorcist.GetComponent<ExorcistAbilities>();
        demonLaser = playerDemon.GetComponent<DemonAbilities>();

        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        capsuleCollider = GetComponent<CapsuleCollider>();

        handCollider.enabled = false;

        targets.Add(GameObject.FindGameObjectWithTag("Exorcist").transform);
        targets.Add(GameObject.FindGameObjectWithTag("Demon").transform);

        agent.stoppingDistance = attackRange;
        agent.autoBraking = true;

        currentState = EnemyState.Chasing;
    }

    protected virtual void Update()
    {
        switch (currentState)
        {
            case EnemyState.Chasing:
                animator.SetInteger("AIState", 1);
                UpdateChase();
                break;

            case EnemyState.Attacking:
                animator.SetInteger("AIState", 2);
                UpdateAttack();
                break;

            case EnemyState.Electrocuted:
                animator.SetInteger("AIState", 3);
                UpdateInBeam();
                break;
        }

        

        FieldOfViewCheck();

        if (health <= 0)
            Die();

        if (!isDead && !exorcistLaser.isLaserActive && !demonLaser.isLaserActive)
        {
            ResumeEnemy();
        }
    }

    public virtual EnemyState GetCurrentState()
    {
        return currentState;
    }

    protected virtual void UpdateInBeam()
    {
        // Default stun behavior: stop moving
        if (agent != null)
            agent.isStopped = true;
    }

    protected virtual void UpdateChase()
    {
        Transform target = GetClosestTarget();
        if (target == null) return;

        agent.isStopped = false;
        agent.speed = Mathf.Min(chaseSpeed, Vector3.Distance(transform.position, target.position));
        agent.SetDestination(target.position);

        if (Vector3.Distance(transform.position, target.position) <= attackRange)
            currentState = EnemyState.Attacking;
    }

    protected virtual void UpdateAttack()
    {
        agent.isStopped = false;
        agent.ResetPath();

        Transform target = GetClosestTarget();
        if (target == null) return;

        Vector3 dir = target.position - transform.position;
        dir.y = 0;
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            Quaternion.LookRotation(dir),
            Time.deltaTime * 5f
        );

        if (Vector3.Distance(transform.position, target.position) > attackRange + 0.5f)
            currentState = EnemyState.Chasing;
    }

    protected Transform GetClosestTarget()
    {
        
        Transform closest = null;
        float minDist = Mathf.Infinity;

        foreach (var target in targets)
        {
            float distance = Vector3.Distance(transform.position, target.position);
            if (distance < minDist)
            {
                minDist = distance;
                closest = target;
            }
        }
        return closest;
    }

    protected void FieldOfViewCheck()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, radius, targetMask);
        canSeePlayer = false;

        if (hits.Length == 0) return;

        Vector3 dir = (hits[0].transform.position - transform.position).normalized;
        if (Vector3.Angle(transform.forward, dir) < angle / 2 &&
            !Physics.Raycast(transform.position, dir,
                Vector3.Distance(transform.position, hits[0].transform.position),
                obstructionMask))
        {
            canSeePlayer = true;
        }
    }

    public virtual void TakeDamage(float damage)
    {
        health -= damage;
    }

    public virtual void AffectedByLaser()
    {
        if (!canBeStunned || isStunned) return;

        isStunned = true;
        stateBeforeStun = currentState;
        currentState = EnemyState.Electrocuted;
    }

    public virtual void ResumeEnemy()
    {
        if (!isStunned || isDead) return;

        isStunned = false;
        agent.isStopped = false;
        currentState = stateBeforeStun;
    }

    protected virtual void Die()
    {
        //capsuleCollider.enabled = false;
        //agent.enabled = false;
        //animator.enabled = false;
        //enabled = false;
        //Destroy(gameObject, 2f);
        if (isDead) return;
        isDead = true;

        if (isStunned) Instantiate(healthPack, this.transform.position, this.transform.rotation);

        this.GetComponent<DeathNotifier>().NotifyDied();
        this.capsuleCollider.enabled = false;
        this.agent.updatePosition = false;
        this.agent.updateRotation = false;
        this.agent.enabled = false;
        //animator.enabled = false;
        this.animator.Play("Death");
        this.enabled = false;
        Destroy(this.gameObject, 1f);
    }

    public void ActivateEnemyCollider()
    {
        handCollider.enabled = true;
    }

    public void DisaleEnemyCollider()
    {
        handCollider.enabled = false;
    }
}