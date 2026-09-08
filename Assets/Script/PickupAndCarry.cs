using UnityEngine;

public class PickupAndCarry : MonoBehaviour
{
    private Rigidbody rb;
    public Transform player;
    private BoxCollider bc;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        bc = GetComponent<BoxCollider>();
    }

    [ContextMenu(nameof(Pickup))]
    void Pickup()
    {
        SetObjectPhysics(false, false, false, false);
        transform.position = player.position+new Vector3(0f,0.14f,0.80f);
    }
    [ContextMenu(nameof(Throw))]
    void Throw()
    {
        SetObjectPhysics(true, true, true, true);
        rb.AddForce(0, 0, 999f);
    }

    void SetObjectPhysics(bool grav, bool lin, bool collide, bool rota)
    {
        rb.useGravity=grav;
        if(!lin){rb.linearVelocity=Vector3.zero;}
        bc.isTrigger=!collide;
        if(!rota){rb.freezeRotation=true;}else{rb.freezeRotation=false;}
    }
}
