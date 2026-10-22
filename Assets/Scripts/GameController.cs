using UnityEngine;

public class GameController : MonoBehaviour
{
    //Singleton Script

    public static GameController instance { get; private set; }

    public int currEnemyCount = 0;

    public int maxEnemyCount = 100;

    private int viewCount;

    public void Awake()
    {
        if(instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;

        DontDestroyOnLoad(gameObject);
    }

}
