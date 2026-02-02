//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class FlyingAI : MonoBehaviour
//{
//    [Header("References")]
//    public PlayerController playerExorcist;
//    public PlayerController playerDemon;
//    private ExorcistAbilities exorcistLaser;
//    private DemonAbilities demonLaser;
//    private List<Transform> target;
//    [SerializeField] private WaypointHolder waypointHolder;

//    [Header("Movement Settings")]
//    [SerializeField] private float moveSpeed = 20f;
//    [SerializeField] private float rotationSpeed = 7.5f;
//    [SerializeField] private float circleDuration = 5f;
//    [SerializeField] private float waypointDistanceThreshold = 3f;

//    [Header("Attack Settings")]
//    [SerializeField] private float attackRange = 30f;
//    [SerializeField] private float attackDuration = 3f;
//    [SerializeField] private float angleToAttackPlayer = 0.1f;
//    [SerializeField] private float shootInterval = 0.5f;
        
//    private Transform currenWaypointTarget;
//    private Transform[] waypoints;

//    private void Start()
//    {
//        target.Add(GameObject.FindGameObjectWithTag("Demon").transform);
//        target.Add(GameObject.FindGameObjectWithTag("Exorcist").transform);
//        playerExorcist = GameObject.FindGameObjectWithTag("Exorcist").GetComponent<PlayerController>();
//        playerDemon = GameObject.FindGameObjectWithTag("Demon").GetComponent<PlayerController>();
//        exorcistLaser = playerExorcist.GetComponent<ExorcistAbilities>();
//        demonLaser = playerDemon.GetComponent<DemonAbilities>();

//        if (waypointHolder != null)
//        {
//            waypointHolder.RefreshWaypoints();
//            waypoints = waypointHolder.waypoints;
//        }

//        if (waypoints == null || waypoints.Length == 0) return;

//        StartCoroutine(StateMachine());
//    }

//    private void FaceTarget(Vector3 targetPos)
//    {
//        Vector3 dir = targetPos - transform.position;
//        if (dir.sqrMagnitude < 0.00001) return;

//        dir.Normalize();
//        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * rotationSpeed);
//    }

//    private bool IsFacingPlayer (float angleThreshold)
//    {


//        if (!target) return true;
//        Vector3 toPlayer = (target.position - transform.position).normalized;
//        float angle = Vector3.Angle(transform.forward, toPlayer);
//        return angle < angleThreshold;
//    }

//    private IEnumerator RotateUntilFacingPlayer(float angleThreshold)
//    {
//        while(!IsFacingPlayer(angleThreshold))
//        {
//            FaceTarget(target.position);
//            yield return null;
//        }
//    }

//    private void PickRandomWayPoint()
//    {
//        if (waypoints != null && waypoints.Length > 0)
//        {
//            currenWaypointTarget = waypoints[Random.Range(0, waypoints.Length)];
//        }
//    }

//    private bool ReachedWaypoint()
//    {
//        if(!currenWaypointTarget) return false;
//        return Vector3.Distance(transform.position, currenWaypointTarget.position) < waypointDistanceThreshold;
//    }

//    private void MoveTowardsTarget(Vector3 targetPos)
//    {
//        Vector3 dir = targetPos - transform.position;
//        if (dir.sqrMagnitude < 0.00001f) return;

//        dir.Normalize();
//        transform.rotation = Quaternion.Slerp(transform.rotation,Quaternion.LookRotation(dir), Time.deltaTime * rotationSpeed);
//        transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
//    }

//    //private float DistanceToPlayer()
//    //{
//    //    //if (!target) return float.MaxValue;
//    //    //return Vector3.Distance(transform.position, target.position);
//    //}

//    private IEnumerator CircleState(float duration)
//    {
//        float timer = 0f;
//        PickRandomWayPoint();

//        while (timer < duration)
//        {
//            timer += Time.deltaTime;
//            if (currenWaypointTarget)
//            {
//                MoveTowardsTarget(currenWaypointTarget.position);
//            }

//            if (ReachedWaypoint())
//            {
//                PickRandomWayPoint();
//            }
//            yield return null;
//        }
//    }

//    private IEnumerator AttackState(float duration)
//    {
//        yield return StartCoroutine(RotateUntilFacingPlayer(angleToAttackPlayer));

//        float timer = 0f;
//        float shootTimer = 0f;

//        while (timer < duration)
//        {
//            timer += Time.deltaTime;
//            shootTimer += Time.deltaTime;

//            //FaceTarget(target.position);

//            if (DistanceToPlayer() > attackRange)
//            {
//                //MoveTowardsTarget(target.position);
//            }

//            if (shootTimer > shootInterval)
//            {
//                shootTimer = 0f;
//                Debug.Log("Should attack player");
//            }

//            yield return null;
//        }
//    }

//    private IEnumerator StateMachine()
//    {
//        while (true)
//        {
//            yield return StartCoroutine(CircleState(circleDuration));
//            yield return StartCoroutine(AttackState(attackDuration));
//        }
//    }
//}
