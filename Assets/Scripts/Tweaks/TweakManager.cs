using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class TweakManager : MonoBehaviour
{
    public static TweakManager instance {get; private set;}
    [Header("Tweak Toggles")]
    public bool doubleViewSpeed;
    public bool alwaysPoison;
    public bool moreBearTraps; //Still need to add to subway
    public bool moreEnemies; 
    public bool streamSniperImmediately; //TODO
    public bool deactivateLights; 

    [Space(3)]
    [Header("Assisting assets")]
    Volume poisonVolume;
    public Material lightDead;
    public GameObject[] allLights;
    public GameObject[] allBonusSpawnAreas;
    public GameObject[] allBonusTrapAreas;
    public GameObject bearTrap;
    public int enemiesToSpawn = 1;

    
    

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
    public void FlipspawnSSImm(bool isOn)
    {
        streamSniperImmediately=isOn;
    }
    public void FlipfastViewDecline(bool isOn)
    {
        doubleViewSpeed=isOn;
    }
    public void FlipmoreTraps(bool isOn)
    {
        moreBearTraps=isOn;
    }
    public void FlipallLightsOff(bool isOn)
    {
        deactivateLights=isOn;
    }
    public void FlipcameraFilters(bool isOn)
    {
        alwaysPoison=isOn;
    }
    public void FlipmoreEnemies(bool isOn)
    {
        moreEnemies=isOn;
    }
    public void StartTweakManager()
    {
        poisonVolume = GameObject.FindGameObjectWithTag("PoisonVolume").GetComponent<Volume>();
        
        if (poisonVolume!=null && alwaysPoison)
        {
            
            poisonVolume.enabled=true;
        }
        if (moreBearTraps)
        {
            allBonusSpawnAreas = GameObject.FindGameObjectsWithTag("BonusSpawns");
            foreach (GameObject spot in allBonusSpawnAreas)
            {
                Vector3 vector3 = new(spot.transform.eulerAngles.x-90, spot.transform.eulerAngles.y, spot.transform.eulerAngles.z);
                GameObject jeffery = Instantiate(bearTrap, spot.transform.position, Quaternion.Euler(vector3));
                
                
            }
        }
        if (moreEnemies)
        {
            enemiesToSpawn=2;
        }
        if (deactivateLights)
        {
            Material[] newMaterials = new Material[3];
            allLights = new GameObject[GameObject.FindGameObjectsWithTag("SubwayLight").Length];
            allLights = GameObject.FindGameObjectsWithTag("SubwayLight");
            for(int i = 0; i < 3; i++)
            {
                newMaterials[i] = lightDead;
            }

            foreach(GameObject light in allLights)
            {
                
                light.GetComponent<Renderer>().sharedMaterials = newMaterials;
                if (light.GetComponentInChildren<Light>()!=null)
                {
                    light.GetComponentInChildren<Light>().enabled = false;
                }
                
                if(light.GetComponent<AsylumLights>()!=null)
                {
                    light.GetComponent<AsylumLights>().enabled = false;
                }
                if (light.GetComponent<Collider>() != null)
                {
                    light.GetComponent<Collider>().enabled = false;
                }
                
                
                
            }
        }
    }
}
