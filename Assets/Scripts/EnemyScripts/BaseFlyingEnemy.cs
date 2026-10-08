using System.Collections;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class BaseFlyingEnemy : BaseEnemy
{

    [SerializeField] protected float hoverOffset;
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
        agent.baseOffset = 0f;
        rb.useGravity = true;
        rb.isKinematic = false;
        rb.linearVelocity = -agent.velocity * 3.5f;
        agent.enabled = false;
        yield return new WaitForSeconds(stunDuration);
        Debug.Log("Escaping stun");
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        agent.Warp(transform.position);
        agent.enabled = true;
        agent.baseOffset = hoverOffset;
        rb.isKinematic = true;
        rb.useGravity = false;
        currentState = EnemyState.Wander;
        yield return null;

    }

}
