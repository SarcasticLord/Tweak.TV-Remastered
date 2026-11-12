using UnityEngine;

public class TrackAudioTrigger : MonoBehaviour
{
    public SubwayAudio subwayAudio;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("OnTracks"))
        {
            subwayAudio.StartTrackAudio();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("OnTracks"))
        {
            subwayAudio.StopTrackAudio();
        }
    }
}
