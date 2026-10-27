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
    [SerializeField] private GameObject inventory;
    public InputActionReference useItem;
    public InputActionReference itemDrop;
    protected Quaternion origRot;

    [SerializeField] protected PlayerInventoryManager playerInventory;

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
        origRot = transform.rotation;
        //StartCoroutine(MoveObject());
        rb = gameObject.GetComponent<Rigidbody>();
        col = gameObject.GetComponent<CapsuleCollider>();
    }

    private void FixedUpdate()
    {
        if (!inInventory)
        {
            transform.Rotate(new Vector3(0f, 30f, 0f) * rotSpeed * Time.deltaTime);
        }
        if (inInventory && transform.parent != null)
        {
            Transform parent = transform.parent.transform;
            rb.useGravity = false;
            col.enabled = false;
            gameObject.transform.SetPositionAndRotation(parent.position, parent.rotation);

        }
        

    }

    private void OnUseItem(InputAction.CallbackContext use)
    {
        if (isSelected && use.ReadValueAsButton())
        {
            //insert use here
            Debug.Log(gameObject.name + " used");
        }
    }

    private void OnDropItem(InputAction.CallbackContext q)
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && !inInventory && playerInventory.items.Count < playerInventory.maxItems)
        {
            inInventory = true;
            gameObject.transform.SetParent(inventory.transform);
        }
    }

    //IEnumerator MoveObject()
    //{
    //    while (true)
    //    {
    //        transform.Translate(new Vector3(0, distance * movement) * Time.deltaTime);
    //        counter--;
    //        if(counter <= 0)
    //        {
    //            movement *= -1;
    //            counter = 5;
    //        }
    //        yield return new WaitForSeconds(1f);
    //    }
    //}

    public void StopMove()
    {
        //StopCoroutine(MoveObject());
        SphereCollider col = gameObject.GetComponent<SphereCollider>();
        col.enabled = false;
    }
}
