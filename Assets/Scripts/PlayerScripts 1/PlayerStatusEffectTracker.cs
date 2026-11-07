using System;
using System.Collections;
using UnityEngine;

public class PlayerStatusEffectTracker : MonoBehaviour
{
    
    public static event Action<float> onSwarmStatusValueChanged;
    public static event Action<float> onPoisonStatusValueChanged;

    private static float _swarmStatus = 0.0f;
    private float _swarmStatusLimit = 100.0f;
    private float _swarmDecreaseVal = 1f;
    private float _swarmStatusDecreaseRate = 0.1f;
    public bool swarmed;
    public float swarmStatus
    {
        get {return _swarmStatus;}
        set 
        {
            _swarmStatus = Mathf.Clamp(value, 0, _swarmStatusLimit);
            onSwarmStatusValueChanged?.Invoke(_swarmStatus); //Subscribe to delegate in UI script to update UI
        }
    }

    private static float _poisonStatus = 0.0f;
    private float _poisonStatusLimit = 100.0f;
    private float _poisonStatusDecreaseRate = 0.1f;
    public bool poisoned;
    public float poisonStatus
    {
        get {return _poisonStatus;}
        set
        {
            _poisonStatus = Mathf.Clamp(value, 0, _poisonStatusLimit);
            onPoisonStatusValueChanged?.Invoke(_poisonStatus); //Subscribe to delegate in UI script to update UI
        }
    }

    protected enum StatusEffect {SWARM, POISON};


    protected void AddToStatus(PlayerStatusEffectTracker.StatusEffect status, float value)//Call in other scripts to add to status
    {
        switch(status)
        {
            case StatusEffect.SWARM:
                if(swarmed) return;
                if(_swarmStatus <= _swarmStatusLimit)
                {
                    swarmStatus += value;
                }
                else
                {
                    swarmed = true;
                }
                break;
            case StatusEffect.POISON:
                if(poisoned) return;
                if(_poisonStatus <= _poisonStatusLimit)
                {
                    poisonStatus += value;
                }
                else
                {
                    poisoned = true;
                }
                break;
            default:
                break;
                
        }
    }

    private IEnumerator decreaseSwarmStatus()
    {
        while(true)
        {
            if(swarmStatus <= 0) yield break;
            //Decrease rate when swarm has not been activated
            if(swarmed != true)
            {
                swarmStatus -= _swarmDecreaseVal;
                yield return new WaitForSeconds(_swarmStatusDecreaseRate);
            }
            else
            {
                //Can change the rate the effect decreases after it has been triggered
                swarmStatus -= _swarmDecreaseVal;
                if(swarmStatus <= 0)
                {
                    swarmed = false;
                }
                yield return new WaitForSeconds(_swarmStatusDecreaseRate * 2);//TODO: Change the decrease rate when you are affected with the effect
            }
            
        }
    }


}
