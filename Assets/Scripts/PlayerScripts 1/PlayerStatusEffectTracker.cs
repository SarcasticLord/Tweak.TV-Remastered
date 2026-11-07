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
    public bool swarmed = false;
    private Coroutine _swarmDecreaseCoroutine;
    public float swarmStatus
    {
        get {return _swarmStatus;}
        set
        {
            _swarmStatus = Mathf.Clamp(value, 0, _swarmStatusLimit + 20); //+20 as a buffer incase the swarm overcaps
            onSwarmStatusValueChanged?.Invoke(_swarmStatus); //Subscribe to delegate in UI script to update UI
            if (_swarmStatus > 0 && _swarmDecreaseCoroutine == null)
            {
                _swarmDecreaseCoroutine = StartCoroutine(DecreaseSwarmStatus());
            }
        }
    }

    private static float _poisonStatus = 0.0f;
    private float _poisonStatusLimit = 100.0f;
    private float _poisonStatusDecreaseRate = 0.1f;
    public bool poisoned;
    private Coroutine _poisonDecreaseCoroutine;
    public float poisonStatus
    {
        get {return _poisonStatus;}
        set
        {
            _poisonStatus = Mathf.Clamp(value, 0, _poisonStatusLimit);
            onPoisonStatusValueChanged?.Invoke(_poisonStatus); //Subscribe to delegate in UI script to update UI
            if(_poisonStatus > 0 && _poisonDecreaseCoroutine == null)
            {
                //TODO: Setup poison status
                return; //_poisonDecreaseCoroutine = StartCoroutine(DecreasePoisonStatus());
            }
        }
    }

    public enum StatusEffect {SWARM, POISON};

    public void AddToStatus(PlayerStatusEffectTracker.StatusEffect status, float value)//Call in other scripts to add to status
    {
        switch(status)
        {
            case StatusEffect.SWARM:
                Debug.Log("swarming!");
                if(swarmStatus < _swarmStatusLimit && !swarmed)
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
        yield return new WaitForSeconds(1f);
        while (swarmStatus > 0)
        {
            
            //Decrease rate when swarmed status has not been activated
            if(!swarmed)
            {
                swarmStatus -= _swarmDecreaseVal;    
                yield return new WaitForSeconds(_swarmStatusDecreaseRate);
            }
            else
            {
                //Can change the rate the effect decreases after it has been triggered
                swarmStatus -= _swarmDecreaseVal * 10; //Empties Status quicker

                if(_swarmStatus <= 0)
                {
                    swarmed = false;
                }
                yield return new WaitForSeconds(_swarmStatusDecreaseRate / 2f);//TODO: Change the decrease rate when you are affected with the effect
            }
            
        }
        _swarmDecreaseCoroutine = null;
    }


}
