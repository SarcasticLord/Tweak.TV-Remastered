using System;
using UnityEngine;

public class StatusEffectUI : MonoBehaviour
{
    [SerializeField] private GameObject StatusPanel;

    [SerializeField] private GameObject _poisonStatus;
    private ProgressBarScript _poisonProgressBar;
    [SerializeField] private GameObject _swarmStatus;
    private ProgressBarScript _swarmProgressBar;

    public float testPoisonValue = 100;
    public float testSwarmValue = 100;

    void OnEnable()
    {
        PlayerStatusEffectTracker.onPoisonStatusValueChanged += UpdatePoisonStatusBar;
        PlayerStatusEffectTracker.onSwarmStatusValueChanged += UpdateSwarmStatusBar;
    }
    void OnDisable()
    {
        PlayerStatusEffectTracker.onPoisonStatusValueChanged -= UpdatePoisonStatusBar;
        PlayerStatusEffectTracker.onSwarmStatusValueChanged -= UpdateSwarmStatusBar;   
    }

    void Awake()
    {
        _poisonProgressBar = _poisonStatus.GetComponent<ProgressBarScript>();
        _swarmProgressBar = _swarmStatus.GetComponent<ProgressBarScript>();
    }

    void Update()
    {
        UpdatePoisonStatusBar(testPoisonValue);
        UpdateSwarmStatusBar(testSwarmValue);
    }

    private void UpdatePoisonStatusBar(float currentValue)
    {
        if(currentValue > 0)
        {
            _poisonStatus.SetActive(true);
            ToggleStatusVisibility(true);
            _poisonProgressBar.UpdateProgressBar(currentValue);
        }
        else
        {
            _poisonStatus.SetActive(false);
            ToggleStatusVisibility(false);
        }
    }

    private void UpdateSwarmStatusBar(float currentValue)
    {
        if(currentValue > 0)
        {
            _swarmStatus.SetActive(true);
            ToggleStatusVisibility(true);
            _swarmProgressBar.UpdateProgressBar(currentValue);
        }
        else
        {
            _swarmStatus.SetActive(false);
            ToggleStatusVisibility(false);   
        }
    }

    private void ToggleStatusVisibility(bool visbility)
    {
        if(visbility)
        {
            StatusPanel.SetActive(true);
        }
        else
        {
            //Check if any status bar is visible before disabling visibility
            if(_poisonStatus.activeSelf) return;
            if(_swarmStatus.activeSelf) return;
            StatusPanel.SetActive(false);
        }
    }
}
