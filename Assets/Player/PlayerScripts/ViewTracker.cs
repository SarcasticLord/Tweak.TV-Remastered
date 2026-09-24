using System;
using System.Collections;
using UnityEngine;

public class ViewTracker : MonoBehaviour
{
    public bool streamSniperSpawned;
    private static WaitForSeconds _waitForSeconds_01 = new WaitForSeconds(.1f);
    public double baseNumber=-50; //increases over time
    public double baseModifier=2; //When things happen on stream, edit this
    public double displayNumber; //show this
    public double baseOffset=2000; //Raises the end number
    public double slowingFactor=20; //Makes the process take longer

    public GameObject streamSniper;
    public Transform streamSniperSpawnpoint;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(nameof(VCOT));
    }

    IEnumerator VCOT() //Views Change Over Time
    {
        while (!streamSniperSpawned)
        {
            baseNumber++;
            displayNumber = (-Math.Pow(baseNumber*baseModifier, 2)/slowingFactor)+baseOffset;
            if(displayNumber<=0){SpawnStreamSniper();}
            yield return _waitForSeconds_01;
        }
        
    }

    public void SpawnStreamSniper()
    {
        Instantiate(streamSniper, streamSniperSpawnpoint.position, streamSniper.transform.rotation);
        streamSniperSpawned=true;
    }
    [ContextMenu("Increase Views By 10000")]
    public void IncreaseViewCountByThousand()
    {
        IncreaseViewCount(10000);
    }
    private void IncreaseViewCount(int a)
    {
        baseOffset+=a;
    }
}
