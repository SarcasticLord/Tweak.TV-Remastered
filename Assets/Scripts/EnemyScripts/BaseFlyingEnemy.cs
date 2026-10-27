using System.Collections;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class BaseFlyingEnemy : BaseEnemy
{

    [SerializeField] protected float hoverOffset = 10f;
    [SerializeField] protected float projSpeed = 10f;
    [SerializeField] protected GameObject beeProj;
    [SerializeField] protected Transform attackSpawn;

    protected override void Start()
    {
        base.Start();
        agent.baseOffset = hoverOffset;
    }

    protected override IEnumerator Stunned()
    {
        Vector3 launchVelocity = agent.velocity;
        agent.baseOffset = 0f;
        agent.enabled = false;
        rb.useGravity = true;
        rb.isKinematic = true;
        rb.linearVelocity = launchVelocity;
        yield return new WaitForSeconds(stunDuration);
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.isKinematic = false;
        rb.useGravity = false;
        agent.enabled = true;
        agent.baseOffset = hoverOffset;
        currentState = EnemyState.Chase;
        yield return null;
    }


    protected override void AttackBehavior()
    {
        Vector3 direction =
            (chaseTarget.position - transform.position).normalized;
        direction.y = 0f;

        Quaternion lookRotation =
            Quaternion.LookRotation(direction);

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


        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            lookRotation,
            10f * Time.deltaTime);


        if (canAttack)
        {
            StartCoroutine(BeeAttack());
            canAttack = false;
        }
    }


    protected virtual void ShootAttack(Vector3 dir)
    {
        GameObject bullet = Instantiate(beeProj,attackSpawn.position, attackSpawn.rotation);
        bullet.GetComponent<Rigidbody>().AddForce(dir * projSpeed, ForceMode.VelocityChange);
    }

    IEnumerator BeeAttack()
    {
        Vector3 direction =
            (chaseTarget.position - transform.position).normalized;
        agent.velocity = Vector3.zero;
        agent.isStopped = true;
        yield return new WaitForSeconds(1f);
        ShootAttack(direction);
        agent.isStopped = false;
    }

    protected override void Death()
    {
        base.Death();
        agent.baseOffset = 0f;
        rb.useGravity = true;
        rb.isKinematic = false;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

}
