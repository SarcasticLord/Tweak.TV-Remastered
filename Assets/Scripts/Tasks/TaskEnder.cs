using System.Threading.Tasks;
using UnityEngine;

public class TaskEnder : MonoBehaviour
{
    [Header("General")]
    public TaskMasterScript taskMaster;
    public bool isCollector;

    [Header("Pickup Variables")]
    public bool isPickup;
    public PickupAndCarry pickupAndCarry;

    void Start()
    {
        if(isPickup){pickupAndCarry=GetComponent<PickupAndCarry>();}
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnTriggerEnter(Collider other)
    {
        if (isPickup)
        {
            return;
        }
        if (other.gameObject.CompareTag("KeyItem")) //If key item found, either collect or end task
        {
            if (!isCollector && taskMaster.taskCursor==2)
            {
                taskMaster.EndTask();
            }
            else if(taskMaster.taskCursor==4)
            {
                taskMaster.CollectObject();
                Destroy(other.gameObject);
            }
            
        }
    }

    void FixedUpdate()
    {
        if(!isPickup){return;}
        if (pickupAndCarry.isCarried && taskMaster.taskCursor==1)
        {
            taskMaster.EndTask();
            Destroy(gameObject);
        }
    }
}
