using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{

    public Transform EnemyPoint;
    public GameObject Enemy; 
    public GameObject EnemyExists;

    public bool facingRight = true;

    public int enemyCount = 0;
    public int maxEnemies = 10;

    void Start()
    {
        //StartCoroutine(EnemySpawn());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            {
                StartCoroutine(EnemySpawn());
            }
        }
    }
    

    IEnumerator EnemySpawn() 
    {
        while (true)
        {
            if (enemyCount < maxEnemies)
            {
                EnemyExists = Instantiate(Enemy, EnemyPoint.position, facingRight ? EnemyPoint.rotation : Quaternion.Euler(-90, 0, 0));
                enemyCount++;
            
            }
            yield return new WaitForSeconds(500f);  
        } 
    }
}
