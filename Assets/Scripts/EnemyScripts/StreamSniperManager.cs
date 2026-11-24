using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class StreamSniperManager : BaseEnemy
{
    protected GameObject player;
    public GameObject root;

    public float rotationSpeed = 100f;

    private bool isActive = false;


    protected override void Awake()
    {
        targetVisible = true;
        currentHealth = health;
        rb = GetComponent<Rigidbody>();
        agent = GetComponent<NavMeshAgent>();
        animator = gameObject.GetComponent<Animator>();
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
            StartCoroutine(OnSpawn());
        }
        player = GameObject.FindGameObjectWithTag("Player");
    }
    protected override void Update()
    {
        //if(rb.linearVelocity.magnitude > 15f) Death(); //Rat explodes when it reaches terminal velocity TODO: replace in the future probably
        if (isActive)
        {
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
    }

    public IEnumerator OnSpawn()
    {
        // Wait until state finishes
        while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.99f)
            yield return null;

        //Debug.Log("Animation finished!");
        isActive = true;
        animator.SetTrigger("OnSpawn");
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
        float distanceToTarget =
            Vector3.Distance(transform.position,
                             chaseTarget.position);

        Debug.Log(distanceToTarget);
        if (distanceToTarget > attackRange)
        {
            Debug.Log(distanceToTarget);
            currentState = EnemyState.Chase;
            ChaseTarget();
            return;
        }

        if (canAttack)
        {
            agent.velocity = Vector3.zero;
            Attack();
        }
        Vector3 direction =
            chaseTarget.position - transform.position;

        direction.y = 0f;

        Quaternion lookRotation =
            Quaternion.LookRotation(direction);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            lookRotation,
            rotationSpeed * Time.deltaTime);
    }

    protected override IEnumerator UpdateAnimations()
    {
        if (animator)
        {
            while (this.isActiveAndEnabled)
            {
                switch (currentState)
                {
                    case EnemyState.Attack:
                        animator.SetBool("isAttacking", true);
                        animator.SetBool("isChasing", false);
                        break;
                    case EnemyState.Chase:
                        animator.SetBool("isAttacking", false);
                        animator.SetBool("isChasing", true);
                        break;
                }
                yield return new WaitForSeconds(.15f);
            }
        }
    }
    private void Attack()
    {
        Debug.Log("Attacked");
        currentState = EnemyState.Attack;
        agent.velocity = Vector3.zero;
        canAttack = false;
    }
}
