using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnEnemy : MonoBehaviour
{
    public GameObject[] enemyPrefabs;
    public GameObject[] spawnPoint;

    private float minTimeRandom = 1;
    private float maxTimeRandom = 4;

    private bool isSpawn;

    private void Update()
    {
        if (GameManager.instance.point >= 4 && !isSpawn)
        {
            isSpawn = true;
            StartCoroutine("randomSpawnPoint");
        }
    }

    IEnumerator randomSpawnPoint()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(minTimeRandom, maxTimeRandom));    // Random Time to Spawn Enemy
            SpawnEnemyPrefab();
        }
    }

    void SpawnEnemyPrefab()
    {
        int n = Random.Range(0, 3);                                                         // n for Enemy Array  
        int i = Random.Range(0, 4);
        float r = Random.Range(-3, 4);

        if (i == 0)
        {
            Vector2 spawnPos = new Vector2(spawnPoint[0].transform.position.x, spawnPoint[0].transform.position.y + r);
            GameObject enemyClone = Instantiate(enemyPrefabs[n], spawnPos, Quaternion.identity);
        }

        if (i == 1)
        {
            Vector2 spawnPos = new Vector2(spawnPoint[1].transform.position.x, spawnPoint[1].transform.position.y + r);
            GameObject enemyClone = Instantiate(enemyPrefabs[n], spawnPos, Quaternion.identity);
        }

        if (i == 2)
        {
            Vector2 spawnPos = new Vector2(spawnPoint[2].transform.position.x + r, spawnPoint[2].transform.position.y);
            GameObject enemyClone = Instantiate(enemyPrefabs[n], spawnPos, Quaternion.identity);
        }

        if (i == 3)
        {
            Vector2 spawnPos = new Vector2(spawnPoint[3].transform.position.x + r, spawnPoint[3].transform.position.y);
            GameObject enemyClone = Instantiate(enemyPrefabs[n], spawnPos, Quaternion.identity);
        }
    }
}