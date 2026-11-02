using UnityEngine;

public class TPtoTweakrooms : MonoBehaviour
{
    public Transform[] TPpoints;

    private void OnTriggerEnter(Collider other)
    {
        int randomPoint = Random.Range(0, TPpoints.Length); // 0 - total random point
        
        if (other.CompareTag("Player") || other.CompareTag("Enemy")) // checks the tag of the enemies and the player to send to the backrooms. yes rats can go to the backrooms
        {
            CharacterController controller = other.GetComponent<CharacterController>(); // gets the controller, disables it, tps, then reenables it
            
            controller.enabled = false;
            other.transform.position = TPpoints[randomPoint].position;
            controller.enabled = true;

        } 
        else // for the rats
        {
            other.transform.position = TPpoints[randomPoint].position;

        }

    }
}
