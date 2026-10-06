using UnityEngine;
using UnityEngine.AI;
using static UnityEditor.PlayerSettings;

public class FlyingEnemyOffset : MonoBehaviour
{
    public bool isFlying;
    public Transform pivot;
    public float hoverHeight;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        isFlying = true;
        pivot = gameObject.GetComponent<Transform>();
    }

    // Update is called once per frame
    private void LateUpdate()
    {
        {
            
        }
    }
}
