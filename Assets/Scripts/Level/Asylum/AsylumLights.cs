// this turns the lights on when the player is near
// it also swaps the material on the light to match its state 

using System.Runtime.CompilerServices;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AsylumLights : MonoBehaviour
{
    [SerializeField] private Light light;
    [SerializeField] private Renderer glowing;
    [SerializeField] private Material glowingMaterial;
    [SerializeField] private int materialSlot = 2;      // this is a workaround to get the material to be in the proper place 


    [SerializeField] private Color emissionColor = Color.white;
    [SerializeField] private float offLight = 0f;
    [SerializeField] private float onLight = 3f;
    [SerializeField] private bool turnOffOnExit;

    [Header("Flicker Settings")] // this is the stuff that flickers the lights when theyre on   // they are set public so you should be able to access them
    [SerializeField] public bool flicker = true;
    [SerializeField] public float minFlickerTime = 0.05f;
    [SerializeField] public float maxFlickerTime = 0.4f;
    [SerializeField] public float flickerChance = 0.6f;

    [SerializeField] private bool foreverLight;

    private Material glowMat;
    private bool isOn;
    private Coroutine flickerCo;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (light == null)
        {
            light = GetComponentInChildren<Light>();
        }

        if (glowing == null)
        {
            glowing = GetComponentInChildren<Renderer>();
        }

        if (glowing != null)
        {
            Material[] mats = glowing.materials;
            if (materialSlot >= 0 && materialSlot < mats.Length)
            {
                if (glowingMaterial != null)
                {
                    mats[materialSlot] = glowingMaterial;
                    glowing.materials = mats;
                    mats = glowing.materials;
                }
            }
            

            glowMat = mats[materialSlot];
            glowMat.EnableKeyword("_EMISSION");
        }

        SetLight(foreverLight);
    }

    void OnTriggerEnter(Collider other) // when the player enters the collider
    {
        if(other.CompareTag("Player") && !foreverLight)
        {
            SetLight(true);
        }
    }

    void OnTriggerExit(Collider other) // when the player leaves the collider
    {
        if (turnOffOnExit && other.CompareTag("Player") && !foreverLight)
        {
            SetLight(false);
        }
    }

    private void SetLight(bool on)
    {

        isOn = on;
        if (flickerCo != null)
        {
            StopCoroutine(flickerCo);
            flickerCo = null;
        }
        if (light != null)
        {
            light.enabled = on;
            light.intensity = on ? onLight : offLight;
        }

        UpdateGlow(on ? onLight : offLight);

        if (on && flicker)
        {
            flickerCo = StartCoroutine(FlickerLight());
        }

        
    }
    
    private IEnumerator FlickerLight()
    {
        while (isOn)
        {
            float waitTime = Random.Range(minFlickerTime, maxFlickerTime);
            yield return new WaitForSeconds(waitTime);

            if(Random.value < flickerChance)
            {
                float randomIntensity = Random.Range(0f, onLight);

                if(light != null)
                {
                    light.intensity = randomIntensity;
                }

                UpdateGlow(randomIntensity);

                yield return new WaitForSeconds(Random.Range(0.02f, 0.1f));

                if (isOn)
                {
                    if (light != null)
                    {
                        light.intensity = onLight;
                    }

                    UpdateGlow(onLight);
                }
            }
        }
    }
    private void UpdateGlow(float intensity)
    {
        if (glowMat != null)
        {
            glowMat.SetColor("_EmissionColor", emissionColor * intensity);
        }
    }
        
}

