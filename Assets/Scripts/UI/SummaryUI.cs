using UnityEngine;
using TMPro;
using System;
using Unity.VisualScripting;

public class SummaryUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI enemiesKilledTxt;  // enemies killed total
    [SerializeField] private TextMeshProUGUI totalStreamTime;   // length of the run
    [SerializeField] private TextMeshProUGUI maxViewersTxt;     // peak number of views in a run
    [SerializeField] private TextMeshProUGUI viewsEndedWith;    // the number of views you ended the run with
    [SerializeField] private TextMeshProUGUI subscribersGained; //How many subs you got

    private void Start()
    {

        enemiesKilledTxt.text = " " + GameController.enemiesKilled.ToString();
        maxViewersTxt.text = " " + GameController.maxViewership.ToString();
        viewsEndedWith.text = " " + GameController.instance.viewCount.ToString();
    
        TimeSpan time = TimeSpan.FromSeconds(GameController.streamLength);

        int hours = time.Hours;
        int minutes = time.Minutes; 
        int seconds = time.Seconds;

        totalStreamTime.text = string.Format("{0:00}:{1:00}:{2:00}", hours, minutes, seconds);
        
        GameController.instance.ConvertViewsToSubscribers();
        subscribersGained.text = subscribersGained.ToString();

    }

}
