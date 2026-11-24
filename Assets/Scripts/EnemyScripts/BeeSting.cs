using System;
using UnityEngine;

public class BeeSting : AttackHitbox
{
    public PlayerStatusEffectTracker playerStatusTracker;
    public float beeStingStrength = 55.0f;

    protected override void Start()
    {
        base.Start();
    }
    protected override void OnTriggerStay(Collider other)
    {
        foreach (string tag in target)
        {
            if (other.gameObject.CompareTag(tag))
            {
                if(tag == "Player")
                {
                    Debug.Log("Poisoned!");
                    playerStatusTracker = other.gameObject.GetComponent<PlayerStatusEffectTracker>();
                    playerStatusTracker.AddToStatus(PlayerStatusEffectTracker.StatusEffect.POISONED, beeStingStrength);
                }
                //Debug.Log(other.tag.ToString() + " attacked. Decreasing views...");
                Destroy(gameObject);
            }
        }
        //For projectiles touching anything other than target tags
        if ((other.gameObject.layer.ToString() == "Ground") && groundDestroy)
        {
            Destroy(gameObject);
        }
    }
}
