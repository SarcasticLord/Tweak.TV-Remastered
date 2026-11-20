using System;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class PickupAndThrow : MonoBehaviour
{
    public GameObject pickedUpItem;
    private Rigidbody itemRb;
    private Collider col;
    public Transform cam;
    public PlayerItemInteract playerInteract;
    public float distanceFromCamera=1.1f;
    public float lerpTime=0.2f;
    public float lerpTimeStamp=0f;
    public float throwForce = 1000f;
    public LayerMask ignoreLayer;
    public float maxDistance;
    public bool pickupInHand;
    public InputActionReference pickupItem;


    private void OnEnable()
    {
        pickupItem.action.performed += OnPickupItem;
        pickupItem.action.canceled += OnPickupItem;

    }

    private void OnDisable()
    {
        pickupItem.action.performed -= OnPickupItem;
        pickupItem.action.canceled -= OnPickupItem;

    }

    void Start()
    {
        pickupInHand = false;
        //itemRb = pickedUpItem.GetComponent<Rigidbody>();
        cam = gameObject.GetComponentInChildren<Camera>().gameObject.transform;
        playerInteract = gameObject.GetComponent<PlayerItemInteract>();

    }

    public void Pickup() //Run continuously to carry object
    {
        pickupInHand = true;
        //Vector3.Lerp(transform.position, player.position+(player.forward*distanceFromCamera), lerpTime);
        itemRb = pickedUpItem.GetComponent<Rigidbody>();
        itemRb.constraints = RigidbodyConstraints.FreezeAll;
        itemRb.useGravity = false;
        pickedUpItem.transform.position = cam.position+(cam.forward*distanceFromCamera);
        pickedUpItem.transform.LookAt(cam.transform);
        pickedUpItem.transform.parent = cam.transform;


    }
    [ContextMenu(nameof(Throw))]
    public void Throw() //Reactivate object functions and launch it
    {
        pickupInHand = false;
        itemRb.constraints = RigidbodyConstraints.None;
        itemRb.useGravity = true;
        itemRb.AddForce(cam.forward*throwForce);
        pickedUpItem.transform.parent = null;
        itemRb = null;

    }
    void OnPickupItem(InputAction.CallbackContext pickup)
    {
        if (pickup.ReadValueAsButton())
        {
            if (!pickupInHand)
            {
                Vector3 rayOrigin = cam.position;
                Vector3 rayDirection = cam.forward;

                Debug.DrawRay(rayOrigin, rayDirection * 10f, Color.red, 5f);
                RaycastHit hit;
                if (Physics.Raycast(rayOrigin, rayDirection * 10f, out hit, maxDistance, ~ignoreLayer))
                {
                    GameObject target = hit.transform.gameObject;
                    Collider targetCollider = hit.collider;
                    Rigidbody targetRb = targetCollider.GetComponent<Rigidbody>();
                    if (targetRb != null)
                    {
                            pickedUpItem = target;
                            Pickup();
                    }
                }
            } else if (pickupInHand)
            {
                Throw();
            }
        }
       
    }
}
