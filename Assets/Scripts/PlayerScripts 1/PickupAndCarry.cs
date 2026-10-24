using System;
using UnityEngine;

public class PickupAndCarry : MonoBehaviour
{
    [SerializeField]private Rigidbody rb;
    public Transform player;
    public Transform playerCamera;
    public PlayerItemInteract playerInteract;
    private Collider c;
    public float distanceFromCamera=1.1f;
    public float lerpTime=0.2f;
    public float lerpTimeStamp=0f;
    public float throwForce = 1000f;



    public bool isCarried=false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        c = GetComponent<Collider>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        playerCamera = GameObject.FindGameObjectWithTag("PlayerCamera").GetComponent<Transform>();
        playerInteract = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerItemInteract>();
    }

    public void StartPickup() //Sets up statistics for helobject being carried
    {
        SetObjectPhysics(false, false, false, false);
        isCarried=true;

    }
    [ContextMenu(nameof(Pickup))]
    public void Pickup() //Run continuously to carry object
    {
        
        //Vector3.Lerp(transform.position, player.position+(player.forward*distanceFromCamera), lerpTime);
        transform.position = player.position+(player.forward*distanceFromCamera);
        transform.LookAt(player);

    }
    [ContextMenu(nameof(Throw))]
    public void Throw() //Reactivate object functions and launch it
    {
        SetObjectPhysics(true, true, true, true);
        rb.AddForce(playerCamera.forward*throwForce);
        isCarried=false;
        playerInteract.heldObject = null;
    }
    void Update()
    {
        if (isCarried)
        {
            Pickup();
        }
    }
    void SetObjectPhysics(bool grav, bool lin, bool collide, bool rota) //Set object statistics when needed
    {
        rb.useGravity=grav;
        if(!lin){rb.linearVelocity=Vector3.zero;}
        c.isTrigger=!collide;
        // if(!rota){rb.freezeRotation=true;}else{rb.freezeRotation=false;}
    }
}
