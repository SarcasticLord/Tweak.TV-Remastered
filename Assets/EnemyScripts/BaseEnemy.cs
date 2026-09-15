using System.Collections;
using UnityEngine;
using UnityEngine.AI;
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
    public float wanderRadius = 1f;
    public float wanderInterval = 5f;
    public float wanderTimer;
    public float idleTime = 10f;

    //Chase variables
    public float detectionAngle = 60f;
    public float detectionRange = 10f;
    public Transform chaseTarget;
    private bool targetVisible = false;

    //Attack variables


    public float wanderSpeed = 10f;
    public float chaseSpeed;
    public float attackSpeed;
    public GameObject hitbox;
    public float offset = 2;
    public float attackCooldown = 3f;

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
    private void FixedUpdate()
    {
        RaycastHit hit;
        if (Physics.Raycast(rayOrigin.position, Vector3.down, out hit, 100f, groundMask))
        {
            transform.up = hit.normal; // align up to NavMesh normal
        }
    }
    void Update()
    {
        HandleLinks();
            if (isJumping)
            return;
        Look();

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

    public void Look()
    {
        Vector3 forward = transform.forward;
        Vector3 toPlayer = (chaseTarget.position - transform.position).normalized;
        float angle = Vector3.Angle(forward, toPlayer);
        float distance = Vector3.Distance(transform.position, chaseTarget.position);

        bool inCone = angle < detectionAngle;
        bool inRange = distance <= detectionRange;

        if (inCone && inRange)
        {
            targetVisible = true;
            Debug.Log("Player Detected!");
        } else
        {
            targetVisible = false;
        }
    }

    protected virtual void ChaseBehavior()
    {
        if (animator != null)
            animator.SetInteger("EnemyState", (int)EnemyState.Chase);

        if (!agent.hasPath && targetVisible)
        {
            agent.SetDestination(chaseTarget.position);
        }
    }

    protected virtual void WanderBehavior()
    {
        if (animator != null)
            animator.SetInteger("EnemyState", (int)EnemyState.Wander);

        if (isJumping)
            return;

        if (targetVisible)
        {
            currentState = EnemyState.Chase;
        }

        wanderTimer += Time.deltaTime;

        if (!agent.pathPending &&
            agent.remainingDistance <= agent.stoppingDistance &&
            wanderTimer >= idleTime)
        {
            ChooseNewWanderPoint();
        }
        //Debug.Log(agent.pathPending);
    }

    protected virtual void ChooseNewWanderPoint()
    {
        wanderTimer = 0f;

        for (int i = 0; i < 10; i++)
        {
            Vector3 randomDirection =
                Random.insideUnitSphere * wanderRadius;

            randomDirection += transform.position;

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
                    agent.SetDestination(hit.position);
                    return;
                }
            }
        }
    }
    

    protected virtual void CreateNewWanderPoint()
    {
        wanderTimer = 0f;

        Vector3 randomDirection = Random.insideUnitSphere * wanderRadius;

        randomDirection += transform.position;

        if(NavMesh.SamplePosition(
            randomDirection,
            out NavMeshHit hit,
            wanderRadius,
            1))
        {
            agent.SetDestination(hit.position);
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
}