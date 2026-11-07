using System.Collections;
using UnityEngine;

public class RailShock : MonoBehaviour
{
    [SerializeField] private PlayerStatusEffects _effects;

    private PlayerStatusEffectTracker _tracker;

    private void Awake()
    {
        _tracker = GameObject.FindWithTag("Player").GetComponent<PlayerStatusEffectTracker>();
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Entered");
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log(_effects.storeDefaultSlideSpeed);
            Debug.Log("Player Detected");
            _effects.OnSlowed();
            _tracker.AddToStatus(PlayerStatusEffectTracker.StatusEffect.SHOCKED, 100);
            StopCoroutine("ExitSlowed");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("Player Exit");
            StartCoroutine("ExitSlowed");
        }
    }

    private IEnumerator ExitSlowed()
    {
        yield return new WaitForSeconds(_effects.slowDuration);
        _effects.OnRemoveSlowed();
    }
}
