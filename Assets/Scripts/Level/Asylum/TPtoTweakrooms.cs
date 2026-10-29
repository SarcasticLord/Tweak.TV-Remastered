using UnityEngine;

public class TPtoTweakrooms : MonoBehaviour
{
    public Transform[] TPpoints;

    private void OnTriggerEnter(Collider other)
    {
        int randomPoint = Random.Range(0, TPpoints.Length);
        
        if (other.CompareTag("Player") || other.CompareTag("Enemy"))
        {
            CharacterController controller = other.GetComponent<CharacterController>();

            
            
            controller.enabled = false;
            other.transform.position = TPpoints[randomPoint].position;
            controller.enabled = true;

        } 
        else
        {
            other.transform.position = TPpoints[randomPoint].position;

        }

    }
}
