using System.Collections;
using UnityEngine;

public class BaseFlyingEnemy : BaseEnemy
{
    protected FlyingEnemyOffset flyOffset;
    protected void Start()
    {
        stunTimer = 0f;
        attackTimer = 0f;
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
                Death();
                break;

            case EnemyState.Stunned:
                if (stunTimer >= stunCooldown)
                {
                    StartCoroutine(FlyingStunned());
                    stunTimer = 0f;
                }
                break;
        }
    }

    private IEnumerator FlyingStunned()
    {
        agent.isStopped = true;
        rb.useGravity = true;
        rb.isKinematic = false;
        yield return new WaitForSeconds(stunDuration);
        Debug.Log("Escaping stun");
        agent.isStopped = false;
        currentState = EnemyState.Wander;
        rb.isKinematic = true;
        rb.useGravity = false;
        yield return null;

    }

}
