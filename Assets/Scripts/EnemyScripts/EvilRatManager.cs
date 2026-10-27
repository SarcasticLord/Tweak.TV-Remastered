using System.Collections;
using UnityEngine;

public class EvilRatManager : BaseEnemy
{
    public GameObject hitbox;
    public Transform attackPoint;

    protected override void AttackBehavior()
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
            Instantiate(hitbox, attackPoint);
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

}
