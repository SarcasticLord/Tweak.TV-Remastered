using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Coffee : BaseItem
{
    public Vector3 drinkingPos;

    private bool isDrinking = false;


    protected override void Start()
    {
        base.Start();
    }
    protected override void OnUseItem(InputAction.CallbackContext use)
    {

    }

    protected virtual void FixedUpdate()
    {
        if (!inInventory)
        {
            //spins as a pickup when out of inventory
            transform.Rotate(new Vector3(0f, 30f, 0f) * rotSpeed * Time.deltaTime);
        }
        if (inInventory && transform.parent != null && !isDrinking)
        {
            //Fixes position and rotation of item when in player inventory
            Transform parent = transform.parent.transform;
            rb.useGravity = false;
            col.enabled = false;
            gameObject.transform.SetLocalPositionAndRotation(itemHeldPos, itemHeldRot);
        }

        if (inInventory && transform.parent != null && isDrinking)
        {
            //Fixes position and rotation of item when in player inventory
            Transform parent = transform.parent.transform;
            rb.useGravity = false;
            col.enabled = false;
            if(transform.localPosition != transform.localPosition + drinkingPos)
            {
                Vector3.Lerp(transform.localPosition, transform.localPosition + drinkingPos, 5f);
            }
        }



    }
}