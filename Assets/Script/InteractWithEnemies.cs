using UnityEngine;

public class InteractWithEnemies : MonoBehaviour
{
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("StreamSniper"))
        {
            DeathDummy();
        }
    }

    void DeathDummy()
    {
        Debug.Log("This is an example function");
    }
}
