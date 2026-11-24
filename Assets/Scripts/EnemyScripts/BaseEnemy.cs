using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.GraphicsBuffer;
public class BaseEnemy : MonoBehaviour
{
    public enum EnemyState { Wander, Chase, Attack, Death, Stunned}
    public EnemyState currentState;

    public enum EnemyType { RAT, BEE, STREAMSNIPER }

    public EnemyType enemyType;

    //Component variables
    protected Rigidbody rb;
    protected NavMeshAgent agent;
    [SerializeField] protected GameObject blood;
    [SerializeField] protected Material ghostMat;
    protected bool isDead;



    //Health Variables, use in other scripts
    [SerializeField] protected int health = 3;
    protected int currentHealth;
   
    // Jumping between links variables
    protected bool canUseLinks = true;
    protected bool isJumping = false;
    [SerializeField] protected float jumpTime = 3f;
    [SerializeField] protected float jumpHeight = 3f;

    //Wander variables
    [SerializeField] protected float wanderSpeed = 1f;
    [SerializeField] protected float wanderRadius = 1f;
    [SerializeField] protected float wanderInterval = 5f;
    [SerializeField] protected float wanderTimer;
    [SerializeField] protected float wanderTolerance = 1f;
    [SerializeField] protected float idleTime = 10f;

    //Chase variables
    [SerializeField] protected float chaseSpeed = 3f;


    //Attack variables
    [SerializeField] protected float attackRange = 2f;
    [SerializeField] protected float cooldown = 3f;
    protected float attackTimer;
    protected bool canAttack;


    //Stun variables
    [SerializeField] protected GameObject stunObj;
    [SerializeField] protected float stunTimer;
    [SerializeField] protected float stunDuration = 999f; //Original Val = 5f
    [SerializeField] protected float stunCooldown = 1f;


    //Detection variables
    [SerializeField] protected float detectionAngle = 60f;
    [SerializeField] protected float detectionRange = 10f;
    [SerializeField] protected LayerMask obstructionMask;
    [SerializeField] protected LayerMask targetMask;

    protected Transform chaseTarget;
    protected bool targetVisible = false;
    protected float lookTimer;
    [SerializeField] protected float lookInterval = 0.1f;

    [SerializeField] protected float targetMemoryTime = 3f;
    protected float lastSeenTimer;

    protected Vector3 lastTargetPos;



    protected Coroutine pathCoroutine;
    private Vector3 currentDestination;


    [SerializeField] protected LayerMask groundMask;

    [SerializeField] protected Transform rayOrigin;

    //Optional
    protected Animator animator;

    protected virtual void Awake()
    {
        currentHealth = health;
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

    protected virtual void Start()
    {
        Debug.Log("Has blood:" + (blood != null));
        stunTimer = 0f;
        attackTimer = 0f;
        currentState = EnemyState.Wander;
        if (!ChooseNewWanderPoint())
            wanderTimer = idleTime;
        if (animator != null || animator.runtimeAnimatorController == null)
        {
            StartCoroutine(UpdateAnimations());
        }
    }

    protected virtual void Update()
    {
        //if(rb.linearVelocity.magnitude > 15f) Death(); //Rat explodes when it reaches terminal velocity TODO: replace in the future probably

        RotateTowardsMovement();
        
        HandleLinks();
        
        if (isJumping)
            return;
        
        lookTimer += Time.deltaTime;
        lastSeenTimer += Time.deltaTime;
        attackTimer += Time.deltaTime;
        stunTimer += Time.deltaTime;

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
                if (!isDead)
                {
                    Death();
                    isDead = true;
                }
                break;

            case EnemyState.Stunned:
                if (stunTimer >= stunCooldown)
                {
                    StartCoroutine(Stunned());
                    stunTimer = 0f;
                }
                break;
        }

    }

