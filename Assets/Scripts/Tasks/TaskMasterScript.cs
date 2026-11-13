using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class TaskMasterScript : MonoBehaviour
{
    public static TaskMasterScript instance { get; private set; }
    public List<TaskTemplate> availableTasks = new List<TaskTemplate>(); //All of the tasks needed to be completed, where 0 is always the exit task
    public TaskTemplate endTask; //Should be set to the end task
    public TextMeshProUGUI descDisplay; //The description of the current task
    public ViewTracker viewTracker; //The viewtracker
    public int objectsNeededForTask; //How many objects are needed to complete the collection task
    public int objectsCollected; //The current number of collected objects
    public int enemiesLeftAlive; //How many enemies must be killed to complete the task
    public int enemiesKilled; //The current number of enemies killed
    public int grafittiSpots; //How many enemies must be killed to complete the task
    public int grafittiCompleted; //The current number of enemies killed
    public TaskTemplate[] asylumTasks;
    public TaskTemplate[] subwayTasks;
    public void SetTasks() //Change the task and task description
    {
        descDisplay.text="";
        foreach (TaskTemplate task in availableTasks)
        {
            descDisplay.text+=task.TaskDescription;
            if(task.TaskID==4){descDisplay.text+="\nTrash Collected: "+objectsCollected+" / "+objectsNeededForTask;}
            if(task.TaskID==5){descDisplay.text+="\nRats Killed: "+enemiesKilled+" / "+enemiesLeftAlive;}
            if(task.TaskID==3){descDisplay.text+="\nGrafitti Spots Completed: "+grafittiCompleted+" / "+grafittiSpots;}
            descDisplay.text+="\n\n";
        }
        
    }
    public void EndAllTasks()
    {
        
        availableTasks.Clear();
        
    }

    public void ApplyAsylumTasks()
    {
        availableTasks.AddRange(asylumTasks);
    }
    public void ApplySubwayTasks()
    {
        availableTasks.AddRange(subwayTasks);
    }
    public void Awake()
    {
        //Singleton design patttern
        if(instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;

        DontDestroyOnLoad(gameObject);
    }

    public void StartTaskMasterScript()
    {
        
        descDisplay=GameObject.FindGameObjectWithTag("DescDisplay").GetComponent<TextMeshProUGUI>();
        viewTracker = GetComponent<ViewTracker>();
        objectsCollected=0;grafittiCompleted=0;enemiesKilled=0;
        SetTasks();
    }
    private void OnEnable()
    {
        GameController.OnEnemyKilled += CountEnemiesKilled;

    }
    private void OnDisable()
    {
        GameController.OnEnemyKilled -= CountEnemiesKilled;
        
    }

    void AddTask(TaskTemplate task)
    {
        availableTasks.Add(task);
        SetTasks();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    

    // Update is called once per frame
    [ContextMenu(nameof(EndTask))]
    public void EndTask(int taskID) //Switch to next task, and pay out
    {
        TaskTemplate endedTask=null;
        foreach (TaskTemplate task in availableTasks)
        {
            if(task.TaskID == taskID){endedTask=task;break;}
        }
        if(endedTask==null){return;}
        availableTasks.Remove(endedTask);
        if(0==availableTasks.Count)
        {
            //Put exit spawn here
            Debug.Log("All Tasks Complete!");
            AddTask(endTask);
        }
        // viewTracker.baseOffset+=endedTask.TaskViewGain;
        GameController.instance.IncreaseViewerRetention(GameController.ViewerRetentionCategories.TASK, endedTask.TaskViewGain);
        SetTasks();
    }
    

    public void CollectObject() //Increment object and end task if complete
    {
        objectsCollected++;
        SetTasks();
        if (objectsCollected >= objectsNeededForTask)
        {
            EndTask(4);
        }
    }
    public void CompleteGrafitti() //Increment object and end task if complete
    {
        grafittiCompleted++;
        SetTasks();
        if (grafittiCompleted >= grafittiSpots)
        {
            EndTask(3);
        }
    }

    public void CountEnemiesKilled() //Increment object and end task if complete
    {
        
        enemiesKilled++;
        SetTasks();
        if (enemiesKilled >= enemiesLeftAlive)
        {
            EndTask(5);
        }
    }
}
