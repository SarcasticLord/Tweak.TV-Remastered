using System;
using UnityEngine;
using TMPro;
using System.Collections;

public class StatusEffectUI : MonoBehaviour
{
    [SerializeField] private GameObject StatusPanel;
    [SerializeField] private TextMeshProUGUI statusText;

    [SerializeField] private GameObject _poisonStatus;
    private ProgressBarScript _poisonProgressBar;
    [SerializeField] private GameObject _swarmStatus;
    private ProgressBarScript _swarmProgressBar;
    [SerializeField] private GameObject _shockStatus;
    private ProgressBarScript _shockProgressBar;
    [SerializeField] private GameObject _trappedStatus;
    private ProgressBarScript _trappedProgressBar;

    private Coroutine _statusTextCoroutine;

    void OnEnable()
    {
        PlayerStatusEffectTracker.onPoisonStatusValueChanged += UpdatePoisonStatusBar;
        PlayerStatusEffectTracker.onSwarmStatusValueChanged += UpdateSwarmStatusBar;
        PlayerStatusEffectTracker.onShockStatusValueChanged += UpdateShockStatusBar;
        PlayerStatusEffectTracker.onTrappedStatusValueChanged += UpdateTrappedStatusBar; 
        PlayerStatusEffectTracker.onPlayerStatusChanged += StatusTriggered;
    }
    void OnDisable()
    {
        PlayerStatusEffectTracker.onPoisonStatusValueChanged -= UpdatePoisonStatusBar;
        PlayerStatusEffectTracker.onSwarmStatusValueChanged -= UpdateSwarmStatusBar;
        PlayerStatusEffectTracker.onShockStatusValueChanged -= UpdateShockStatusBar;
        PlayerStatusEffectTracker.onTrappedStatusValueChanged -= UpdateTrappedStatusBar;
        PlayerStatusEffectTracker.onPlayerStatusChanged -= StatusTriggered;
    }

    void Awake()
    {
        ToggleStatusVisibility(false);
        statusText.text = null;
        _poisonProgressBar = _poisonStatus.GetComponent<ProgressBarScript>();
        _swarmProgressBar = _swarmStatus.GetComponent<ProgressBarScript>();
        _shockProgressBar = _shockStatus.GetComponent<ProgressBarScript>();
        _trappedProgressBar = _trappedStatus.GetComponent<ProgressBarScript>();
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

    private void UpdateShockStatusBar(float currentValue)
    {
        if(currentValue > 0)
        {
            _shockStatus.SetActive(true);
            ToggleStatusVisibility(true);
            _shockProgressBar.UpdateProgressBar(currentValue);
        }
        else
        {
            _shockStatus.SetActive(false);
            ToggleStatusVisibility(false);
        }
    }

    private void UpdateTrappedStatusBar(float currentValue)
    {
        if(currentValue > 0)
        {
            _trappedStatus.SetActive(true);
            ToggleStatusVisibility(true);
            _trappedProgressBar.UpdateProgressBar(currentValue);
        }
        else
        {
            _trappedStatus.SetActive(false);
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
            //Switch to for loop if we make more than 5 statuses + restructure status gameobject in PlayerCanvas prefab 
            if(_poisonStatus.activeSelf) return;
            if(_swarmStatus.activeSelf) return;
            if(_shockStatus.activeSelf) return;
            if(_trappedStatus.activeSelf) return;
            StatusPanel.SetActive(false);
        }
    }

    private void StatusTriggered(PlayerStatusEffectTracker.StatusEffect newStatus)
    {
        //Stop coroutine if an old alert is still fading out
        if(_statusTextCoroutine != null)
        {
            StopCoroutine(_statusTextCoroutine);
        }
        _statusTextCoroutine = StartCoroutine(ShowStatusTextRoutine(newStatus)) ;
    }

    private IEnumerator ShowStatusTextRoutine(PlayerStatusEffectTracker.StatusEffect newStatus)
    {
        statusText.text = newStatus.ToString() + "!!";
        yield return new WaitForSeconds(3f);
        statusText.text = "";
        _statusTextCoroutine = null;
    }
}
