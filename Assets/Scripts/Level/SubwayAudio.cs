using UnityEngine;

public class SubwayAudio : MonoBehaviour
{
    public AudioSource audioRandomizer;
    public AudioSource[] speakerSources;

    void Start()
    {
        GameObject[] speakers = GameObject.FindGameObjectsWithTag("SubwaySpeakers");
        speakerSources = new AudioSource[speakers.Length];

        for(int i = 0; i < speakers.Length; i++)
        {
            speakerSources[i] = speakers[i].GetComponentInChildren<AudioSource>();
        }
    }
    void Update()
    {
        if (audioRandomizer.isPlaying)
        {
            SyncSpeakers();
        }
    }

    void SyncSpeakers()
    {
        foreach (AudioSource speaker in speakerSources)
        {
            if(speaker.clip != audioRandomizer.clip || !speaker.isPlaying)
            {
                speaker.clip = audioRandomizer.clip;
                speaker.time = audioRandomizer.time;
                speaker.Play();
            }
        }
    }
}

