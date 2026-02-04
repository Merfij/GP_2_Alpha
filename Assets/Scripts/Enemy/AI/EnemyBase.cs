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

    protected GameObject playerExorcist;
    protected GameObject playerDemon;
    protected ExorcistAbilities exorcistLaser;
    protected DemonAbilities demonLaser;

    protected bool isStunned;
    protected bool canBeStunned = true;
    public GameObject healthPack;
    protected bool isDead = false;
    private static readonly int AISTateHash = Animator.StringToHash("AIState");

    [Header("Death")]
    [Tooltip("If true, the enemy will self-destruct after a set time.")]
    public bool shouldSelfDestruct = false;
    public float selfDestructAfter = 10f;
    [Tooltip("How long after death the enemy object is destroyed.")]
    public float destroyAfter = 1f;

    protected virtual void Start()
    {
        playerExorcist = GameObject.FindGameObjectWithTag("Exorcist");
        playerDemon = GameObject.FindGameObjectWithTag("Demon");

        if (playerExorcist != null)
        {
            exorcistLaser = playerExorcist.GetComponent<ExorcistAbilities>();
            targets.Add(playerExorcist.transform);
        }

        if (playerDemon != null)
        {
            demonLaser = playerDemon.GetComponent<DemonAbilities>();
            targets.Add(playerDemon.transform);
        }

        animator = GetComponent<Animator>();
        if(animator == null)
            Debug.LogError("Animator component not found on " + gameObject.name);
        agent = GetComponent<NavMeshAgent>();
        if(agent == null)
            Debug.LogError("NavMeshAgent component not found on " + gameObject.name);
        capsuleCollider = GetComponent<CapsuleCollider>();
        if(capsuleCollider == null)
            Debug.LogError("CapsuleCollider component not found on " + gameObject.name);

        handCollider.enabled = false;

        agent.stoppingDistance = attackRange;
        agent.isStopped = false;

        currentState = EnemyState.Chasing;
    }

    protected virtual void Update()
    {
        if (shouldSelfDestruct)
        {
            selfDestructAfter -= Time.deltaTime;
            if (selfDestructAfter <= 0f)
            {
                Die();
            }
        }

        switch (currentState)
        {
            case EnemyState.Chasing:
                animator.SetInteger(AISTateHash, 1);
                UpdateChase();
                break;

            case EnemyState.Attacking:
                animator.SetInteger(AISTateHash, 2);
                UpdateAttack();
                break;

            case EnemyState.Electrocuted:
                animator.SetInteger(AISTateHash, 3);
                UpdateInBeam();
                break;
        }

        

        FieldOfViewCheck();

        if (health <= 0)
            Die();

        if (!isDead && currentState == EnemyState.Electrocuted && !exorcistLaser.isLaserActive && !demonLaser.isLaserActive)
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
        agent.isStopped = false;
        Debug.Log($"{gameObject.name} current speed: {agent.speed} | Wanted speed: {chaseSpeed}");
        Transform target = GetClosestTarget();
        if (target == null) return;

        float distanceToTarget = Vector3.Distance(transform.position, target.position);

        float slowDownDistance = 1f;
        float startSlowingDistance = attackRange + slowDownDistance;

        if (distanceToTarget <= startSlowingDistance)
        {
            float t = (distanceToTarget - attackRange) / slowDownDistance;
            float minSpeed = chaseSpeed * 0.3f;
            agent.speed = Mathf.Lerp(minSpeed, chaseSpeed, t);
        }
        else
        {
            agent.speed = chaseSpeed;
        }

        agent.SetDestination(target.position);
        Debug.Log($"Dist: {distanceToTarget} | Target: {attackRange}");
        if (distanceToTarget < attackRange)
        {
            currentState = EnemyState.Attacking;
        }
    }

    protected virtual void UpdateAttack()
    {
        agent.isStopped = true;
        agent.velocity = Vector3.zero;
        agent.ResetPath();

        Transform target = GetClosestTarget();
        if (target == null)
        {
            currentState = EnemyState.Chasing; return;
        }

        float distanceToTarget = Vector3.Distance(transform.position, target.position);

        Vector3 dir = target.position - transform.position;
        dir.y = 0;
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            Quaternion.LookRotation(dir),
            Time.deltaTime * 5f
        );

        if (distanceToTarget > attackRange + 0.5f)
            currentState = EnemyState.Chasing; return;
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

        DisaleEnemyCollider();

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

        EnemyCountDebug.instance.DecrementEnemyCount();

        DisaleEnemyCollider();
        this.GetComponent<DeathNotifier>().NotifyDied();
        this.capsuleCollider.enabled = false;
        this.agent.updatePosition = false;
        this.agent.updateRotation = false;
        this.agent.enabled = false;
        //animator.enabled = false;
        this.animator.Play("Death");
        this.enabled = false;
        Destroy(this.gameObject, destroyAfter);
    }

    public void ActivateEnemyCollider()
    {
        handCollider.enabled = true;
    }

    public void DisaleEnemyCollider()
    {
        handCollider.enabled = false;
    }

    public void ForceResetState()
    {
        currentState = EnemyState.Chasing;
        if (agent != null && agent.isActiveAndEnabled)
        {
            agent.isStopped = false;
            // Optional: Clear the path so they don't slide toward the old position
            agent.ResetPath();
        }
    }
}