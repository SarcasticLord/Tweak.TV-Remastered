using System.Runtime.CompilerServices;
using UnityEngine;

public class AsylumLights : MonoBehaviour
{
    [SerializeField] private Light light;
    [SerializeField] private Renderer glowing;
    [SerializeField] private Material glowingMaterial;
    [SerializeField] private int materialSlot = 2;


    [SerializeField] private Color emissionColor = Color.white;
    [SerializeField] private float offLight = 0f;
    [SerializeField] private float onLight = 3f;
    [SerializeField] private bool turnOffOnExit;

    private Material glowMat;

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

        SetLight(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            SetLight(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (turnOffOnExit && other.CompareTag("Player"))
        {
            SetLight(false);
        }
    }

    private void SetLight(bool on)
    {
        if (light != null)
        {
            light.enabled = on;
        }

        if (glowMat != null)
        {
            float intenseity = on ? onLight : offLight;
            glowMat.SetColor("_EmissionColor", emissionColor * intenseity);
        }
    }
}
