using System;
using UnityEngine;

public class AttackHitbox : MonoBehaviour
{
    public float duration;
    public string[] target;

    public bool groundDestroy = false;

    private void Start()
    {
        Destroy(gameObject, duration);
    }

    private void OnTriggerEnter(Collider other)
    {
        foreach (string tag in target)
        {
            if (other.gameObject.CompareTag(tag))
            {
                //Debug.Log(other.tag.ToString() + " attacked. Decreasing views...");
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
