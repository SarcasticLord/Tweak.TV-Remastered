using System;
using UnityEngine;

public class AttackHitbox : MonoBehaviour
{
    [SerializeField] protected float duration;
    [SerializeField] protected string[] target;

    [SerializeField] protected bool groundDestroy = false;

    protected virtual void Start()
    {
        Destroy(gameObject, duration);
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        foreach (string tag in target)
        {
            if (other.gameObject.CompareTag(tag))
            {
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
