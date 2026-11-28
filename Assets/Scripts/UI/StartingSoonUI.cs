using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;


public class StartingSoonUI : MonoBehaviour
{

    public GameObject asylumSelected;
    public GameObject stationSelected;
    public GameObject demoSelected;
    public GameObject centerSelected;


    public string asylumScene = "Asylum";
    public string stationScene = "LoadingScene";
    public string demoScene = "Tutorial";
    public string centerScene = "Center";

    public static string lastScene = "";

    private string selectedLevel = "";

    public GameObject selectErr;
    

    public void SelectAsylum()
    {
        lastScene = "Asylum";
        TaskMasterScript.instance.level="asylum";
        asylumSelected.SetActive(true);
        stationSelected.SetActive(false);
        demoSelected.SetActive(false);
        centerSelected.SetActive(false);
        selectedLevel = asylumScene;
        TaskMasterScript.instance.EndAllTasks();
        TaskMasterScript.instance.ApplyAsylumTasks();
    }

    public void SelectStation()
    {
        lastScene = "LoadingScene";
        TaskMasterScript.instance.level="subway";
        asylumSelected.SetActive(false);
        stationSelected.SetActive(true);
        demoSelected.SetActive(false);
        centerSelected.SetActive(false);
        selectedLevel = stationScene;
        TaskMasterScript.instance.EndAllTasks();
        TaskMasterScript.instance.ApplySubwayTasks();
    }

    public void SelectDemo()
    {
        lastScene = "Tutorial";
        //TaskMasterScript.instance.level="Tutorial";
        asylumSelected.SetActive(false);
        stationSelected.SetActive(false);
        demoSelected.SetActive(true);
        centerSelected.SetActive(false);
        selectedLevel = demoScene;
        //TaskMasterScript.instance.EndAllTasks();
        //TaskMasterScript.instance.ApplySubwayTasks();
    }

    public void SelectCenter()
    {
        lastScene = "Center";
        //TaskMasterScript.instance.level="Tutorial";
        asylumSelected.SetActive(false);
        stationSelected.SetActive(false);
        demoSelected.SetActive(false);
        centerSelected.SetActive(true);
        selectedLevel = centerScene;
        //TaskMasterScript.instance.EndAllTasks();
        //TaskMasterScript.instance.ApplySubwayTasks();
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

     public void RestartLevel()
    {
        if (string.IsNullOrEmpty(lastScene))
        {
            Debug.Log("loading last scene");
        } // after you die it opens the last scene
        SceneManager.LoadScene(lastScene);
    }
}
