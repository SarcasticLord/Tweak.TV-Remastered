using System.Collections;
using UnityEngine;

public class EvilRatManager : BaseEnemy
{
    private PickupAndCarry pickup;
    //public bool canPickup;
    private bool isPickedUp;
    public GameObject hitbox;
    public Transform attackPoint;
    public LayerMask ground;
    public bool grounded;
    private void CheckGround()
    {
        // Raycast from the player's position downward
        if (Physics.Raycast(transform.position, Vector3.down, 5f, ground))
        {
            grounded = true;
        }
        else
        {
            grounded = false;
        }
     
    }
    protected override void Start()
    {
        pickup = gameObject.GetComponent<PickupAndCarry>();
        base.Start();
        isPickedUp = false;
        pickup.enabled = true;
        rb.isKinematic = false;
    }

    private void FixedUpdate()
    {
        PlayerItemInteract interact = pickup.playerInteract;
        Collider[] cols = gameObject.GetComponents<Collider>();
        if (pickup.enabled && interact.heldObject == this.gameObject)
        {
            foreach (Collider col in cols)
            {
                col.enabled = false;
            }
            isPickedUp = true;
            currentState = EnemyState.Stunned;
        }
        else
        {
            foreach (Collider col in cols)
            {
                col.enabled = enabled;
            }
            if(isPickedUp == true)
            {
                isPickedUp = false;
                currentState = EnemyState.Death;
            }
            
        }
        CheckGround();
        if (grounded)
        {
            rb.isKinematic = false;
        }
    }
    protected override IEnumerator Stunned()
    {
        PlayerItemInteract interact = pickup.playerInteract;
        rb.isKinematic = false;

        agent.velocity = Vector3.zero;
        agent.enabled = false;
        rb.useGravity = true;
        pickup.enabled = true;

        yield return new WaitForSeconds(stunDuration);
        //After stun
        if (interact.heldObject == this.gameObject)
        {
            pickup.Throw();
        }
        pickup.enabled = false;
        agent.enabled = true;
        //currentState = EnemyState.Chase;
        yield return null;

    }

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
