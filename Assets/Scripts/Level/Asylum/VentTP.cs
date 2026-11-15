using UnityEngine;

public class VentTP : MonoBehaviour
{
    private GameObject[] ventTPpoints;

    void Start()
    {
        ventTPpoints = GameObject.FindGameObjectsWithTag("VentPoint");
    }

    public void Teleport()
    {
        if(ventTPpoints.Length == 0)
            return;

        int randomPoint = Random.Range(0, ventTPpoints.Length); // 0 - total random point

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        CharacterController controller = player.GetComponent<CharacterController>(); // gets the controller, disables it, tps, then reenables it
        
        controller.enabled = false;
        player.transform.position = ventTPpoints[randomPoint].transform.position;
        controller.enabled = true;
             
    }


}