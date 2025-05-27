using UnityEngine;
using UnityEngine.UI;
public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance;

    [Header("Boss Settings")]
    public GameObject bossPrefab;
    public Transform bossSpawnPoint;
    public int enemiesToSpawnBoss = 20;
    public GameObject bossHealthBarUI;

    private int enemiesDefeated = 0;
    private bool bossSpawned = false;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
    public void OnEnemyDefeated()
    {
        enemiesDefeated++;

        if (!bossSpawned && enemiesDefeated >= enemiesToSpawnBoss)
        {
            SpawnBoss();
        }
    }

    private void SpawnBoss()
    {
        if (bossPrefab != null && bossSpawnPoint != null)
        {
            bossHealthBarUI.SetActive(true);
            Instantiate(bossPrefab, bossSpawnPoint.position, bossSpawnPoint.rotation);
            bossSpawned = true;
            Debug.Log("Boss spawned!");
        }
        else
        {
            Debug.LogWarning("Boss prefab or spawn point is not assigned.");
        }
    }
}
