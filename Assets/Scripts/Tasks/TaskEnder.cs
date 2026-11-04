using UnityEngine;

/// <summary>
/// Handles ending or updating specific tasks based on triggers, pickups, graffiti, or enemy destruction.
/// </summary>
public class TaskEnder : MonoBehaviour
{
    [Header("General")]
    
    
    [Tooltip("Determines if this object handles collection behavior.")]
    public bool isCollector;

    [Header("Pickup Variables")]
    [Tooltip("Is this object associated with a pickup task?")]
    public bool isPickup;
    public TaskTemplate taskToBeEnded;
    
    public PickupAndCarry pickupAndCarry;

    [Header("Grafitti Variables")]
    [Tooltip("Is this object associated with a graffiti task?")]
    public bool isGrafitti;
    public MeshRenderer grafittiImage;
    public bool canGrafitti;
    
    public GrafittiChallenge grafittiChallenge;

    [Header("Enemy Variables")]
    [Tooltip("Should this object track enemy deaths upon destruction?")]
    public bool trackEnemyDeath;

    void Start()
    {
        // Automatically cache the PickupAndCarry component if this is a pickup object
        if (isPickup)
        {
            pickupAndCarry = GetComponent<PickupAndCarry>();
        }

        // Find and cache the TaskMasterScript via the "ViewTracker" tag
        
    }

    /// <summary>
    /// Trigger-based logic for interacting with KeyItems (e.g., reaching an end zone or collecting items).
    /// </summary>
    void OnTriggerEnter(Collider other)
    {
        // If this object is designated for pickups or graffiti, ignore standard trigger volume end logic here
        if (isPickup || isGrafitti)
        {
            return;
        }

        // Check if the colliding object is tagged as a KeyItem
        if (other.gameObject.CompareTag("KeyItem"))
        {
            // TaskID 2: Reach a destination/trigger with a key item to end the task (if not a collector)
            if (!isCollector)
            {
                TaskMasterScript.instance.EndTask(2);
            }
            // TaskID 4: Collect the object, update progress, and destroy the item in the world
            
            TaskMasterScript.instance.CollectObject();
            Destroy(other.gameObject);
            
        }
    }

    /// <summary>
    /// Called when the GameObject is destroyed. Used here to track enemy elimination tasks.
    /// </summary>
    

    /// <summary>
    /// FixedUpdate is used for physics-synced checks, such as checking if a carried pickup has reached its goal.
    /// </summary>
    void FixedUpdate()
    {
        // Skip if this isn't a pickup object
        if (!isPickup || pickupAndCarry == null)
        {
            return;
        }
        
        // TaskID 1, 6 & 7: End the task and destroy this object once it is successfully carried
        if (pickupAndCarry.isCarried)
        {
            TaskMasterScript.instance.EndTask(taskToBeEnded.TaskID);
            Destroy(gameObject);
        }
    }
}