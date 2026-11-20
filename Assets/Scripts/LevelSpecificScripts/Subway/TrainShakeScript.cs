using UnityEngine;
using Unity.Cinemachine;
using System.Collections;
public class TrainShakeScript : MonoBehaviour
{
    [SerializeField] private CinemachineImpulseSource impulseSource;
    [SerializeField] private float shakeCooldown = 0.25f;

    void Start()
    {
        StartCoroutine(nameof(RepeatedlyInvokeShake));
    }
    
    IEnumerator RepeatedlyInvokeShake()
    {
        while(true)
        {
            impulseSource.GenerateImpulse();
            yield return new WaitForSeconds(shakeCooldown);
        }

    }
}
