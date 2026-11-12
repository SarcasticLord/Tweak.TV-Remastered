using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

public class GameController : MonoBehaviour
{
    //GameController Script by Christian Andrion

    public static GameController instance { get; private set; }
    public static event Action<int> OnViewCountChanged;
    public static event Action<float> OnViewerRetentionChanged;
    public static event Action OnEnemyKilled;

    //Enemy related stuff
    [Header("Enemy Count")] 
    #region 
    public int currEnemyCount = 0;
    public int maxEnemyCount = 20;
    public BaseEnemy.EnemyType lastEnemyKilled;
    #endregion

    [Header("Viewer Count")]
    #region 
    [SerializeField] private int _viewCount;
    public int viewCount
    {
        get { return _viewCount; }
        set 
        {
            _viewCount = Mathf.Clamp(value,0,999999);
            OnViewCountChanged?.Invoke(_viewCount); //Update views on the GUI
        }
    }
    [SerializeField] private float _maxViewerRetention = 5f;
    [SerializeField] private float _minViewerRetention = -3f; 
    [SerializeField] private float _viewerRetention = 1f; //Weighted value that adds or decreases viewers over time
    public float viewerRetention
    {
        get {return _viewerRetention;}
        set
        {
            _viewerRetention = Mathf.Clamp(value, _minViewerRetention, _maxViewerRetention);
            OnViewerRetentionChanged?.Invoke(_viewerRetention);
        }
    }

    public enum ViewerRetentionCategories {TASK, SHMOVEMENT, VIOLENCE, RISKY}

    [SerializeField] private float baseShmovementMult = 2f;
    private float shmovementMult;
    [SerializeField] private float baseViolenceMult = 2f;
    private float violenceMult;
    [SerializeField] private float baseRiskyMult = 2f;
    private float riskyMult;

    private float viewChangeTime;
    #endregion

    [Header("Stream Sniper")]
    #region 
    [SerializeField] private GameObject streamSniper;
    private Transform streamSniperSpawnpoint;
    [SerializeField] private float streamSniperCooldown = 60f;
    [SerializeField] private float streamSniperSpawnTimer = 0f;
    [SerializeField] private float streamSniperSpawnChance = 0f;
    [SerializeField] private float currentStreamSniperSpawnChance;
    #endregion

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
        StopAllCoroutines(); //For safety
        streamSniperSpawnpoint = GameObject.Find("StreamSniperSpawn").transform;
        gameStarted = true;
        //Viewer Retention Base stats
        viewerRetention = 1f;
        shmovementMult = baseShmovementMult;
        violenceMult = baseViolenceMult;
        riskyMult = baseRiskyMult;
        //Reset enemy count
        currEnemyCount = 0;
        StartCoroutine(nameof(ChangeViewerRetention));
        currentStreamSniperSpawnChance = UnityEngine.Random.Range(0f, 1.0f);
        StartCoroutine(nameof(WeightedChanceToSpawnSniper));
        TaskMasterScript.instance.StartTaskMasterScript();
        // ViewTracker.instance.StartViewTracker();
        GrafittiChallenge.instance.StartGrafittiChallenge();
        TweakManager.instance.StartTweakManager();
    }

    public void StopGame()
    {
        gameStarted = false;
        StopAllCoroutines();
        //TODO: save values for stats screen
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
    public void decreaseCurrEnemyCount(BaseEnemy.EnemyType killedEnemyType) 
    {
        lastEnemyKilled = killedEnemyType;
        if(currEnemyCount > 0)
        {
            currEnemyCount--;
            OnEnemyKilled?.Invoke();
        }
    }

    public void ChangeViews()
    {
        float newRetention = UnityEngine.Random.Range(0,1f); //Random roll if your gonna lose or gain a viewer
        if(newRetention < viewerRetention)
        {
            viewCount += (int)MathF.Ceiling(viewerRetention);    
        }
        else
        {
            float viewDelta = viewerRetention > 0 ? -MathF.Ceiling(viewerRetention) : MathF.Floor(viewerRetention);
            //If viewer retention is between 0 and 1 decrease by 1
            //Else if viewer retention is in the negatives decreases by that negative value
            viewCount += (int)viewerRetention;
        }
        
        
        if (InGameUIController.gameTime <= 60) return; //Prevent streamsniper from spawning before a minute passes

        if (viewCount <= 0 && streamSniperSpawnTimer < streamSniperCooldown/2)
        {
            SpawnStreamSniper();
        }
    }

    //Called in other scripts to increase viewer retention
    public void IncreaseViewerRetention(ViewerRetentionCategories category, float increaseViewerRetention)
    {
        switch(category)
        {
            case ViewerRetentionCategories.TASK:
                viewerRetention += increaseViewerRetention; 
                break;
            case ViewerRetentionCategories.SHMOVEMENT:
                viewerRetention += increaseViewerRetention * shmovementMult;
                shmovementMult = Mathf.Clamp(shmovementMult - 0.01f, 0, baseShmovementMult); //Lower multiplier every time we increase retention
                break;
            case ViewerRetentionCategories.VIOLENCE:
                viewerRetention += increaseViewerRetention * violenceMult;
                violenceMult = Mathf.Clamp(violenceMult - 0.01f,0,baseViolenceMult);
                break;
            case ViewerRetentionCategories.RISKY:
                viewerRetention += increaseViewerRetention * riskyMult;
                riskyMult = Mathf.Clamp(riskyMult - 0.05f, 0, baseRiskyMult);
                break;
            default:
                UnityEngine.Debug.Log("Invalid viewer retention category");
                break;
        }

    }

    public void DecreaseViewerRetention(float decreaseViewerRetention = 0)
    {
        if (InGameUIController.gameTime <= 15) return;// 15 second grace period

        if (decreaseViewerRetention == 0)//Natural Viewer retention decrease rate
            viewerRetention -= 0.01f;
        else
        {
            //Allow other scripts to choose a value to decrease view count by
            viewerRetention -= decreaseViewerRetention; 
        }
    }

    //Looping coroutine that has the view increase and view decrease rates fight
    IEnumerator ChangeViewerRetention()
    {
        while (gameStarted)
        {
            viewChangeTime = UnityEngine.Random.Range(1f, 3f);
            ChangeViews();
            DecreaseViewerRetention();
            yield return new WaitForSeconds(viewChangeTime);
        }
    }

    private IEnumerator WeightedChanceToSpawnSniper()
    {
        WaitForSeconds waitTime = new WaitForSeconds(1f);

        while(gameStarted)
        {
            
            if(streamSniperSpawnTimer < streamSniperCooldown)//So we dont spawn a million streamsnipers at once
            {
                streamSniperSpawnTimer += 1f;
                yield return waitTime;
                continue; 
            } 

            if(streamSniperSpawnChance < 1f) //if streamSniperSpawnChance = 1, 100% chance to spawn StreamSniper
            {
                streamSniperSpawnChance += 0.01f;
            }

            if(currentStreamSniperSpawnChance < streamSniperSpawnChance)
            {
                SpawnStreamSniper();
            }

            yield return waitTime;
        }
    }

    public void SpawnStreamSniper()
    {
        Instantiate(streamSniper, streamSniperSpawnpoint.position, streamSniper.transform.rotation);
        streamSniperSpawnTimer = 0f;//Reset stream sniper spawn timer
        streamSniperSpawnChance = 0f;//Reset stream sniper spawn chance
        currentStreamSniperSpawnChance = UnityEngine.Random.Range(0f, 1.0f);
    }

}
