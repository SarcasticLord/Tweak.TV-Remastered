using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class BaseBallBat : BaseItem
{
    public Collider hitbox;
    private Animator batAnim;

    private int randAnim;

    public float attackCooldown = 1f;

    private float attackTimer;


    protected override void Start()
    {
        base.Start();
        hitbox.enabled =  false;
        batAnim = gameObject.GetComponent<Animator>();
    }

    private void Update()
    {
        if(attackCooldown > 0)
        {
            attackTimer -= Time.deltaTime;
        }
    }

    protected override void OnUseItem(InputAction.CallbackContext use)
    {
        if (isSelected && use.ReadValueAsButton() && attackTimer <= 0)
        {
            StartCoroutine(Attack());
            attackTimer = attackCooldown;
        }
    }

    protected IEnumerator Attack()
    {
        batAnim.SetInteger("Attack", 2);
        hitbox.enabled = true;
        yield return new WaitForSeconds(attackCooldown);
        batAnim.SetInteger("Attack", 0);
        hitbox.enabled = false;
    }
}
