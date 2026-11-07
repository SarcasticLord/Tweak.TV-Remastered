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

    private Coroutine _statusTextCoroutine;

    void OnEnable()
    {
        PlayerStatusEffectTracker.onPoisonStatusValueChanged += UpdatePoisonStatusBar;
        PlayerStatusEffectTracker.onSwarmStatusValueChanged += UpdateSwarmStatusBar;
        PlayerStatusEffectTracker.onPlayerStatusChanged += StatusTriggered;
    }
    void OnDisable()
    {
        PlayerStatusEffectTracker.onPoisonStatusValueChanged -= UpdatePoisonStatusBar;
        PlayerStatusEffectTracker.onSwarmStatusValueChanged -= UpdateSwarmStatusBar;
        PlayerStatusEffectTracker.onPlayerStatusChanged -= StatusTriggered;
    }

    void Awake()
    {
        ToggleStatusVisibility(false);
        statusText.text = null;
        _poisonProgressBar = _poisonStatus.GetComponent<ProgressBarScript>();
        _swarmProgressBar = _swarmStatus.GetComponent<ProgressBarScript>();
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
