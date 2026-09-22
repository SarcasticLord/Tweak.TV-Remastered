using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.GraphicsBuffer;
public class BaseEnemy : MonoBehaviour
{
    enum EnemyState { Wander, Chase, Attack }
    EnemyState currentState;

    //Component variables
    private Rigidbody rb;
    private NavMeshAgent agent;
   

    protected bool canUseLinks = true;
    private bool isJumping = false;
    public float jumpTime = 3f;
    public float jumpHeight = 3f;

    //Wander variables
    public float wanderSpeed = 1f;
    public float wanderRadius = 1f;
    public float wanderInterval = 5f;
    public float wanderTimer;
    public float wanderTolerance = 1f;
    public float idleTime = 10f;

    //Chase variables
    public float chaseSpeed = 3f;


    //Attack variables
    public float attackRange = 2f;



    //Detection variables
    public float detectionAngle = 60f;
    public float detectionRange = 10f;
    public LayerMask obstructionMask;
    public LayerMask targetMask;

    private Transform chaseTarget;
    private bool targetVisible = false;

    private float lookTimer;
    public float lookInterval = 0.1f;

    public float targetMemoryTime = 3f;
    private float lastSeenTimer;



    private Coroutine pathCoroutine;
    private Vector3 currentDestination;


    public LayerMask groundMask;

    public Transform rayOrigin;

