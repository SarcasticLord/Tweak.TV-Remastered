using UnityEngine;
using TMPro;
using System;

public class SummaryUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI enemiesKilledTxt;
    [SerializeField] private TextMeshProUGUI totalStreamTime;
    [SerializeField] private TextMeshProUGUI maxViewersTxt;

    private void Start()
    {
        GameController game = GameController.instance;

        enemiesKilledTxt.text = game.enemiesKilled.ToString();
        maxViewersTxt.text = game.maxViewership.ToString();

    
        TimeSpan time = TimeSpan.FromSeconds(game.streamLength);

        int hours = time.Hours;
        int minutes = time.Minutes; 
        int seconds = time.Seconds;

        totalStreamTime.text = string.Format("{0:00}:{1:00}:{2:00}", hours, minutes, seconds);

        
    }

}
