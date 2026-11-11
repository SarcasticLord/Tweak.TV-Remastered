using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class GameSettings : MonoBehaviour
{
    [Header("Volume")]
    public AudioMixer masterAudio;
    public Slider volumeSlider;

    [Header("sensitivity")]
    public Slider SenseSlider;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // saves the players choices and loads it when it starts
        float saveVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
        volumeSlider.value = saveVolume;

        SetMasterVolume(saveVolume);
        volumeSlider.onValueChanged.AddListener(SetMasterVolume);

        float saveSense = PlayerPrefs.GetFloat("CameraSense", 2f);
        SenseSlider.value = saveSense;
        SenseSlider.onValueChanged.AddListener(SetCameraSense);
    }

    public void SetMasterVolume(float volume)
    {
        masterAudio.SetFloat("MasterVolume", Mathf.Log10(Mathf.Max(volume, 0.0001f)) * 20);
        PlayerPrefs.SetFloat("MasterVolume", volume);
    }

    public void SetCameraSense(float sensitivity)
    {
        PlayerPrefs.SetFloat("CameraSense", sensitivity);
    }
}
