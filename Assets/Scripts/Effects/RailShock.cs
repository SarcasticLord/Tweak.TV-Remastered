using System.Collections;
using UnityEngine;

public class RailShock : MonoBehaviour
{
    public PlayerStatusEffects effects;
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Entered");
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log(effects.storeDefaultSlideSpeed);
            Debug.Log("Player Detected");
            effects.OnSlowed();
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
        yield return new WaitForSeconds(effects.slowDuration);
        effects.OnRemoveSlowed();
    }
}
