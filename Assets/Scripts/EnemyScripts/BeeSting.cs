using System;
using UnityEngine;

public class BeeSting : AttackHitbox
{
    protected PlayerStatusEffects playerStatus;

    protected override void Start()
    {
        base.Start();
    }
    protected override void OnTriggerEnter(Collider other)
    {
        foreach (string tag in target)
        {
            if (other.gameObject.CompareTag(tag))
            {
                if(tag == "Player")
                {
                    playerStatus = other.gameObject.GetComponent<PlayerStatusEffects>();
                    
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
