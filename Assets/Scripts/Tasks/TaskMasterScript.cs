using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class TaskMasterScript : MonoBehaviour
{
    public TaskTemplate currentTask;
    public TaskTemplate[] availableTasks;
    public int taskCursor = 1;
    public TextMeshProUGUI descDisplay;
    public ViewTracker viewTracker;

    void SetTask(int taskNumber)
    {
        currentTask=availableTasks[taskCursor];
        descDisplay.text=currentTask.TaskDescription;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SetTask(taskCursor);
    }

    // Update is called once per frame
    [ContextMenu(nameof(EndTask))]
    public void EndTask()
    {
        taskCursor++;
        if(taskCursor>=availableTasks.Length){taskCursor=0;}
        viewTracker.baseOffset+=currentTask.TaskViewGain;
        SetTask(taskCursor);
    }
}
