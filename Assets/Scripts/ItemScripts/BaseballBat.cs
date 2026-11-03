using UnityEngine;
using UnityEngine.InputSystem;

public class BaseBallBat : BaseItem
{
    public GameObject hitbox;
    private Animator batAnim;

    private int randAnim;

    protected override void Start()
    {
        base.Start();
        batAnim = gameObject.GetComponent<Animator>();
    }
    protected override void OnUseItem(InputAction.CallbackContext use)
    {
        if (isSelected && use.ReadValueAsButton())
        {
            Debug.Log("Bat input read");
            randAnim = Random.Range(1, 4);
            batAnim.SetInteger("Attack", randAnim);
        }
    }
}
