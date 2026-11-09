using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;

public class PoisonEffect : MonoBehaviour
{

    public Volume volume;
    
    public float intensitySpeed; //The speed at which the screen oscillates. A multiplier
    public float intensitySpeedSet; //The default speed
    public float intensitySpeedDoubled; //The doubled speed when the player is poisoned (by the PlayerStatusEffect script)
    private VolumeProfile runtimeProfile;
    void Start()
    {
        volume = GetComponent<Volume>();
        runtimeProfile = Instantiate(volume.sharedProfile);
        volume.profile = runtimeProfile; 
        intensitySpeed=intensitySpeedSet;
        StartCoroutine(nameof(UpdateLensDistortion));
        
    }

    IEnumerator UpdateLensDistortion()
    {
        if (!runtimeProfile.TryGet<UnityEngine.Rendering.Universal.LensDistortion>(out var lensDistortion))
        {
            yield break;
        }

        float currentSpeed = 0.2f; // Much slower transition speed (adjust as needed)
        float targetValue = Random.Range(-1f, 1f);

        while (true)
        {
            // Smoothly move towards the current target value
            lensDistortion.intensity.value = Mathf.MoveTowards(
                lensDistortion.intensity.value, 
                targetValue, 
                currentSpeed * Time.deltaTime
            );

            // Once we are very close to the target, pick a new random intermediate point
            // between 0 and the outer bounds (-1 or 1)
            if (Mathf.Abs(lensDistortion.intensity.value - targetValue) < 0.01f)
            {
                // Randomly decide whether to target the negative or positive side
                float outerBound = Random.value > 0.5f ? 1f : -1f;
                
                // Pick a random point between 0 and that outer bound
                targetValue = Random.Range(0f, outerBound);
                
                // Randomize speed slightly for organic movement
                currentSpeed = Random.Range(0.1f, 0.4f);
            }

            yield return null; // Update every frame smoothly without lag spikes
        }
    }
    
    
    public void DoubleSpeed()
    {
        intensitySpeed=intensitySpeedDoubled;
    }
    public void NormalSpeed()
    {
        intensitySpeed=intensitySpeedSet;
    }
    
    
}
