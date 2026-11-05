using UnityEngine;

public class PlayerStatusEffectTracker : MonoBehaviour
{
    
    public static event Action<int> onSwarmStatusValueChanged;
    public static event Action<int> onPoisonStatusValueChanged;

    private static float _swarmStatus = 0.0f;
    private float _swarmStatusLimit = 10.0f;
    private float _swarmStatusDecreaseRate = 0.1f;
    public bool swarmed;
    public float swarmStatus
    {
        get {return _swarmStatus}
        set 
        {
            _swarmStatus = Mathf.Clamp(value, 0, _swarmStatusLimit)
            onSwarmStatusValueChanged?.Invoke(_swarmStatus) //Subscribe to delegate in UI script to update UI
        }
    }

    private static float _poisonStatus = 0.0f;
    private float _poisonStatusLimit = 10.0f;
    private float _poisonStatusDecreaseRate = 0.1f;
    public bool poisoned;
    public float poisonStatus
    {
        get {return _poisonStatus}
        set
        {
            _poisonStatus = Mathf.Clamp(value, 0, _poisonStatusLimit)
            onPoisonStatusValueChanged?.Invoke(_poisonStatus) //Subscribe to delegate in UI script to update UI
        }
    }

    protected enum StatusEffect {SWARM, POISON};


    protected void AddToStatus(PlayerStatusEffect.StatusEffect status, float value)//Call in other scripts to add to status
    {
        switch(status)
        {
            case StatusEffect.SWARM:
                if(_swarmStatus <= _swarmStatusLimit)
                {
                    swarmStatus += value;
                }
                break;
            case StatusEffect.POISON:
                if(_poisonStatus <= _poisonStatusLimit)
                {
                    poisonStatus += value;
                }
                break;
            case _:
                
        }
    }

    IEnumerator decreaseStatusEffects
    {
        while(true)
        {
            swarmStatus -= swarmStatusDecreaseRate
        }
    }


}
