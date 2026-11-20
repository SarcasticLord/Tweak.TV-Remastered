using UnityEngine;
using TMPro;
using System;

public class SummaryUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI enemiesKilledTxt;  // enemies killed total
    [SerializeField] private TextMeshProUGUI totalStreamTime;   // length of the run
    [SerializeField] private TextMeshProUGUI maxViewersTxt;     // peak number of views in a run
    [SerializeField] private TextMeshProUGUI viewsEndedWith;    // the number of views you ended the run with

    private void Start()
    {
        GameController game = GameController.instance;

        enemiesKilledTxt.text = game.enemiesKilled.ToString();
        maxViewersTxt.text = game.maxViewership.ToString();
        viewsEndedWith.text = game.maxViewership.ToString();


    
        TimeSpan time = TimeSpan.FromSeconds(game.streamLength);

        int hours = time.Hours;
        int minutes = time.Minutes; 
        int seconds = time.Seconds;

        totalStreamTime.text = string.Format("{0:00}:{1:00}:{2:00}", hours, minutes, seconds);

        
    }

}
