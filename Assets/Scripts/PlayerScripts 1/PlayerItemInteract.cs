using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerItemInteract : MonoBehaviour
{
    public float maxDistance;
    public GameObject heldObject;
    private PickupAndCarry heldObjectScript;
    public GameObject playerCamera;
    public float pickupCooldown = 1f;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    
    //private IEnumerator PickupDelay()
    //{
    //    canPickup = false;
    //    yield return new WaitForSeconds(.5f);
    //    canPickup = true;
    //}
    private void Update()
    {
        
        if(pickupCooldown > 0)
            pickupCooldown -= Time.deltaTime;

    }


    void OnPickup(InputValue pickupValue)
    {
        if(heldObject == null && pickupCooldown <= 0)
        {
            Vector3 direction = playerCamera.transform.forward * maxDistance;
            Debug.DrawRay(transform.position, direction, Color.green);
            if (Physics.Raycast(transform.position, playerCamera.transform.forward, out RaycastHit hit, maxDistance)
            && (hit.collider.gameObject.CompareTag("Object") || hit.collider.gameObject.CompareTag("KeyItem")))
            {
                heldObject = hit.collider.gameObject;
                heldObjectScript = heldObject.GetComponent<PickupAndCarry>();
                heldObjectScript.StartPickup();
            }
        }    
    }

    void OnThrow(InputValue throwValue)
    {
        if (heldObject != null)
        {
            heldObjectScript.Throw();
            pickupCooldown = 1.0f;
            heldObject = null;
        }
    }

    // Update is called once per frame
    void OnTriggerEnter(Collider other)
    {
        switch (other.gameObject.tag)
        {
            case "KillZone":
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                SceneManager.LoadScene("DeathScene");
                break;
            case "AttackZone":
                GameObject.FindGameObjectWithTag("ViewTracker").GetComponent<ViewTracker>().baseOffset-=100;
                break;
            default:
                break;
        }
        
    }
}
