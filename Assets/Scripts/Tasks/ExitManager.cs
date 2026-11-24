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
        if (exitTrain)
            exitTrain.SetActive(false);
        if (exitDoor)
            exitDoor.GetComponent<Animator>().enabled = false ;
    }

    public void SpawnExit()
    {
        if (exitTrain)
            exitTrain.SetActive(true);
        if(exitDoor)
            exitDoor.GetComponent<Animator>().enabled = true;
    }


}
