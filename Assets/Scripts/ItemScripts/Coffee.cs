using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Coffee : BaseItem
{
    public Vector3 drinkingPos;
    public PlayerStatusEffects statusEffects;
    public float duration = 15f;
    public int uses = 3;

    public bool isCaffeinated = false;


    protected override void Start()
    {
        base.Start();
        statusEffects = inventory.GetComponentInChildren<PlayerStatusEffects>();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        
        if(uses <= 0 && !isCaffeinated)
        {
            playerInventory.items.Remove(gameObject);
            playerInventory.CycleItems();
            inInventory = false;
            Destroy(this.gameObject);
        }
    }
    protected override void OnUseItem(InputAction.CallbackContext use)
    {
        if(uses > 0 && !isCaffeinated)
        {
            StartCoroutine(Caffeinated());
            uses--;
            isCaffeinated = true;
        }
    }
    protected virtual IEnumerator Caffeinated()
    {
        statusEffects.OnCanDash();
        Debug.Log("can dash");
        yield return new WaitForSeconds(duration);
        Debug.Log("no can dash");
        statusEffects.OnRemoveCanDash();
        isCaffeinated = false;
    }
}