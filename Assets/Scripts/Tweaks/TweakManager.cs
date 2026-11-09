using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class TweakManager : MonoBehaviour
{
    public bool doubleViewSpeed;
    public bool alwaysPoison;
    
    Volume poisonVolume;

    void Awake()
    {
        poisonVolume = GameObject.FindGameObjectWithTag("PoisonVolume").GetComponent<Volume>();
        if (poisonVolume!=null)
        {
            poisonVolume.enabled=true;
        }
    }
}
