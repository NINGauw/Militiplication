using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;       // Prefab của enemy bạn muốn spawn
    public float spawnInterval = 2f;     // Khoảng thời gian giữa mỗi lần spawn (giây)
    public int enemiesPerSpawn = 1;      // Số lượng quái spawn mỗi lần
    public Transform[] spawnPoints;      // Danh sách vị trí spawn

    private float spawnTimer;

    void Update()
    {
        spawnTimer += Time.deltaTime;

        if (spawnTimer >= spawnInterval)
        {
            SpawnEnemies();
            spawnTimer = 0f;
        }
    }

    void SpawnEnemies()
    {
        for (int i = 0; i < enemiesPerSpawn; i++)
        {
            // Lấy vị trí spawn ngẫu nhiên trong mảng
            Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

            Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
        }
    }
}
