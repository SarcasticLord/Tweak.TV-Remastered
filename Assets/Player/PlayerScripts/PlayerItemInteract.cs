using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerItemInteract : MonoBehaviour
{
    public float maxDistance;
    public GameObject heldObject;
    private PickupAndCarry heldObjectScript;
    public GameObject playerCamera;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    

    void OnPickup(InputValue pickupValue)
    {
        
        Vector3 direction = playerCamera.transform.forward*maxDistance;
        Debug.DrawRay(transform.position, direction, Color.green);
        if(Physics.Raycast(transform.position, playerCamera.transform.forward, out RaycastHit hit, maxDistance)
        && hit.collider.gameObject.CompareTag("Object"))
        {
            heldObject = hit.collider.gameObject;
            heldObjectScript = heldObject.GetComponent<PickupAndCarry>();
            
            heldObjectScript.Pickup();
        }
        
    }

    void OnThrow(InputValue throwValue)
    {
        if (heldObject!=null)
        {
            heldObjectScript.Throw();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
