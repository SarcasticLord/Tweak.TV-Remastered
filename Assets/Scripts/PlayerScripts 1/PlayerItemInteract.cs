using System.Collections;
using System.IO.Compression;
using System.Threading.Tasks;
using Unity.VisualScripting;

//using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerItemInteract : MonoBehaviour
{
    public float maxDistance = 0.1f;
    public GameObject heldObject; //Should be empty in editor
    private PickupAndCarry heldObjectScript; //Should be empty in editor
    public GameObject playerCamera; //should be attached to camera
    public float pickupCooldown = 1f;
    
    public GrafittiChallenge grafittiChallenge; //Should be empty in editor
    public ViewTracker viewTracker; //Should be empty in editor
    public float testRayScatter;
    LayerMask carry;
    
    void OnProvideMarks(InputValue movementValue)
    {
        
        GrafittiChallenge.instance.CommandInputs = movementValue.Get<Vector2>();
    }
    
    //private IEnumerator PickupDelay()
    //{
    //    canPickup = false;
    //    yield return new WaitForSeconds(.5f);
    //    canPickup = true;
    //}

    void Start()
    {
        if (GameObject.FindGameObjectWithTag("ViewTracker").GetComponent<GrafittiChallenge>() != null)
        {
            grafittiChallenge = GameObject.FindGameObjectWithTag("ViewTracker").GetComponent<GrafittiChallenge>();
        }
        
        viewTracker = GameObject.FindGameObjectWithTag("ViewTracker").GetComponent<ViewTracker>();
        carry = LayerMask.GetMask("Carry");
    }
    private void Update()
    {
        
        if(pickupCooldown > 0)
            pickupCooldown -= Time.deltaTime;

    }


    void OnPickup(InputValue pickupValue)
    {
        RaycastHit hit = default;
        if(heldObject == null && pickupCooldown <= 0)
        {
            
            Vector3 rayDirection = playerCamera.transform.forward; //Direction you're facing
            
            bool hasHit = false;

            float x = testRayScatter;
            float y = testRayScatter;
            float z = testRayScatter;

            Vector3 baseOrigin = playerCamera.transform.position;
            Vector3 rayOrigin = new Vector3(baseOrigin.x + x, baseOrigin.y + y, baseOrigin.z + z);
            for (int i = 0; i < 10; i++)
            {
                if(i == 9) {x=0;y=0;z=0;}
                else if (i % 4 == 0) { x *= -1; }
                else if (i % 2 == 0) { y *= -1; }
                else { z *= -1; }

                
                rayOrigin = new Vector3(baseOrigin.x + x, baseOrigin.y + y, baseOrigin.z + z);
                Debug.DrawRay(rayOrigin, rayDirection * maxDistance, Color.red, 1000f);
                
                // Check if THIS specific raycast hit something
                
                if (Physics.Raycast(rayOrigin, rayDirection, out hit, maxDistance, carry))
                {
                    hasHit = true;
                    break; // Stop looping once the first valid hit is found 
                }
                
            }

            if (!hasHit)
            {
                return;
            }
            else if (hit.collider.gameObject.CompareTag("Object") || hit.collider.gameObject.CompareTag("KeyItem")) //If an object or key item
            {
                heldObject = hit.collider.gameObject; //Know who the held object is
                heldObjectScript = heldObject.GetComponent<PickupAndCarry>(); 
                heldObjectScript.StartPickup(); //Have the item react as if its being held
            }
            else if (hit.collider.gameObject.CompareTag("GrafittiWall")) //Start grafitti challenge if appropriate
            {

                TaskEnder taskEnder = hit.collider.gameObject.GetComponent<TaskEnder>();
                grafittiChallenge.canBeStarted=taskEnder.canGrafitti;
                grafittiChallenge.grafittiResult=taskEnder.grafittiImage;
                grafittiChallenge.wallTaskEnder=taskEnder;

                grafittiChallenge.Activate();
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
                //if (grafittiChallenge.systemActive){grafittiChallenge.FailState();} //Fail them if they get hit in the minigame
                Cursor.visible = true;
                SceneManager.LoadScene("DeathScene");
                break;
            case "AttackZone":
                GameObject.FindGameObjectWithTag("ViewTracker").GetComponent<ViewTracker>().baseOffset-=100;
                if(grafittiChallenge.systemActive){grafittiChallenge.FailState();} //Fail them if they get hit in the minigame
                break;
            default:
                break;
        }
        
    }
}
