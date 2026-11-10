using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class BearTrap : BaseTrap
{
    public GameObject closedTrap;
    public GameObject trapEffect; 

    private bool spawnedClosedTrap = false;


    protected override void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Player"))
        {
            trapActive = true;
            player = col.gameObject;
            if (!spawnedClosedTrap)
            {
                GameObject trap = Instantiate(closedTrap, gameObject.transform.position, gameObject.transform.rotation);
                spawnedClosedTrap = true;
                Destroy(trap, duration + 1f);
                Destroy(this.gameObject, duration + 1f);
            }
            gameObject.GetComponent<MeshRenderer>().enabled = false;
        }
    }
    protected override void FixedUpdate()
    {
        PlayerStatusEffects effects = player.GetComponent<PlayerStatusEffects>();
        if (trapActive)
        {
            StartCoroutine(TrapPlayer());
            effects.trappedEffect = trapEffect;
            trapActive = false;
        }
    }

}
