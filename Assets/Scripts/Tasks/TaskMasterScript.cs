using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class TaskMasterScript : MonoBehaviour
{
    public TaskTemplate currentTask; //The current task needed to complete
    public TaskTemplate[] availableTasks; //All of the tasks needed to be completed, where 0 is always the exit task
    public int taskCursor = 1; //The current number for the task
    public TextMeshProUGUI descDisplay; //The description of the current task
    public ViewTracker viewTracker; //The viewtracker
    public int objectsNeededForTask; //How many objects are needed to complete the collection task
    public int objectsCollected; //The current number of collected objects
    public int enemiesLeftAlive; //How many enemies must be killed to complete the task
    public int enemiesKilled; //The current number of enemies killed

    void SetTask(int taskNumber) //Change the task and task description
    {
        currentTask=availableTasks[taskCursor];
        descDisplay.text=currentTask.TaskDescription;
        if(currentTask.TaskID==4){descDisplay.text=currentTask.TaskDescription+"\nTrash Collected: "+objectsCollected+" / "+objectsNeededForTask;}
        if(currentTask.TaskID==5){descDisplay.text=currentTask.TaskDescription+"\nRats Killed: "+enemiesKilled+" / "+enemiesLeftAlive;}
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SetTask(taskCursor);
    }

    // Update is called once per frame
    [ContextMenu(nameof(EndTask))]
    public void EndTask() //Switch to next task, and pay out
    {
        taskCursor++;
        if(taskCursor>=availableTasks.Length){taskCursor=0;}
        //viewTracker.baseOffset+=currentTask.TaskViewGain;
        GameController.instance.IncreaseViewIncreaseRate(currentTask.TaskViewGain);
        SetTask(taskCursor);
    }

    public void CollectObject() //Increment object and end task if complete
    {
        objectsCollected++;
        descDisplay.text=currentTask.TaskDescription+"\nTrash Collected: "+objectsCollected+" / "+objectsNeededForTask;
        if (objectsCollected >= objectsNeededForTask)
        {
            EndTask();
        }
    }

    public void CountEnemiesKilled() //Increment object and end task if complete
    {
        if(currentTask.TaskID!=5){return;}
        enemiesKilled++;
        descDisplay.text=currentTask.TaskDescription+"\nRats Killed: "+enemiesKilled+" / "+enemiesLeftAlive;
        if (enemiesKilled >= enemiesLeftAlive)
        {
            EndTask();
        }
    }
}
