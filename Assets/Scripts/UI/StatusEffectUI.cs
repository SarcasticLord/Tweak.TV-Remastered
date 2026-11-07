using System;
using UnityEngine;

public class StatusEffectUI : MonoBehaviour
{
    [SerializeField] private ProgressBarScript _poisonProgressBar;
    [SerializeField] private ProgressBarScript _swarmProgressBar;

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

    private void UpdatePoisonStatusBar(float currentValue)
    {
        _poisonProgressBar.UpdateProgressBar(currentValue);
    }

    private void UpdateSwarmStatusBar(float currentValue)
    {
        _swarmProgressBar.UpdateProgressBar(currentValue);
    }
}
