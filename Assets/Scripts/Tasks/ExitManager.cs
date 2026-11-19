using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitManager : MonoBehaviour
{
    public GameObject exitTrain;
    public Animator exitDoor;


    private void OnEnable()
    {
        TaskMasterScript.AllTasksComplete += SpawnExit; 
    }

    private void OnDisable()
    {
        TaskMasterScript.AllTasksComplete -= SpawnExit;

    }

    private void Awake()
    {
        exitDoor.enabled = false;
        exitTrain.SetActive(false);
    }

    public void SpawnExit()
    {
        if (exitTrain)
            exitTrain.SetActive(true);
        if(exitDoor)
            exitDoor.enabled = true;
    }


}
