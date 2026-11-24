using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
//using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class TaskMasterScript : MonoBehaviour
{
    public static TaskMasterScript instance { get; private set; }
    public List<TaskTemplate> availableTasks = new List<TaskTemplate>(); //All of the tasks needed to be completed, where 0 is always the exit task
    public TaskTemplate endTask; //Should be set to the end task
    public TaskTemplate exitSubwayTask;
    public TaskTemplate exitAsylumTask; 
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
    public TaskTemplate escapeTask;
    public string level;
    public GameObject coffeeTaskItem;
    public GameObject pillTaskItem;
    public GameObject logTaskItem;
    public List<Transform> coffeeTaskLocation = new List<Transform>();
    public List<Transform> pillTaskLocation = new List<Transform>();
    public List<Transform> logTaskLocation = new List<Transform>();
    public AudioSource audioSource;

    //Exit delegate
    public delegate void OnAllTasksComplete();
    public static event OnAllTasksComplete AllTasksComplete;



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

    public void ShowEscapeTask()
    {
        descDisplay.text=escapeTask.TaskDescription;
    }

    public void EndEscapeTask()
    {
        SetTasks();
        PlaySuccessSound();
    }

    public bool checkForThisTask(TaskTemplate taskToCheck)
    {
        return availableTasks.Contains(taskToCheck);
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
        audioSource = GetComponent<AudioSource>();
    }

    public void StartTaskMasterScript()
    {
        
        descDisplay=GameObject.FindGameObjectWithTag("DescDisplay").GetComponent<TextMeshProUGUI>();
        viewTracker = GetComponent<ViewTracker>();
        objectsCollected=0;grafittiCompleted=0;enemiesKilled=0;
        SetTasks();
        RandomizeItemLocations();
    }

    void RandomizeItemLocations()
    {
        
        if (level == "asylum")
        {
            GameObject[] pillPoints = GameObject.FindGameObjectsWithTag("PillSpawns");
            for (int i = 0; i < pillPoints.Length; i++)
            {
                pillTaskLocation.Add(pillPoints[i].transform);
            }
            GameObject[] logPoints = GameObject.FindGameObjectsWithTag("LogSpawns");
            for (int i = 0; i < logPoints.Length; i++)
            {
                logTaskLocation.Add(logPoints[i].transform);
            }
            int pillIndex = UnityEngine.Random.Range(0, pillTaskLocation.Count);
            Transform pillLocationOfChoice = pillTaskLocation[pillIndex];
            int logIndex = UnityEngine.Random.Range(0, logTaskLocation.Count);
            Transform logLocationOfChoice = logTaskLocation[logIndex];
            Instantiate(pillTaskItem, pillLocationOfChoice.position, pillTaskItem.transform.rotation);
            Instantiate(logTaskItem, logLocationOfChoice.position, logTaskItem.transform.rotation);
        }
        else if (level == "subway")
        {
            Debug.Log("Addressed");
            GameObject[] coffeePoints = GameObject.FindGameObjectsWithTag("CoffeeSpawns");
            for (int i = 0; i < coffeePoints.Length; i++)
            {
                coffeeTaskLocation.Add(coffeePoints[i].transform);
            }
            int coffeeIndex = UnityEngine.Random.Range(0, coffeeTaskLocation.Count);
            Transform coffeeLocationOfChoice = coffeeTaskLocation[coffeeIndex];
            Instantiate(coffeeTaskItem, coffeeLocationOfChoice.position, coffeeTaskItem.transform.rotation);
        }
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

    void PlaySuccessSound()
    {
        audioSource.Play();
    }
    

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
        PlaySuccessSound();
        if(0==availableTasks.Count)
        {
            //Put exit spawn here
            AllTasksComplete?.Invoke();
            Debug.Log("All Tasks Complete!");
            

            if(level == "asylum")
            {
                AddTask(exitAsylumTask);
            }

            else if(level == "subway")
            {
                AddTask(exitSubwayTask);
            }
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

    public void CountEnemiesKilled(BaseEnemy.EnemyType enemyKilled) //Increment object and end task if complete
    {
        switch (enemyKilled)
        {
            case BaseEnemy.EnemyType.RAT:
                enemiesKilled++;
                SetTasks();
                if (enemiesKilled >= enemiesLeftAlive)
                {
                    EndTask(5);
                }
                break;
            case BaseEnemy.EnemyType.BEE:
                Debug.Log("Bee killed, we currently have no tasks related to them so uhhh, yeah");
                break;
            default:
                Debug.LogError("Invalid enemy type: " + enemyKilled);
                break;
        }

    }
}
