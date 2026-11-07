using System.Runtime.CompilerServices;
using UnityEngine;

public class RatHitbox : AttackHitbox
{
    private PlayerStatusEffectTracker _playerStatus;

    private void OnTriggerEnter(Collider other)
    {

        foreach (string tag in target)
        {
            if (other.gameObject.CompareTag(tag))
            {
                //Debug.Log(other.tag.ToString() + " attacked. Decreasing views...");
                _playerStatus = other.gameObject.GetComponent<PlayerStatusEffectTracker>();
                _playerStatus.AddToStatus(PlayerStatusEffectTracker.StatusEffect.SWARMED, 10);
                Destroy(gameObject);
            }
            //For projectiles touching ground
            else if ((other.gameObject.layer.ToString() == "Ground") && groundDestroy)
            {
                Destroy(gameObject);
            }
        }
    }
}
