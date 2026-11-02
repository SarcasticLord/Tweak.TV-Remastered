using UnityEngine;

public class VentTP : MonoBehaviour
{
    private GameObject[] ventTPpoints;

    void Start()
    {
        ventTPpoints = GameObject.FindGameObjectsWithTag("VentPoint");
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if(!other.CompareTag("Player"))
            return;
        int randomPoint = Random.Range(0, ventTPpoints.Length); // 0 - total random point
        
        if (other.CompareTag("Player"))
        {
            Debug.Log("tp " + other.name); 
            CharacterController controller = other.GetComponent<CharacterController>(); // gets the controller, disables it, tps, then reenables it
                
            controller.enabled = false;
            other.transform.position = ventTPpoints[randomPoint].transform.position;
            controller.enabled = true;
        }
             

    }
}
