using UnityEngine;

public class LevelStarter : MonoBehaviour
{
    void Start()
    {
        GameController.instance.StartGame();
    }
}
