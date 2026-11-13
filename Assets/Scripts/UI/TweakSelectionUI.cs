using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;

public class TweakSelectionUI : MonoBehaviour
{
    [Header("Panel")]
    public GameObject spawnSSImmUI;
    public GameObject fastViewDeclineUI;
    public GameObject moreTrapsUI;
    public GameObject allLightsOffUI;
    public GameObject cameraFiltersUI;
    public GameObject moreEnemiesUI;

    [Header("Toggles")]
    public Toggle spawnSSImmToggle;
    public Toggle fastViewDeclineToggle;
    public Toggle moreTrapsToggle;
    public Toggle allLightsOffToggle;
    public Toggle cameraFiltersToggle;
    public Toggle moreEnemiesToggle;

    void Awake()
    {
        spawnSSImmToggle.onValueChanged.AddListener(TweakManager.instance.FlipspawnSSImm);
        fastViewDeclineToggle.onValueChanged.AddListener(TweakManager.instance.FlipfastViewDecline);
        moreTrapsToggle.onValueChanged.AddListener(TweakManager.instance.FlipmoreTraps);
        allLightsOffToggle.onValueChanged.AddListener(TweakManager.instance.FlipallLightsOff);
        cameraFiltersToggle.onValueChanged.AddListener(TweakManager.instance.FlipcameraFilters);
        moreEnemiesToggle.onValueChanged.AddListener(TweakManager.instance.FlipmoreEnemies);
    }
    

// these are for the buttons. clicking a button to view the description will set its desc true and set all the othes to false
// is there a better way to do this? probably
    public void SelectSpawnSS()
    {
        spawnSSImmUI.SetActive(true);
        fastViewDeclineUI.SetActive(false);
        moreTrapsUI.SetActive(false);
        allLightsOffUI.SetActive(false);
        cameraFiltersUI.SetActive(false);
        moreEnemiesUI.SetActive(false);
        
    }

    public void SelectFastDecline()
    {
        spawnSSImmUI.SetActive(false);
        fastViewDeclineUI.SetActive(true);
        moreTrapsUI.SetActive(false);
        allLightsOffUI.SetActive(false);
        cameraFiltersUI.SetActive(false);
        moreEnemiesUI.SetActive(false);
        
    }

    public void SelectMoreTraps()
    {
        spawnSSImmUI.SetActive(false);
        fastViewDeclineUI.SetActive(false);
        moreTrapsUI.SetActive(true);
        allLightsOffUI.SetActive(false);
        cameraFiltersUI.SetActive(false);
        moreEnemiesUI.SetActive(false);
        
    }

    public void SelectLightsOff()
    {
        spawnSSImmUI.SetActive(false);
        fastViewDeclineUI.SetActive(false);
        moreTrapsUI.SetActive(false);
        allLightsOffUI.SetActive(true);
        cameraFiltersUI.SetActive(false);
        moreEnemiesUI.SetActive(false);
        
    }

    public void SelectCameraFilters()
    {
        spawnSSImmUI.SetActive(false);
        fastViewDeclineUI.SetActive(false);
        moreTrapsUI.SetActive(false);
        allLightsOffUI.SetActive(false);
        cameraFiltersUI.SetActive(true);
        moreEnemiesUI.SetActive(false);
    }

    public void SelectMoreEnemies()
    {
        spawnSSImmUI.SetActive(false);
        fastViewDeclineUI.SetActive(false);
        moreTrapsUI.SetActive(false);
        allLightsOffUI.SetActive(false);
        cameraFiltersUI.SetActive(false);
        moreEnemiesUI.SetActive(true);
    }

    
}
