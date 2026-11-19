using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class TrainTicket : BaseItem
{
    public GameObject TicketUI;
    public bool hasTicket = false;

    protected override void FixedUpdate()
    {
        if (!inInventory)
        {
            //spins as a pickup when out of inventory
            transform.Rotate(new Vector3(0f, 30f, 0f) * rotSpeed * Time.deltaTime);
            
        }
        if (inInventory && transform.parent != null)
        {
            //Fixes position and rotation of item when in player inventory
            Transform parent = transform.parent.transform;
            rb.useGravity = false;
            col.enabled = false;
            hasTicket = true;
            gameObject.transform.SetLocalPositionAndRotation(itemHeldPos, itemHeldRot);
        }

    }
    protected override void OnUseItem(InputAction.CallbackContext use)
    {
        if (isSelected && use.ReadValueAsButton())
        {
            TicketUI.SetActive(!TicketUI.activeSelf);
        }
    }
}
