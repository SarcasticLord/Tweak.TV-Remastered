using System;
using System.Collections;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Coffee : BaseItem
{
    // public Vector3 drinkingPos;
    // private PlayerStatusEffects statusEffects;
    // public float duration = 15f;
    public int uses = 3;
    // public bool isCaffeinated = false; TODO: move to status effect tracker
    // public CinemachineCamera cam;
    // public float storedFOV;
    // public float caffeinatedFOV = 90f;
    private PlayerStatusEffectTracker tracker; 

    protected override void Start()
    {
        base.Start();
        tracker = inventory.GetComponentInChildren<PlayerStatusEffectTracker>();
        // cam = inventory.GetComponentInChildren<CinemachineCamera>();
        // storedFOV = cam.Lens.FieldOfView;
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        
        if(uses <= 0)
        {
            playerInventory.items.Remove(gameObject);
            playerInventory.CycleItems();
            inInventory = false;
            Destroy(gameObject);
        }
    }
    protected override void OnUseItem(InputAction.CallbackContext use)
    {
        if(uses > 0 && !tracker.caffeinated && isSelected)
        {
            // StartCoroutine(Caffeinated());
            tracker.AddToStatus(PlayerStatusEffectTracker.StatusEffect.CAFFEINATED, 0);
            uses--;
            
        }
    }
    // protected virtual IEnumerator Caffeinated()
    // {
    //     // statusEffects.OnCanDash(); //Speed player up
    //     // cam.Lens.FieldOfView = caffeinatedFOV; 
    //     Debug.Log("can dash");
    //     yield return new WaitForSeconds(duration);
    //     Debug.Log("no can dash");
    //     cam.Lens.FieldOfView = storedFOV;
    //     statusEffects.OnRemoveCanDash(); //Slow player down
    //     isCaffeinated = false;
    // }
}