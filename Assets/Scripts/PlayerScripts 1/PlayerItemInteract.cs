using System.Collections;
using System.Threading.Tasks;
using Microsoft.Unity.VisualStudio.Editor;
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
    public TaskMasterScript taskMasterScript;
    public GrafittiChallenge grafittiChallenge;
    public ViewTracker viewTracker;
    
    
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
            Vector3 rayOrigin = playerCamera.transform.position;
            Vector3 rayDirection = playerCamera.transform.forward * maxDistance;

            // Draw the ray from the camera's position
            Debug.DrawRay(rayOrigin, rayDirection, Color.green);

            if (Physics.Raycast(rayOrigin, playerCamera.transform.forward, out RaycastHit hit, maxDistance))
            {
                if (hit.collider.gameObject.CompareTag("Object") || hit.collider.gameObject.CompareTag("KeyItem"))
                {
                    heldObject = hit.collider.gameObject;
                    heldObjectScript = heldObject.GetComponent<PickupAndCarry>();
                    heldObjectScript.StartPickup();
                }
                if (hit.collider.gameObject.CompareTag("GrafittiWall"))
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
                if(grafittiChallenge.systemActive){grafittiChallenge.FailState();} //Fail them if they get hit in the minigame
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
