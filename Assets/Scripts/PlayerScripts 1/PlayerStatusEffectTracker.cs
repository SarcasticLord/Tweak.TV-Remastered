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
    private float _swarmStatusDecreaseRate = 1f;
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

    public enum StatusEffect {SWARM, POISON};

    void Awake()
    {
        StartCoroutine(nameof(DecreaseSwarmStatus));
    }

    public void AddToStatus(PlayerStatusEffectTracker.StatusEffect status, float value)//Call in other scripts to add to status
    {
        switch(status)
        {
            case StatusEffect.SWARM:
                if(_swarmStatus < _swarmStatusLimit && !swarmed)
                {
                    swarmStatus += value;
                }
                else
                {
                    //TODO: Apply effect
                    swarmed = true;
                }
                break;
            case StatusEffect.POISON:
                if(poisoned) return;
                if(_poisonStatus < _poisonStatusLimit)
                {
                    poisonStatus += value;
                }
                else
                {
                    poisoned = true;
                }
                break;
            default:
                Debug.Log("Invalid Status");
                break;
                
        }
    }

    private IEnumerator DecreaseSwarmStatus()
    {
        while(true)
        {
            //Decrease rate when swarm has not been activated
            if(swarmed != true)
            {
                if(_swarmStatus > 0)
                {
                    swarmStatus -= _swarmDecreaseVal;    
                }
                yield return new WaitForSeconds(_swarmStatusDecreaseRate);
            }
            else
            {
                //Can change the rate the effect decreases after it has been triggered
                if(_swarmStatus > 0)
                {
                    swarmStatus -= _swarmDecreaseVal * 10; //Empties Status quicker
                }
                else if(_swarmStatus <= 0)
                {
                    swarmed = false;
                }
                yield return new WaitForSeconds(_swarmStatusDecreaseRate / 2);//TODO: Change the decrease rate when you are affected with the effect
            }
            
        }
    }


}
