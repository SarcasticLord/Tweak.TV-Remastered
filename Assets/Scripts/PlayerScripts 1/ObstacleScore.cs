using UnityEngine;

public class ObstacleScore : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Obstacle"))
        {
            GameController.instance.IncreaseViewerRetention(GameController.ViewerRetentionCategories.SHMOVEMENT, 0.1f);

            Destroy(other);
        }
        
        
        
    }
}
