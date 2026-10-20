using UnityEngine;
[CreateAssetMenu(fileName = "NewTask", menuName = "Tasks/NewTask")]
public class TaskTemplate : ScriptableObject
{
    [TextArea]
    public string TaskDescription;
    public int TaskID;
    public double TaskViewGain;
}
