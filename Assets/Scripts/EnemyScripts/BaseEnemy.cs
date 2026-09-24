using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.GraphicsBuffer;
public class BaseEnemy : MonoBehaviour
{
    enum EnemyState { Wander, Chase, Attack, Death}
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
    public float cooldown = 3f;
    private float attackTimer;
    private bool canAttack;

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

    private Vector3 lastTargetPos;



    private Coroutine pathCoroutine;
    private Vector3 currentDestination;


    public LayerMask groundMask;

    public Transform rayOrigin;

    //Optional
    private Animator animator;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        if (agent != null)
        {
            agent.speed = wanderSpeed;
            agent.autoTraverseOffMeshLink = false;
            agent.stoppingDistance = attackRange;
            agent.autoBraking = false;
            agent.updateRotation = false;
        }
    }

    void Start()
    {
        currentState = EnemyState.Wander;
        ChooseNewWanderPoint();
        if (animator != null || animator.runtimeAnimatorController == null)
        {
            StartCoroutine(UpdateAnimations());
        }
    }

    void Update()
    {
        RotateTowardsMovement();
        
        HandleLinks();
        
        if (isJumping)
            return;
        
        lookTimer += Time.deltaTime;
        lastSeenTimer += Time.deltaTime;
        attackTimer += Time.deltaTime;

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
                if (attackTimer >= cooldown)
                {
                    canAttack = true;
                    attackTimer = 0f;
                }
                break;
            case EnemyState.Death:
                Death();
                break;
        }

    }

    //Meant to test death, may be removed/altered later

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Hitbox"))
        {
            Debug.Log("Hitbox collided");
            currentState = EnemyState.Death;
        }
    }

    protected virtual void ChaseBehavior()
    {

        if (chaseTarget == null)
        {
            currentState = EnemyState.Wander;
            return;
        }

        if (!targetVisible)
        {
            agent.SetDestination(lastTargetPos);
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
        ChaseTarget();
    }

    private void ChaseTarget()
    {
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
            wanderTimer += Time.deltaTime;

            if (wanderTimer >= idleTime)
            {
                ChooseNewWanderPoint();
            }
        }
    }

    protected virtual void AttackBehavior()
    {

        if (animator != null)
            animator.SetInteger("EnemyState", (int)EnemyState.Attack);

        if (!targetVisible)
        {
            if (lastSeenTimer <= targetMemoryTime)
            {
                currentState = EnemyState.Chase;
                ChaseTarget();
            }
            return;
        }

        float distanceToTarget =
            Vector3.Distance(transform.position,
                             chaseTarget.position);
 
        if (distanceToTarget > attackRange)
        {
            currentState = EnemyState.Chase;
            return;
        }

        if (canAttack)
        {
            agent.velocity = Vector3.zero;
            
            //Replace with Attack Function            
            //Debug.Log("attack function");
            
            canAttack = false;
        }
        Vector3 direction =
            chaseTarget.position - transform.position;
 
        direction.y = 0f;
 
        Quaternion lookRotation =
            Quaternion.LookRotation(direction);
 
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            lookRotation,
            10f * Time.deltaTime);
    }


    private void Death()
    {
        agent.isStopped = true;
        Destroy(gameObject, 5);
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
    private void RotateTowardsMovement()
    {
        if (agent.velocity.sqrMagnitude < 0.01f)
            return;

        Quaternion targetRotation =
            Quaternion.LookRotation(agent.velocity.normalized);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            8f * Time.deltaTime);
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
                lastTargetPos = target.transform.position;
                return;
            }
        }
    }

    protected virtual void ChooseNewWanderPoint()
    {
        wanderTimer = 0f;

        Vector3 randomDirection = transform.position + Random.insideUnitSphere * wanderRadius;

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
    private IEnumerator UpdateAnimations()
    {
        while(this.isActiveAndEnabled)
        {
            Vector3 currentVel = agent.velocity;

            if (currentVel.sqrMagnitude > 0f)
            {
                animator.SetInteger("EnemyState", 1);
            }
            else
            {
                animator.SetInteger("EnemyState", 0);
            }

            if (currentState == EnemyState.Attack)
            {
                animator.SetInteger("EnemyState", 2);
            }
            if (currentState == EnemyState.Death)
            {
                animator.SetInteger("EnemyState", 3);
            }

            yield return new WaitForSeconds(.5f);
        }
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