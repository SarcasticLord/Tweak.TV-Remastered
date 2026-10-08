using UnityEngine;

public class TaskEnder : MonoBehaviour
{
    public TaskMasterScript taskMaster;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("KeyItem"))
        {
            taskMaster.EndTask();
        }
    }
}
