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
    private Vector3 wanderTarget;
    public Animator animator;
    protected bool canUseLinks = true;
    private bool isJumping = false;
    private float jumpTime = 3f;
    private float jumpHeight = 3f;

    //General variables
    public float walkSpeed = 6;
    public float runSpeed = 9;
    
    //Wander variables
    public float wanderRadius = 1f;
    public float wanderInterval = 5f;
    public float wanderTimer;
    public float idleTime = 10f;
    
    //Chase variables

    //Attack variables


    public float wanderSpeed = 10f;
    public float chaseSpeed;
    public float attackSpeed;
    public GameObject hitbox;
    public float offset = 2;
    public float attackCooldown = 3f;

    public LayerMask playerMask;
    public LayerMask obstacleMask;

    public Transform[] waypoints;
    int m_CurrentWaypointIndex;

    Vector3 playerLastPosition = Vector3.zero;
    Vector3 m_PlayerPosition;

    float m_WaitTime;
    float m_TimeToRotate;
    bool m_PlayerInRange;
    bool m_PlayerNear;
    bool m_IsPatrol;
    bool m_CaughtPlayer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        agent = GetComponent<NavMeshAgent>();
        if (agent != null)
        {
            agent.speed = walkSpeed;
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
        HandleLinks();
        if (isJumping)
            return;

        FindTarget();

        switch (currentState)
        {
            case EnemyState.Wander:
                WanderBehavior();
                break;

            case EnemyState.Chase:
                ChaseBehavior();
                break;

            case EnemyState.Attack:
                AttackBehavior();
                break;
        }
        UpdateAnimations();
    }
    protected virtual void WanderBehavior()
    {
        if (isJumping)
            return;

        wanderTimer += Time.deltaTime;

        if (!agent.pathPending &&
            agent.remainingDistance <= agent.stoppingDistance &&
            wanderTimer >= idleTime)
        {
            ChooseNewWanderPoint();
        }
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

    void CaughtPlayer()
    {
        m_CaughtPlayer = true;
    }
    void Move(float speed)
    {
        agent.isStopped = false;
        agent.speed = speed;
    }

    

    void FindTarget()
    {
        return;
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

    void ChaseBehavior()
    {

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
        agent.Warp(endPos);

        agent.updatePosition = true;
        agent.isStopped = false;

        isJumping = false;
    }

}