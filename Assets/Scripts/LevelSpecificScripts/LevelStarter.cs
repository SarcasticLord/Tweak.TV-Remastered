using UnityEngine;

public class LevelStarter : MonoBehaviour
{
    private ExitManager exitManager;
    private void Awake()
    {
        exitManager = gameObject.GetComponent<ExitManager>();
        if(exitManager)
            exitManager.gameObject.SetActive(true);
    }

    void Start()
    {
        GameController.instance.StartGame();
    }
}
