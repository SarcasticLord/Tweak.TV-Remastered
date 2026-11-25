using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{

    public Transform EnemyPoint;
    public GameObject Rat; 
    public GameObject Bee;
    public GameObject EnemyExists;
    

    public bool facingRight = true;

    //public int enemyCount = 0;
    //public int maxEnemies = 10;

    public int spawnGap = 5;

    private bool isSpawning = false;
    private int nextEnemy = 0; // alternates between spawning the bee and the rat

    private Coroutine EnemyCoroutine;

    void Start()
    {
        StartCoroutine(EnemySpawn());
    }

    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other) // when the player walks into the circle itll spawn an enemy
    {
        if (other.CompareTag("Player"))
        {
            isSpawning = true;

        } 
        //Debug.Log("starting");
    }

    void OnTriggerExit(Collider other) // when you leave the circle it stops spawning
    {
        if (other.CompareTag("Player"))
        {
            isSpawning = false;
            
            
        }
        //Debug.Log("stopping");
    }


    

    IEnumerator EnemySpawn() // this spawns them once then will spawn more after x seconds
    {
        while (true)
        {
            for (int i = 0; i < TweakManager.instance.enemiesToSpawn; i++)
            {
                if (isSpawning && GameController.instance.increaseCurrEnemyCount())
                {
                    if(nextEnemy == 0)
                    {
                        EnemyExists = Instantiate(Rat, EnemyPoint.position, facingRight ? EnemyPoint.rotation : Quaternion.Euler(-90, 0, 0));
                        nextEnemy = 1;
                    }
                    else
                    {
                        EnemyExists = Instantiate(Bee, EnemyPoint.position, facingRight ? EnemyPoint.rotation : Quaternion.Euler(-90, 0, 0));
                        nextEnemy = 0;
                    }
                    
                }
            }
            
            yield return new WaitForSeconds(spawnGap);  
        } 
    }
}
