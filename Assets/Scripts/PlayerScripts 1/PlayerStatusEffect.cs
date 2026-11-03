using UnityEngine;

public class PlayerStatusEffect : MonoBehaviour
{
    public static float swarmStatus = 0;
    private float swarmStatusLimit = 10;

    protected enum StatusEffect {SWARM};
    protected void AddToStatus(PlayerStatusEffect.StatusEffect status, float value)
    {
        switch(status)
        {
            case StatusEffect.SWARM:
                if(swarmStatus <= swarmStatusLimit)
                {
                    swarmStatus += value;
                }
                break;

        }
    }
}
