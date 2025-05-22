using System;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public float spawnInterval = 2f;
    public int totalEnemiesToSpawn;

    private float spawnTimer = 0f;

    void Start()
    {
        totalEnemiesToSpawn = GameManager.Instance.totalEnemies;
    }
    void Update()
    {
        if (totalEnemiesToSpawn > 0)
        {
            spawnTimer += Time.deltaTime;

            if (spawnTimer >= spawnInterval)
            {
                SpawnEnemy();
                spawnTimer = 0f;
            }
        }
    }

    void SpawnEnemy()
    {
        Instantiate(enemyPrefab, transform.position, transform.rotation);
        totalEnemiesToSpawn--;

        GameManager.Instance.OnEnemySpawned();
    }
}
