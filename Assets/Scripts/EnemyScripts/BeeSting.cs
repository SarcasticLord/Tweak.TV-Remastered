using System;
using UnityEngine;

public class BeeSting : AttackHitbox
{
    public PlayerStatusEffects playerStatus;

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
                    Debug.Log("Poisoned!");
                    playerStatus = other.gameObject.GetComponent<PlayerStatusEffects>();
                    playerStatus.OnPoison();
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
