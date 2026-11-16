using System.Collections;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Coffee : BaseItem
{
    public Vector3 drinkingPos;
    private PlayerStatusEffects statusEffects;
    public float duration = 15f;
    public int uses = 3;
    public bool isCaffeinated = false;
    public CinemachineCamera cam;
    public float storedFOV;
    public float caffeinatedFOV;


    protected override void Start()
    {
        base.Start();
        statusEffects = inventory.GetComponentInChildren<PlayerStatusEffects>();
        cam = inventory.GetComponentInChildren<CinemachineCamera>();
        storedFOV = cam.Lens.FieldOfView;
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
        if(uses > 0 && !isCaffeinated && isSelected)
        {
            StartCoroutine(Caffeinated());
            uses--;
            isCaffeinated = true;
        }
    }
    protected virtual IEnumerator Caffeinated()
    {
        statusEffects.OnCanDash();
        cam.Lens.FieldOfView = caffeinatedFOV; 
        Debug.Log("can dash");
        yield return new WaitForSeconds(duration);
        Debug.Log("no can dash");
        cam.Lens.FieldOfView = storedFOV;
        statusEffects.OnRemoveCanDash();
        isCaffeinated = false;
    }
}