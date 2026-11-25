using UnityEngine;

public class StreamSniperProximityTrigger : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))return;

        if(other.TryGetComponent(out PlayerProximityScript proxy))
        {
            proxy.StreamSniperInProximity(transform);
        }
    }

    // void OnTriggerExit(Collider other)
    // {
    //     if(!other.CompareTag("Player"))return;

    //     if(other.TryGetComponent(out PlayerProximityScript proxy))
    //     {
    //         proxy.ExitedStreamSniperProximity();
    //     }
    // }
}