    //Optional
    public Animator animator;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        agent = GetComponent<NavMeshAgent>();
        if (agent != null)
        {
            agent.speed = wanderSpeed;
            agent.autoTraverseOffMeshLink = false;
        }
    }

    void Start()
    {
        currentState = EnemyState.Wander;
        ChooseNewWanderPoint();
    }

    void Update()
    {
        Debug.Log(agent.updatePosition);
        HandleLinks();
        
        if (isJumping)
            return;
        
        lookTimer += Time.deltaTime;
        lastSeenTimer += Time.deltaTime;

        if (lookTimer >= lookInterval)
        {
            lookTimer = 0f;
            Look();
            
        }

        switch (currentState)
        {
            case EnemyState.Wander:
                agent.speed = wanderSpeed;
                WanderBehavior();
                break;

            case EnemyState.Chase:
                agent.speed = chaseSpeed;
                ChaseBehavior();
                break;

            case EnemyState.Attack:
                AttackBehavior();
                break;
        }

        UpdateAnimations();
    }




    protected virtual void ChaseBehavior()
    {
        if (animator != null)
            animator.SetInteger("EnemyState", (int)EnemyState.Chase);

        if (chaseTarget == null)
        {
            currentState = EnemyState.Wander;
            return;
        }

        if (!targetVisible)
        {
            if (lastSeenTimer >= targetMemoryTime)
            {
                currentState = EnemyState.Wander;
                chaseTarget = null;
                ChooseNewWanderPoint();
            }

            return;
        }

        float distanceToTarget =
            Vector3.Distance(transform.position,
                             chaseTarget.position);

        if (distanceToTarget <= attackRange)
        {
            currentState = EnemyState.Attack;
            agent.ResetPath();
            return;
        }

        NavMeshHit hit;
 
        if (NavMesh.SamplePosition(
                chaseTarget.position,
                out hit,
                attackRange,
                NavMesh.AllAreas))
        {
            NavMeshPath path = new NavMeshPath();
 
            if (agent.CalculatePath(hit.position, path) &&
                path.status != NavMeshPathStatus.PathInvalid)
            {
                currentDestination = hit.position;
                agent.SetDestination(currentDestination);
            }
        }
    }

    protected virtual void WanderBehavior()
    {
        if (animator != null)
            animator.SetInteger("EnemyState", (int)EnemyState.Wander);

        if (targetVisible)
        {
            currentState = EnemyState.Chase;
            return;
        }

        if (agent.pathPending || agent.remainingDistance > wanderTolerance)
            return;

        wanderTimer += Time.deltaTime;

        if (ReachedDestination())
        {
            agent.ResetPath();
            Debug.Log("ResetPath Called");
            wanderTimer += Time.deltaTime;

            if (wanderTimer >= idleTime)
            {
                ChooseNewWanderPoint();
            }
        }
    }

    protected virtual void ChooseNewWanderPoint()
    {
        wanderTimer = 0f;

        Vector3 randomDirection = transform.position +  Random.insideUnitSphere * wanderRadius;

        if (NavMesh.SamplePosition(
            randomDirection,
            out NavMeshHit hit,
            wanderRadius,
            NavMesh.AllAreas))
        {
            NavMeshPath path = new NavMeshPath();
            // Verify the destination is reachable
            if (agent.CalculatePath(hit.position, path) &&
                path.status == NavMeshPathStatus.PathComplete)
            {
                currentDestination = hit.position;
                agent.SetDestination(currentDestination);
            }
        }
    }
    

    void AttackBehavior()
    {

    }

    void UpdateAnimations()
    {

    }

    protected virtual void HandleLinks()
    {
        if (!canUseLinks || isJumping)
            return;
        if (agent.isOnOffMeshLink)
        {
            StartCoroutine(JumpLink());
        }
        
    }

    protected virtual IEnumerator JumpLink()
    {
        Vector3 currentDestination = agent.destination;
        isJumping = true;

        OffMeshLinkData data = agent.currentOffMeshLinkData;

        Vector3 startPos = transform.position;
        Vector3 endPos = data.endPos;

        agent.updatePosition = false;
        agent.isStopped = true;

        float time = 0f;

        while (time < jumpTime)
        {
            float t = time / jumpTime;
            Vector3 pos = Vector3.Lerp(startPos, endPos, t);

            pos.y += Mathf.Sin(t * Mathf.PI) * jumpHeight;

            transform.position = pos;

            time += Time.deltaTime;

            yield return null;
        }

        transform.position = endPos;
        agent.CompleteOffMeshLink();
        agent.updatePosition = true;
        agent.isStopped = false;
        
        if (currentState == EnemyState.Wander)
        {
            MoveTo(currentDestination);
        }

        

        isJumping = false;
    }
 
    public void MoveTo(Vector3 destination)
    {
        agent.SetDestination(destination);
    }
    public virtual void SetTarget(Transform target)
        {
            chaseTarget = target;
 
            if (currentDestination != null)
            {
                agent.SetDestination(chaseTarget.position);
            }
        }

    bool ReachedDestination()
    {
        return !agent.pathPending &&
               agent.remainingDistance <= agent.stoppingDistance &&
               (!agent.hasPath || agent.velocity.sqrMagnitude < 0.01f);
    }

    public void Look()
    {
        targetVisible = false;

        Collider[] targets = Physics.OverlapSphere(transform.position, detectionRange, targetMask);

        foreach (Collider target in targets)
        {
            Vector3 directionToTarget =
                (target.transform.position - transform.position).normalized;
 
            float angle =
                Vector3.Angle(transform.forward, directionToTarget);
 
            if (angle > detectionAngle * 0.5f)
                continue;
 
            float distance =
                Vector3.Distance(transform.position,
                                 target.transform.position);
 
            // Check if something blocks vision
            if (!Physics.Raycast(
                rayOrigin.position,
                directionToTarget,
                distance,
                obstructionMask))
            {
                chaseTarget = target.transform;

                targetVisible = true;
                lastSeenTimer = 0f;
                return;
            }
        }

    }

    protected virtual bool TryGetChasePosition(
    out Vector3 chasePosition)
    {
        chasePosition = Vector3.zero;

        NavMeshHit hit;

        if (!NavMesh.SamplePosition(
                chaseTarget.position,
                out hit,
                10f,
                NavMesh.AllAreas))
            return false;

        NavMeshPath path = new NavMeshPath();

        if (!agent.CalculatePath(hit.position, path))
            return false;

        if (path.status == NavMeshPathStatus.PathInvalid)
            return false;

        chasePosition = hit.position;
        return true;
    }


    private void OnDrawGizmosSelected()
    {
        Vector3 origin = rayOrigin != null
            ? rayOrigin.position
            : transform.position;

        // Detection sphere
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(origin, detectionRange);

        // FOV boundaries
        Vector3 left =
            Quaternion.Euler(0, -detectionAngle * 0.5f, 0) *
            transform.forward;

        Vector3 right =
            Quaternion.Euler(0, detectionAngle * 0.5f, 0) *
            transform.forward;

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(origin, origin + left * detectionRange);
        Gizmos.DrawLine(origin, origin + right * detectionRange);

        // Chase target
        if (chaseTarget != null)
        {
            Gizmos.color = targetVisible
                ? Color.green
                : Color.red;

            Gizmos.DrawLine(origin, chaseTarget.position);
            Gizmos.DrawSphere(chaseTarget.position, 0.2f);
        }

        // Current destination
        if (agent != null && agent.hasPath)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(agent.destination, 0.3f);
            Gizmos.DrawLine(transform.position, agent.destination);
        }
    }
}