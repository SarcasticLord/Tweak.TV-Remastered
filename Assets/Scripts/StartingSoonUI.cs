using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class StartingSoonUI : MonoBehaviour
{

    public GameObject asylumSelected;
    public GameObject stationSelected;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SelectAsylum()
    {
        asylumSelected.SetActive(true);
        stationSelected.SetActive(false);
        
    }

    public void SelectStation()
    {
        asylumSelected.SetActive(false);
        stationSelected.SetActive(true);
    }
}
