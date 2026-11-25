using UnityEngine;
using UnityEngine.UI;

public class PlayerProximityScript : MonoBehaviour
{
    //Handle player proximity to streamsniper for static visual effect
    [SerializeField] private RawImage staticRawImage;
    private float minDistance = 0f;
    private float maxDistance = 50f;    
    
    [Header("Alpha Settings")]
    [Range(0f,1f)]
    [SerializeField] private float maxAlpha = 0.05f; 
    public static Transform nearSniperTransform = null;

    
    

    void Update()
    {
        if(nearSniperTransform == null)return;
        
        
        float distance = Vector3.Distance(transform.position, nearSniperTransform.position);

        if(distance < maxDistance)
        {
            float alpha = Mathf.InverseLerp(maxDistance, minDistance , distance) * maxAlpha;

            Color currentColor = staticRawImage.color;
            currentColor.a = alpha;
            staticRawImage.color = currentColor;
        }

        
    }

    public void StreamSniperInProximity(Transform nearestSniper)
    {
        Debug.Log("StreamSniper in Proximity" + nearestSniper);
        nearSniperTransform = nearestSniper;
    }

    public void ExitedStreamSniperProximity()
    {
        nearSniperTransform = null;
        Color currentColor = staticRawImage.color;
        currentColor.a = 0f;
        staticRawImage.color = currentColor;
    }
}
