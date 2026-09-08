using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class QuitGame : MonoBehaviour
{
    public void ExitGame() // this quits the game
    {
        Application.Quit();

        Debug.Log("this exits the game");
    }
}
