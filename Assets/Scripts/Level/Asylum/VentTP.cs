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
        int randomPoint = Random.Range(0, ventTPpoints.Length); // 0 - total random point
        
        if (other.CompareTag("Player"))
        {
            CharacterController controller = other.GetComponent<CharacterController>(); // gets the controller, disables it, tps, then reenables it
                
            controller.enabled = false;
            other.transform.position = ventTPpoints[randomPoint].transform.position;
            controller.enabled = true;
        }
             

    }
}
