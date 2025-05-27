using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    // public int totalEnemies = 20; // Sẽ được tính toán tự động từ các Spawner
    public int totalBoss = 1; // Giữ lại nếu boss được spawn theo cách khác Spawner thường
    private int enemiesRemaining;

    public TextMeshProUGUI enemiesLeftText; // Đảm bảo đã gán từ Inspector

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // DontDestroyOnLoad(gameObject); // Nếu bạn muốn GameManager tồn tại qua các scene
        }
        else if (Instance != this)
        {
            Destroy(gameObject); // Đảm bảo chỉ có một GameManager instance
            return;
        }
    }

    void Start()
    {
        InitializeLevelEnemyCount();
        UpdateEnemyCountUI();
    }

    void InitializeLevelEnemyCount()
    {
        int calculatedTotalEnemiesFromSpawners = 0;
        EnemySpawner[] spawners = FindObjectsOfType<EnemySpawner>(); // Tìm tất cả EnemySpawner trong scene

        if (spawners.Length == 0)
        {
            Debug.LogWarning("No EnemySpawners found in the scene. Total enemies will rely on manual setup or be 0 (plus bosses).", this);
        }
        else
        {
            foreach (EnemySpawner spawner in spawners)
            {
                if (spawner.enabled) // Chỉ tính các spawner đang hoạt động
                {
                    calculatedTotalEnemiesFromSpawners += spawner.GetPlannedEnemyCount();
                }
            }
            Debug.Log($"Calculated total enemies from {spawners.Length} spawners: {calculatedTotalEnemiesFromSpawners}", this);
        }

        // enemiesRemaining giờ sẽ là tổng số quái từ các spawner cộng với số boss (nếu boss không được quản lý bởi EnemySpawner)
        enemiesRemaining = calculatedTotalEnemiesFromSpawners + totalBoss;
        Debug.Log($"GameManager initialized. Total enemies to defeat (including bosses): {enemiesRemaining}", this);
    }

    public void OnEnemySpawned()
    {
        // Hàm này được gọi bởi EnemySpawner.
        // Hiện tại, chúng ta đã biết tổng số lượng quái từ đầu thông qua InitializeLevelEnemyCount().
        // Nên hàm này có thể không cần làm gì liên quan đến enemiesRemaining.
        // Bạn có thể dùng nó cho mục đích khác nếu cần, ví dụ: theo dõi số lượng quái đang active.
        // Debug.Log("An enemy has been spawned. GameManager notified.");
    }

    public void OnEnemyDefeated() // Được gọi bởi EnemyController khi quái thường bị tiêu diệt
    {
        if (enemiesRemaining > 0) // Chỉ trừ khi còn quái
        {
            enemiesRemaining--;
        }
        else
        {
            Debug.LogWarning("OnEnemyDefeated called, but enemiesRemaining is already 0 or less.", this);
        }
        UpdateEnemyCountUI();
        CheckWinCondition();
    }

    public void OnBossDefeated() // Được gọi bởi script của Boss khi boss bị tiêu diệt
    {
        if (enemiesRemaining > 0) // Chỉ trừ khi còn quái (boss cũng là một phần của enemiesRemaining)
        {
            enemiesRemaining--;
        }
        else
        {
             Debug.LogWarning("OnBossDefeated called, but enemiesRemaining is already 0 or less.", this);
        }
        UpdateEnemyCountUI();
        CheckWinCondition();
    }

    private void CheckWinCondition()
    {
        if (enemiesRemaining <= 0)
        {
            Debug.Log("🎉 You Win! All enemies and bosses defeated.", this);

            if (WinUIManager.Instance != null)
            {
                WinUIManager.Instance.TriggerWin();
            }
            else
            {
                Debug.LogWarning("WinUIManager.Instance is null.", this);
            }

            if (CoinManager.Instance != null)
            {
                CoinManager.Instance.AddCoins(5); // Số coin thưởng có thể tùy chỉnh
            }
            else
            {
                Debug.LogWarning("CoinManager.Instance is null.", this);
            }
            
            // (Tùy chọn) Dừng game hoặc chuyển màn, v.v.
            // Time.timeScale = 0f; // Ví dụ: Dừng game
        }
    }

    private void UpdateEnemyCountUI()
    {
        if (enemiesLeftText != null)
        {
            enemiesLeftText.text = $"Enemies left: {enemiesRemaining}";
        }
        else
        {
            Debug.LogWarning("enemiesLeftText is not assigned in GameManager Inspector.", this);
        }
    }
}