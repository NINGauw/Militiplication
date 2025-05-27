using UnityEngine;
using System.Collections.Generic;
using System.Linq; 


public class EnemySpawner : MonoBehaviour
{
    [Header("Enemy Spawn Configuration")]
    public List<EnemySpawnData> enemiesToSpawnList; // Danh sách các loại enemy và số lượng cần spawn

    [Header("Spawner Settings")]
    public float spawnInterval = 2f;

    private float spawnTimer = 0f;
    private int totalEnemiesRemainingInWave; // Tổng số enemy còn lại cần spawn trong đợt này
    private int initialTotalEnemiesInWave;

    void Awake()
    {
        InitializeSpawner();
    }

    void InitializeSpawner()
    {
        initialTotalEnemiesInWave = 0; // Khởi tạo
        if (enemiesToSpawnList == null || enemiesToSpawnList.Count == 0)
        {
            Debug.LogError("EnemiesToSpawnList is not configured! Spawner will not function.", this);
            enabled = false;
            return;
        }

        foreach (EnemySpawnData data in enemiesToSpawnList)
        {
            if (data.enemyPrefab == null)
            {
                Debug.LogError($"An entry in EnemiesToSpawnList has a null prefab. Please check configuration.", this);
                enabled = false;
                return;
            }
            data.spawnedCount = 0;
            initialTotalEnemiesInWave += data.quantityToSpawn; // Cộng dồn vào tổng ban đầu
        }
        totalEnemiesRemainingInWave = initialTotalEnemiesInWave; // Gán cho biến đếm ngược

        if (initialTotalEnemiesInWave == 0 && enemiesToSpawnList.Count > 0) // Vẫn có cấu hình nhưng tổng là 0
        {
            Debug.LogWarning("Total enemies to spawn in the wave is 0 based on configuration. Spawner might not do anything.", this);
        }
        Debug.Log($"Enemy Spawner initialized (Awake). Total enemies planned by this spawner: {initialTotalEnemiesInWave}", this);
    }

    public int GetPlannedEnemyCount()
    {
        return initialTotalEnemiesInWave;
    }
    void Update()
    {
        if (totalEnemiesRemainingInWave > 0)
        {
            spawnTimer += Time.deltaTime;

            if (spawnTimer >= spawnInterval)
            {
                SpawnEnemy();
                spawnTimer = 0f; // Reset timer
            }
        }
        else
        {
            if (enabled) // Chỉ log một lần sau khi hoàn thành
            {
                Debug.Log("All configured enemies have been spawned for this wave.", this);
                enabled = false; // Vô hiệu hóa spawner sau khi hoàn thành
            }
        }
    }

    void SpawnEnemy()
    {
        // Tạo một danh sách các loại enemy còn có thể spawn (chưa đủ số lượng)
        List<EnemySpawnData> availableToSpawn = new List<EnemySpawnData>();
        foreach (EnemySpawnData data in enemiesToSpawnList)
        {
            if (data.spawnedCount < data.quantityToSpawn)
            {
                availableToSpawn.Add(data);
            }
        }

        // Nếu không còn loại nào để spawn (dù totalEnemiesRemainingInWave > 0, trường hợp này không nên xảy ra nếu logic đúng)
        if (availableToSpawn.Count == 0)
        {
            Debug.LogWarning("No available enemy types to spawn, but wave is not marked as complete. Check logic.", this);
            totalEnemiesRemainingInWave = 0; // Sửa lại trạng thái
            return;
        }

        // Chọn ngẫu nhiên một loại từ danh sách các loại còn có thể spawn
        int randomIndex = Random.Range(0, availableToSpawn.Count);
        EnemySpawnData selectedData = availableToSpawn[randomIndex];

        if (selectedData.enemyPrefab == null) // Kiểm tra lại phòng trường hợp prefab bị null
        {
            Debug.LogError("Selected prefab to spawn is null. This should have been caught in Start. Skipping.", this);
            return;
        }

        // Spawn enemy đã chọn
        Instantiate(selectedData.enemyPrefab, transform.position, transform.rotation);
        selectedData.spawnedCount++;
        totalEnemiesRemainingInWave--;

        Debug.Log($"Spawned: {selectedData.enemyPrefab.name}. Remaining in wave: {totalEnemiesRemainingInWave}. Type spawned: {selectedData.spawnedCount}/{selectedData.quantityToSpawn}", this);

        // Thông báo cho GameManager (nếu có)
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnEnemySpawned();
        }
        else
        {
            Debug.LogWarning("GameManager.Instance is null. Cannot call OnEnemySpawned.", this);
        }
    }
}