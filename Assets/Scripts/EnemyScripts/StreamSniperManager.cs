using UnityEngine;
using UnityEngine.AI;

public class StreamSniperManager : BaseEnemy
{
    protected GameObject player;
    public GameObject root;
    protected override void Awake()
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

    protected override void Start()
    {
        Debug.Log("Has blood:" + (blood != null));
        stunTimer = 0f;
        attackTimer = 0f;
        currentState = EnemyState.Chase;
        if (animator != null || animator.runtimeAnimatorController == null)
        {
            StartCoroutine(UpdateAnimations());
        }
        player = GameObject.FindGameObjectWithTag("Player");
    }
    protected override void Update()
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



        switch (currentState)
        {
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

            case EnemyState.Stunned:
                if (stunTimer >= stunCooldown)
                {
                    StartCoroutine(Stunned());
                    stunTimer = 0f;
                }
                break;
        }

    }

    protected override void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Stun"))
        {
            Debug.Log("Stun");
            currentState = EnemyState.Stunned;
        }
    }

    protected override void ChaseBehavior()
    {

        if (chaseTarget == null)
        {
            chaseTarget = player.transform;
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

    protected override void AttackBehavior()
    {

        if (animator != null)
            

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
            Attack();
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

    private void Attack()
    {

    }
}
