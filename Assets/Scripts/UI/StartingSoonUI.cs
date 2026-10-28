using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;


public class StartingSoonUI : MonoBehaviour
{

    public GameObject asylumSelected;
    public GameObject stationSelected;

    public string asylumScene = "Asylum";
    public string stationScene = "Station";

    private string selectedLevel = "";

    public GameObject selectErr;

    public void SelectAsylum()
    {
        asylumSelected.SetActive(true);
        stationSelected.SetActive(false);
        selectedLevel = asylumScene;
    }

    public void SelectStation()
    {
        asylumSelected.SetActive(false);
        stationSelected.SetActive(true);
        selectedLevel = stationScene;
    }

    public void Play()
    {
        if (string.IsNullOrEmpty(selectedLevel))
        {
            Debug.Log("select a level");
            OpenErrWindow();
            return;
        }
        SceneManager.LoadScene(selectedLevel);
    }

    public void OpenErrWindow()
    {
        selectErr.SetActive(true);
    }

    public void CloseErrWindow()
    {
        selectErr.SetActive(false);
    }
}
