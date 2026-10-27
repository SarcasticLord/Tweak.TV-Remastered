using System.Collections;
using UnityEngine;

public class GameController : MonoBehaviour
{
    //Singleton Script

    public static GameController instance { get; private set; }

    public int currEnemyCount = 0;

    public int maxEnemyCount = 100;

    public int viewCount = 10;
    private float viewCountIncreaseRate = 1f; //Successful tasks and coolness increase this value
    private float viewCountDecreaseRate = 0f; //When damaged increase decrease rate

    private float viewChangeTime;

    public float gameTime = 0; 

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
        StartCoroutine("ChangeViews");
    }

    private void Update()
    {
        //Keep track of time
        gameTime += Time.deltaTime;
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
        if (decreaseViewsVal == 0)//Default value to allow the ChangeViews() coroutine to fight each other
            viewCount -= (int)(1 * viewCountDecreaseRate);
        else
        {
            //Allow other scripts to choose a value to decrease view count by
            viewCount -= decreaseViewsVal; 
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
            viewChangeTime = Random.Range(1, 3);
            viewCountDecreaseRate += 0.1f;
            IncreaseViews();
            DecreaseViews();
            yield return new WaitForSeconds(viewChangeTime);
        }
    }

}
