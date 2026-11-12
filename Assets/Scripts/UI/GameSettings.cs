using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class GameSettings : MonoBehaviour
{
    [Header("Volume")]
    public AudioMixer masterAudio;
    public Slider masterSlider;
    public Slider announcerSlider;
    public Slider flashSlider;


    [Header("sensitivity")]
    public Slider SenseSlider;
    public PlayerCameraScript playerCamera;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // saves the players choices and loads it when it starts

        //master
        float saveMasterVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
        masterSlider.value = saveMasterVolume;

        SetMasterVolume(saveMasterVolume);
        masterSlider.onValueChanged.AddListener(SetMasterVolume);

        // subway announcer
        float saveAnnouncerVolume = PlayerPrefs.GetFloat("Announcer", 1f);
        announcerSlider.value = saveAnnouncerVolume;

        SetAnnouncerVolume(saveAnnouncerVolume);
        announcerSlider.onValueChanged.AddListener(SetAnnouncerVolume);

        // cam flash  
        float saveFlashVolume = PlayerPrefs.GetFloat("CamFlash", 1f);
        flashSlider.value = saveFlashVolume;

        SetFlashVolume(saveAnnouncerVolume);
        flashSlider.onValueChanged.AddListener(SetFlashVolume);

        // camera sense
        float saveSense = PlayerPrefs.GetFloat("CameraSense", 2f);
        SenseSlider.value = saveSense;

        SetCameraSense(saveSense);
        SenseSlider.onValueChanged.AddListener(SetCameraSense);
    }

    public void SetMasterVolume(float volume)
    {
        masterAudio.SetFloat("MasterVolume", Mathf.Log10(Mathf.Max(volume, 0.0001f)) * 20);
        PlayerPrefs.SetFloat("MasterVolume", volume);
    }

    public void SetAnnouncerVolume(float volume)
    {
        masterAudio.SetFloat("Announcer", Mathf.Log10(Mathf.Max(volume, 0.0001f)) * 20);
        PlayerPrefs.SetFloat("Announcer", volume);
    }

    public void SetFlashVolume(float volume)
    {
        masterAudio.SetFloat("CamFlash", Mathf.Log10(Mathf.Max(volume, 0.0001f)) * 20);
        PlayerPrefs.SetFloat("CamFlash", volume);
    }

    public void SetCameraSense(float sensitivity)
    {
        PlayerPrefs.SetFloat("CameraSense", sensitivity);

        if(playerCamera != null)
        {
            playerCamera.sensitivity = sensitivity;
        }
    }
}
