using UnityEngine;

public class TaskEnder : MonoBehaviour
{
    public TaskMasterScript taskMaster;
    public bool isCollector;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("KeyItem")) //If key item found, either collect or end task
        {
            if (!isCollector)
            {
                taskMaster.EndTask();
            }
            else
            {
                taskMaster.CollectObject();
            }
            
        }
    }
}
