using System.ComponentModel.Design;
using UnityEngine;
using UnityEngine.UIElements;

public class SubwayAudio : MonoBehaviour
{
    public AudioSource stationAudio;

    public AudioSource overTrackAudio;
    public AudioSource[] speakerSources;

    private AudioSource activeAudio;

    void Start()
    {
        GameObject[] speakers = GameObject.FindGameObjectsWithTag("SubwaySpeaker");
        speakerSources = new AudioSource[speakers.Length];

        for(int i = 0; i < speakers.Length; i++)
        {
            speakerSources[i] = speakers[i].GetComponentInChildren<AudioSource>();
        }
        activeAudio = stationAudio;
        stationAudio.Play();

        Debug.Log("playing: " + activeAudio);
    }
    void Update()
    {
        if (activeAudio.isPlaying)
        {
            SyncSpeakers();
        }
        else
        {
            StopSpeakers();
        }
    }

    void SyncSpeakers()
    {
        foreach (AudioSource speaker in speakerSources)
        {
            if(speaker.clip != activeAudio.clip || !speaker.isPlaying)
            {
                speaker.clip = activeAudio.clip;
                speaker.time = activeAudio.time;
                speaker.Play();
            }
        }
    }

    void StopSpeakers()
    {
        foreach (AudioSource speaker in speakerSources)
        {
            speaker.Stop();
        }
    }

    // this is for swiching audio sources when the player steps on the tracks
    // the actual detection is a script on the player called TrackAudioTrigger.cs in the PlayerScripts 1 folder
    public void StartTrackAudio()
    {
        stationAudio.Stop();

        activeAudio = overTrackAudio;
        overTrackAudio.Play();

        StopSpeakers();

        Debug.Log("playing: " + activeAudio);
    }

    public void StopTrackAudio()
    {
        overTrackAudio.Stop();

        activeAudio = stationAudio;
        stationAudio.Play();

        StopSpeakers();

        Debug.Log("playing: " + activeAudio);
    }
}

