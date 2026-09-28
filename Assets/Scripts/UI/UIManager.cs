using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static string lastScene = "";


    public void ExitGame() // this quits the game
    {
        Application.Quit();

        Debug.Log("this exits the game");
    }

    public void ToTitle()
    {
        SceneManager.LoadScene("TitleScene"); // 0 opens title scene
    }

    public void ToStarting()
    {
        SceneManager.LoadScene("StartingSoon"); // 1 opens the starting soon scene
    }

    public void ToStats()
    {
        SceneManager.LoadScene("StatScene"); // 4 opens the stats scene
    }

    public void ToSettings()
    {
        SceneManager.LoadScene("SettingsScene"); // 5 opens the settings scene
    }

    public void ToDemo()
    {
        SceneManager.LoadScene("DemoScene"); 
    }

    public void RestartLevel()
    {
        if (lastScene == "DemoScene")
        {
            Debug.Log("Last scene:" + lastScene);
            SceneManager.LoadScene("DemoScene");
        }

        else if (lastScene == "TweakAsylum")
        {
            // when we add the ther levels add it here
        }    
    }
}
