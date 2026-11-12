using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class TweakManager : MonoBehaviour
{
    public static TweakManager instance {get; private set;}
    public bool doubleViewSpeed;
    public bool alwaysPoison;
    
    Volume poisonVolume;
    public void Awake()
    {
        //Singleton design patttern
        if(instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;

        DontDestroyOnLoad(gameObject);
    }
    public void StartTweakManager()
    {
        poisonVolume = GameObject.FindGameObjectWithTag("PoisonVolume").GetComponent<Volume>();
        if (poisonVolume!=null)
        {
            Debug.Log("Poison Volume true");
            poisonVolume.enabled=true;
        }
    }
}
