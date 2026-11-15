using UnityEngine;

public class TPToAsylum : MonoBehaviour
{
    public Transform BackAsylum;

    private void OnTriggerEnter(Collider other)
    {
        
        if (other.CompareTag("Player") || other.CompareTag("Enemy")) // checks the tag of the enemies and the player to send to the backrooms. yes rats can go to the backrooms
        {
            CharacterController controller = other.GetComponent<CharacterController>(); // gets the controller, disables it, tps, then reenables it
            
            controller.enabled = false;
            other.transform.position = BackAsylum.position;
            controller.enabled = true;

        } 
    }
}
