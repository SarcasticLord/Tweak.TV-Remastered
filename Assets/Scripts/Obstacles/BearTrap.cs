using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class BearTrap : BaseTrap
{
    public GameObject closedTrap;
    public GameObject trapEffect; 

    private PlayerStatusEffects _effects;
    private PlayerStatusEffectTracker _playerStatus;

    private bool spawnedClosedTrap = false;


    protected override void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Player"))
        {
            trapActive = true;
            player = col.gameObject;
            _effects = player.GetComponent<PlayerStatusEffects>();
            _playerStatus = player.GetComponent<PlayerStatusEffectTracker>();
            if (!spawnedClosedTrap)
            {
                StartCoroutine(TrapPlayer());
                _playerStatus.AddToStatus(PlayerStatusEffectTracker.StatusEffect.TRAPPED, 100f);
                _effects.trappedEffect = trapEffect;
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

    }

}
