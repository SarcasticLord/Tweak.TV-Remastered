using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class TweakSelectionUI : MonoBehaviour
{

    public GameObject spawnSSImmUI;
    public GameObject fastViewDeclineUI;
    public GameObject moreTrapsUI;
    public GameObject allLightsOffUI;
    public GameObject cameraFiltersUI;
    public GameObject moreEnemieUI;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SelectSpawnSS()
    {
        spawnSSImmUI.SetActive(true);
        fastViewDeclineUI.SetActive(false);
        moreTrapsUI.SetActive(false);
        allLightsOffUI.SetActive(false);
        cameraFiltersUI.SetActive(false);
        moreEnemieUI.SetActive(false);
    }

    
}
