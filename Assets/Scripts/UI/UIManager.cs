using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

[System.Serializable]


public class UIManager : MonoBehaviour
{
    // keeps track of the last played scene
    public static string lastScene = "";


    public void ExitGame() // this quits the game
    {
        Application.Quit();

        Debug.Log("this exits the game");
    }

    public void ToTitle()
    {
        lastScene = "TitleScene";
        SceneManager.LoadScene("TitleScene"); // 0 opens title scene
    }

    public void ToStarting()
    {
        SceneManager.LoadScene("StartingScene"); // 1 opens the starting soon scene
    }

    public void ToStats()
    {
        SceneManager.LoadScene("StatsScene"); // 4 opens the stats scene
    }

    public void ToSettings()
    {
        SceneManager.LoadScene("SettingsScene"); // 5 opens the settings scene
    }

    public void ToDeath()
    {
        SceneManager.LoadScene("DeathScene");
    }
    public void ToDemo()
    {
        lastScene = "DemoScene";
        SceneManager.LoadScene("DemoScene"); 
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
