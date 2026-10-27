using System.Collections;
using System.Threading.Tasks;
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
    public TaskMasterScript taskMasterScript; //Should be empty in editor
    public GrafittiChallenge grafittiChallenge; //Should be empty in editor
    public ViewTracker viewTracker; //Should be empty in editor
    
    
    
    //private IEnumerator PickupDelay()
    //{
    //    canPickup = false;
    //    yield return new WaitForSeconds(.5f);
    //    canPickup = true;
    //}

    void Start()
    {
        grafittiChallenge = GameObject.FindGameObjectWithTag("ViewTracker").GetComponent<GrafittiChallenge>();
        viewTracker = GameObject.FindGameObjectWithTag("ViewTracker").GetComponent<ViewTracker>();
        taskMasterScript = GameObject.FindGameObjectWithTag("ViewTracker").GetComponent<TaskMasterScript>();
    }
    private void Update()
    {
        
        if(pickupCooldown > 0)
            pickupCooldown -= Time.deltaTime;

    }


    void OnPickup(InputValue pickupValue)
    {
        if(heldObject == null && pickupCooldown <= 0)
        {
            Vector3 rayOrigin =  new(playerCamera.transform.position.x, playerCamera.transform.position.y, playerCamera.transform.position.z); //Where the box is aimed
            Vector3 rayDirection = playerCamera.transform.forward; //Direction you're facing

            // Define the dimensions of your box
            Vector3 boxHalfExtents = new Vector3(0.5f, 0.5f, 0.5f); // Total size is 1m x 1m x 1m
            Quaternion boxOrientation = playerCamera.transform.rotation; // Aligns the box with the camera's angle

            // Debug visualization: Draws a line representing the center trajectory of the box
            Debug.DrawRay(rayOrigin, rayDirection * maxDistance, Color.green);

            // Cast the box forward
            if (Physics.BoxCast(rayOrigin, boxHalfExtents, rayDirection, out RaycastHit hit, boxOrientation, maxDistance)) //Casts a box forward
            {
                if (hit.collider.gameObject.CompareTag("Object") || hit.collider.gameObject.CompareTag("KeyItem")) //If an object or key item
                {
                    heldObject = hit.collider.gameObject; //Know who the held object is
                    heldObjectScript = heldObject.GetComponent<PickupAndCarry>(); 
                    heldObjectScript.StartPickup(); //Have the item react as if its being held
                }
                if (hit.collider.gameObject.CompareTag("GrafittiWall")) //Start grafitti challenge if appropriate
                {
                    if(taskMasterScript.currentTask.TaskID!=3){return;}
                    grafittiChallenge.Activate();
                }
                
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
