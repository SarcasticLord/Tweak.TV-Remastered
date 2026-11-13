using System;
using UnityEngine;

public class PlayerAttackHitbox : MonoBehaviour
{

    [SerializeField] protected float force;
    protected Collider col;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void OnTriggerEnter(Collider other)
    {
        // when the bat hits the tv it sets it to the broken image and stops playing the ads
        TVSwitcher tv = other.GetComponentInChildren<TVSwitcher>();

        if(tv != null)
        {
            tv.BreakTV();
            return;
        }

        if (other.CompareTag("Enemy"))
        {
            other.GetComponent<BaseEnemy>().currentState = BaseEnemy.EnemyState.Death;
        }

        
        
    }
}
