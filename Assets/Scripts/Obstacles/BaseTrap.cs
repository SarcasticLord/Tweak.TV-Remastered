using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class BaseTrap : MonoBehaviour
{
    [SerializeField] protected float duration;
    [SerializeField] protected GameObject effect;

    protected bool trapActive = false;

    [SerializeField] protected GameObject player;
    
    protected virtual void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Player"))
        {
            trapActive = true;
            player = col.gameObject;
            
        }
    }

    protected virtual void FixedUpdate()
    {
        PlayerStatusEffects effects = player.GetComponent<PlayerStatusEffects>();
        if (trapActive)
        {
            StartCoroutine(TrapPlayer());
            trapActive = false;
        }
    }

    protected virtual IEnumerator TrapPlayer()
    {
        player.GetComponent<CharacterController>().enabled = false;
        yield return new WaitForSeconds(duration);
        player.GetComponent<CharacterController>().enabled = true;
    }
}
