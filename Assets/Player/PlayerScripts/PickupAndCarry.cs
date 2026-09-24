using System;
using UnityEngine;

public class PickupAndCarry : MonoBehaviour
{
    [SerializeField]private Rigidbody rb;
    public Transform player;
    public Transform playerCamera;
    private BoxCollider bc;
    public float distanceFromCamera=1.1f;
    public float lerpTime=0.2f;
    public float lerpTimeStamp=0f;


    private bool isCarried=false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        bc = GetComponent<BoxCollider>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        playerCamera = GameObject.FindGameObjectWithTag("PlayerCamera").GetComponent<Transform>();
    }

    public void StartPickup()
    {
        SetObjectPhysics(false, false, false, false);
        isCarried=true;
    }
    [ContextMenu(nameof(Pickup))]
    public void Pickup()
    {
        
//Vector3.Lerp(transform.position, player.position+(player.forward*distanceFromCamera), lerpTime);
        transform.position = player.position+(player.forward*distanceFromCamera);
        transform.LookAt(player);

        
    }
    [ContextMenu(nameof(Throw))]
    public void Throw()
    {
        SetObjectPhysics(true, true, true, true);
        rb.AddForce(playerCamera.forward*999f);
        isCarried=false;
    }
    void Update()
    {
        if (isCarried)
        {
            Pickup();
        }
    }
    void SetObjectPhysics(bool grav, bool lin, bool collide, bool rota)
    {
        rb.useGravity=grav;
        if(!lin){rb.linearVelocity=Vector3.zero;}
        bc.isTrigger=!collide;
        // if(!rota){rb.freezeRotation=true;}else{rb.freezeRotation=false;}
    }
}
