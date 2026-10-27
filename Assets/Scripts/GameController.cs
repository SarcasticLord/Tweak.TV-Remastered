using System;
using System.Collections;
using UnityEngine;

public class GameController : MonoBehaviour
{
    //Singleton Script

    public static GameController instance { get; private set; }

    public static event Action<int> OnViewCountChanged;

    public int currEnemyCount = 0;

    public int maxEnemyCount = 100;

    public int _viewCount;

    public int viewCount
    {
        get { return _viewCount; }
        set 
        {
            _viewCount = Mathf.Clamp(value,0,999999);
            OnViewCountChanged?.Invoke(_viewCount);
        }

    }

    private float viewCountIncreaseRate = 1f; //Successful tasks and coolness increase this value

    //TODO; ViewCountIncreaseRateDepreciationRate

    private float viewCountDecreaseRate = 0f; //When damaged increase decrease rate

    private float viewChangeTime;

    [SerializeField] private GameObject streamSniper;
    private Transform streamSniperSpawnpoint;

    private bool streamSniperSpawned;
    private bool gameStarted = false;

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

    public void StartGame()
    {
        streamSniperSpawnpoint = GameObject.Find("StreamSniperSpawn").transform;
        gameStarted = true;
        streamSniperSpawned = false;
        viewCountDecreaseRate = 0f;
        viewCountIncreaseRate = 1f;
        currEnemyCount = 0;
        StartCoroutine("ChangeViews");
    }

    public void StopGame()
    {
        gameStarted = false;
        StopAllCoroutines();
    }

    //Called in EnemySpawner.cs script 
    public bool increaseCurrEnemyCount()
    {
        if (currEnemyCount < maxEnemyCount)
        {
            currEnemyCount++;
            return true;
        }
        return false;
    }

    //Called in BaseEnemy.cs
    public void decreaseCurrEnemyCount() 
    { 
        if(currEnemyCount > 0)
        {
            currEnemyCount--;
        }
    }

    public void IncreaseViews()
    {
        viewCount += (int)(1 * viewCountIncreaseRate);
    }

    public void IncreaseViewIncreaseRate(float viewIncreaseRate)
    {
        viewCountIncreaseRate += viewIncreaseRate;
    }

    public void DecreaseViews(int decreaseViewsVal = 0)
    {
        if (InGameUIController.gameTime <= 15) return;// 15 second grace period

        if (decreaseViewsVal == 0)//Default value to allow the ChangeViews() coroutine to fight each other
            viewCount -= (int)(1 * viewCountDecreaseRate);
        else
        {
            //Allow other scripts to choose a value to decrease view count by
            viewCount -= decreaseViewsVal; 
        }

        if (InGameUIController.gameTime <= 60) return; //Prevent streamsniper from spawning before a minute passes

        if (viewCount <= 0 && !streamSniperSpawned)
        {
            SpawnStreamSniper();
        }
    }

    public void IncreaseViewDecreaseRate(float viewDecreaseRate)
    {
        viewCountDecreaseRate += viewDecreaseRate;
    }

    //Looping coroutine that has the view increase and view decrease rates fight
    IEnumerator ChangeViews()
    {
        while (true)
        {
            viewChangeTime = UnityEngine.Random.Range(1, 3);
            viewCountDecreaseRate += 0.1f;
            IncreaseViews();
            DecreaseViews();
            yield return new WaitForSeconds(viewChangeTime);
        }
    }

    public void SpawnStreamSniper()
    {
        Instantiate(streamSniper, streamSniperSpawnpoint.position, streamSniper.transform.rotation);
        streamSniperSpawned = true;
    }

}
