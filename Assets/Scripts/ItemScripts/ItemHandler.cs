using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;

public class ItemHandler : MonoBehaviour
{
    protected Rigidbody rb;
    protected CapsuleCollider col;
    public float distance = .3f;
    public float movement = .25f;
    public float rotSpeed = 5f;
    //private int counter = 5;
    [SerializeField] private GameObject inventory; //items in PlayerCapsule
    public InputActionReference useItem;
    public InputActionReference itemDrop;
    protected Quaternion origRot;

    protected PlayerInventoryManager playerInventory; //gathered from inventory

    public bool isSelected = false;

    private bool inInventory = false;

    private void OnEnable()
    {
        itemDrop.action.performed += OnDropItem;
        itemDrop.action.canceled += OnDropItem;
        useItem.action.performed += OnUseItem;
        useItem.action.canceled += OnUseItem;

    }

    private void OnDisable()
    {
        itemDrop.action.performed -= OnDropItem;
        itemDrop.action.canceled -= OnDropItem;
        useItem.action.performed -= OnUseItem;
        useItem.action.canceled -= OnUseItem;
    }

    private void Start()
    {
        playerInventory = inventory.GetComponent<PlayerInventoryManager>(); //Get manager from inventory gameobject
        origRot = transform.rotation;
        rb = gameObject.GetComponent<Rigidbody>();
        col = gameObject.GetComponent<CapsuleCollider>();
    }

    private void FixedUpdate()
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
            gameObject.transform.SetPositionAndRotation(parent.position, parent.rotation);

        }
        

    }

    protected virtual void OnUseItem(InputAction.CallbackContext use)
    {
        if (isSelected && use.ReadValueAsButton())
        {
            //insert uses here, override needed
            Debug.Log(gameObject.name + " used");
        }
    }

    protected virtual void OnDropItem(InputAction.CallbackContext q)
    { 
        if (q.ReadValueAsButton())
        {
            if (inInventory && isSelected)
            {
                playerInventory.items.Remove(gameObject);
                playerInventory.CycleItems();
                col.enabled = true;
                rb.useGravity = true;
                rb.AddForce(transform.forward * 50f);
                gameObject.transform.SetParent(null);
                transform.rotation = origRot;
                inInventory = false;
            }
        }
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        //Checks for player collision
        if (other.gameObject.CompareTag("Player") && !inInventory && playerInventory.items.Count < playerInventory.maxItems)
        {
            inInventory = true;
            //makes this item a child of the player's inventory attached to the camera
            gameObject.transform.SetParent(inventory.transform);
        }
    }

    public void StopMove()
    {
        //StopCoroutine(MoveObject());
        SphereCollider col = gameObject.GetComponent<SphereCollider>();
        col.enabled = false;
    }
}
