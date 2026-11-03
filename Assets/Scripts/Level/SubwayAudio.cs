using UnityEngine;

public class SubwayAudio : MonoBehaviour
{
    public AudioClip[] sounds;
    public AudioSource[] speakerSources;

    public float interval = 10f;
    private float timer;

    void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            PlayRandomToAllSpeakers();
            timer = interval;
        }
    }

    void PlayRandomToAllSpeakers()
    {
        // Pick random clip
        AudioClip selected = sounds[Random.Range(0, sounds.Length)];

        // Assign to each speaker
        foreach (AudioSource s in speakerSources)
        {
            s.clip = selected;
        }

        // Sync playback
        foreach (AudioSource s in speakerSources)
        {
            s.Play();
        }
    }
}

