using UnityEngine.InputSystem;
using UnityEngine;
using System.Threading.Tasks;

public class TaskEnder : MonoBehaviour
{
    [Header("General")]
    public TaskMasterScript taskMaster;
    public bool isCollector;

    [Header("Pickup Variables")]
    public bool isPickup;
    public PickupAndCarry pickupAndCarry;
    [Header("Grafitti Variables")]
    public bool isGrafitti;
    public GrafittiChallenge grafittiChallenge;
    [Header("Enemy Variables")]
    public bool trackEnemyDeath;

    void Start()
    {
        if(isPickup){pickupAndCarry=GetComponent<PickupAndCarry>();}
        taskMaster = GameObject.FindGameObjectWithTag("ViewTracker").GetComponent<TaskMasterScript>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnTriggerEnter(Collider other)
    {
        if (isPickup || isGrafitti)
        {
            return;
        }
        if (other.gameObject.CompareTag("KeyItem")) //If key item found, either collect or end task
        {
            if (!isCollector && taskMaster.currentTask.TaskID==2)
            {
                taskMaster.EndTask();
            }
            else if(taskMaster.currentTask.TaskID==4)
            {
                taskMaster.CollectObject();
                Destroy(other.gameObject);
            }
            
        }
    }

    void OnDestroy()
    {
        if(taskMaster.currentTask.TaskID!=5){return;}
        taskMaster.CountEnemiesKilled();
    }

    void FixedUpdate()
    {
        if(!isPickup){return;}
        if (pickupAndCarry.isCarried && taskMaster.currentTask.TaskID==1)
        {
            taskMaster.EndTask();
            Destroy(gameObject);
        }
        
    }
}
