using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitManager : MonoBehaviour
{
    public GameObject exit;

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
        exit.SetActive(false);
    }

    public void SpawnExit()
    {
        Debug.Log("Exit is Active");
        exit.SetActive(true);
    }

}