    //Meant to test death, may be removed/altered later

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("PlayerHitbox"))
        {
            Debug.Log("Death");
            currentState = EnemyState.Death;
        }
        else if (other.gameObject.CompareTag("Stun"))
        {
            Debug.Log("Stun");
            currentState = EnemyState.Stunned;
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

    protected virtual void ChaseTarget()
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

        if (agent.pathPending || !ReachedDestination())
            return;

        wanderTimer += Time.deltaTime;

        if (wanderTimer >= idleTime)
        {
            ChooseNewWanderPoint();
        }

        //if (ReachedDestination())
        //{
        //    agent.ResetPath();
        //    wanderTimer += Time.deltaTime;

        //    if (wanderTimer >= idleTime)
        //    {
        //        ChooseNewWanderPoint();
        //    }
        //}
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


    protected virtual void Death()
    {
        
        GameController.instance.decreaseCurrEnemyCount(enemyType);
        blood.SetActive(true);
        agent.enabled = false;
        if (animator != null)
            animator.SetInteger("EnemyState", (int)EnemyState.Death);
        if (ghostMat != null)
        {
            SkinnedMeshRenderer renderer = gameObject.GetComponentInChildren<SkinnedMeshRenderer>();
            Material[] mats = renderer.materials;
            for (int i = 0; i < mats.Length; i++)
            {
                mats[i] = ghostMat;
                renderer.materials = mats;
            }
        }
        Collider[] cols = gameObject.GetComponents<Collider>();
        foreach (Collider col in cols)
        {
            col.enabled = false;
        }
        Destroy(gameObject, 5);
    }

    protected virtual IEnumerator Stunned()
    {

        agent.isStopped = true;
        yield return new WaitForSeconds(stunDuration);
        Debug.Log("Escaping stun");
        agent.isStopped = false;
        currentState = EnemyState.Wander;
        yield return null;

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
    protected virtual void RotateTowardsMovement()
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
 
    protected void MoveTo(Vector3 destination)
    {
        agent.SetDestination(destination);
    }
    protected virtual void SetTarget(Transform target)
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

    protected void Look()
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

    protected virtual bool ChooseNewWanderPoint()
    {
        //wanderTimer = 0f;

        //Vector3 randomDirection = transform.position + Random.insideUnitSphere * wanderRadius;

        //if (NavMesh.SamplePosition(
        //    randomDirection,
        //    out NavMeshHit hit,
        //    wanderRadius,
        //    NavMesh.AllAreas))
        //{
        //    NavMeshPath path = new NavMeshPath();
        //    // Verify the destination is reachable
        //    if (agent.CalculatePath(hit.position, path) &&
        //        path.status == NavMeshPathStatus.PathComplete)
        //    {
        //        currentDestination = hit.position;
        //        agent.SetDestination(currentDestination);
        //    }
        //}

        Vector2 circle = Random.insideUnitCircle * wanderRadius;
        Vector3 candidate = agent.nextPosition + new Vector3(circle.x, 0f, circle.y);

        if (NavMesh.SamplePosition(candidate, out NavMeshHit hit, wanderRadius, NavMesh.AllAreas))
        {
            NavMeshPath path = new NavMeshPath();
            if (agent.CalculatePath(hit.position, path) &&
                path.status == NavMeshPathStatus.PathComplete)
            {
                wanderTimer = 0f;          // only reset on success
                currentDestination = hit.position;
                agent.SetDestination(hit.position);
                return true;
            }
        }
        return false;
    }
    protected virtual IEnumerator UpdateAnimations()
    {
        while(this.isActiveAndEnabled)
        {
            Vector3 currentVel = agent.velocity;


            //Handles idle and movement animations
            if (currentVel.sqrMagnitude > 0f)
            {
                //Idle
                animator.SetInteger("EnemyState", 1);
            }
            else
            {
                //Moving
                animator.SetInteger("EnemyState", 0);
            }

            //Handles attack, stun, and death animations 
            switch (currentState)
            {
                case EnemyState.Attack:
                    animator.SetInteger("EnemyState", 2);
                    break;
                case EnemyState.Stunned:
                    //change if enemy has dedicated stun animation, for base enemy is same as idle
                    animator.SetInteger("EnemyState", 0);
                    break;
                case EnemyState.Death:
                    animator.SetInteger("EnemyState", 3);
                    break;
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