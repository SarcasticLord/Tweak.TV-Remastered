using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{


    public void ExitGame() // this quits the game
    {
        Application.Quit();

        Debug.Log("this exits the game");
    }

    public void ToTitle()
    {
        SceneManager.LoadScene(0); // 0 opens title scene
    }

    public void ToStarting()
    {
        SceneManager.LoadScene(1); // 1 opens the starting soon scene
    }

    public void ToStats()
    {
        SceneManager.LoadScene(4); // 4 opens the stats scene
    }

    public void ToSettings()
    {
        SceneManager.LoadScene(5); // 5 opens the settings scene
    }

    
}
