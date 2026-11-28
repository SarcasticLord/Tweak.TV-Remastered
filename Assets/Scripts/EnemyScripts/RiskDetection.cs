using UnityEngine;

public class RiskDetection : MonoBehaviour
{
    [SerializeField] private float viewerRetention = 0.1f;
    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            GameController.instance.IncreaseViewerRetention(GameController.ViewerRetentionCategories.RISKY, viewerRetention);
        }
    }
}
