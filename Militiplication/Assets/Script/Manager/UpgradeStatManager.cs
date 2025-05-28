using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager Instance;

    [Header("Coin Display")]
    public TextMeshProUGUI coinDisplayText;

    [Header("Move Speed Upgrade")]
    public TextMeshProUGUI moveSpeedLevelText;
    public TextMeshProUGUI moveSpeedValueText;
    public TextMeshProUGUI moveSpeedCostText;
    public Button moveSpeedUpgradeButton;
    public float baseMoveSpeed = 6f;
    public float moveSpeedIncrementPerLevel = 0.5f;
    public int[] moveSpeedUpgradeCosts;
    public int moveSpeedMaxLevel = 10;
    private int currentMoveSpeedLevel;

    [Header("Attack Speed Upgrade")]
    public TextMeshProUGUI attackSpeedLevelText;
    public TextMeshProUGUI attackSpeedValueText;
    public TextMeshProUGUI attackSpeedCostText;
    public Button attackSpeedUpgradeButton;
    public float baseAttackSpeed = 1.5f;
    public float attackSpeedIncrementPerLevel = 0.2f;
    public int[] attackSpeedUpgradeCosts;
    public int attackSpeedMaxLevel = 10;
    private int currentAttackSpeedLevel;

    [Header("Supply Health Reduction Upgrade")] // Đổi tên cho rõ nghĩa hơn
    public TextMeshProUGUI supplyUpgradeLevelText;
    public TextMeshProUGUI supplyUpgradeValueText;
    public TextMeshProUGUI supplyUpgradeCostText;
    public Button supplyUpgradeButton;
    public int supplyHealthReductionPerLevel = 1; // Mỗi cấp giảm 1 HP của Supply
    public int[] supplyUpgradeCosts;
    public int supplyUpgradeMaxLevel = 5; // Ví dụ: giảm tối đa 5 HP
    private int currentSupplyUpgradeLevel;

    [Header("Navigation")]
    public Button backButton;
    public string menuSceneName = "MenuScene";

    // private int playerCoins; // Xóa dòng này, sẽ dùng CoinManager

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            Destroy(gameObject);
    }

    void Start()
    {
        LoadUpgradeLevels(); // Chỉ tải cấp độ nâng cấp, coin sẽ do CoinManager quản lý
        UpdateAllUI();

        moveSpeedUpgradeButton.onClick.AddListener(() => AttemptUpgrade("MoveSpeed"));
        attackSpeedUpgradeButton.onClick.AddListener(() => AttemptUpgrade("AttackSpeed"));
        supplyUpgradeButton.onClick.AddListener(() => AttemptUpgrade("Supply"));

        if (backButton != null)
            backButton.onClick.AddListener(GoToMenu);
    }

    void LoadUpgradeLevels() // Đổi tên từ LoadPlayerData
    {
        // playerCoins = PlayerPrefs.GetInt("PlayerCoins", 1000); // Xóa, CoinManager sẽ load
        currentMoveSpeedLevel = PlayerPrefs.GetInt("MoveSpeedLevel", 0);
        currentAttackSpeedLevel = PlayerPrefs.GetInt("AttackSpeedLevel", 0);
        currentSupplyUpgradeLevel = PlayerPrefs.GetInt("SupplyUpgradeLevel", 0);
    }

    void SaveUpgradeLevels() // Đổi tên từ SavePlayerData
    {
        // PlayerPrefs.SetInt("PlayerCoins", playerCoins); // Xóa, CoinManager sẽ save
        PlayerPrefs.SetInt("MoveSpeedLevel", currentMoveSpeedLevel);
        PlayerPrefs.SetInt("AttackSpeedLevel", currentAttackSpeedLevel);
        PlayerPrefs.SetInt("SupplyUpgradeLevel", currentSupplyUpgradeLevel);
        PlayerPrefs.Save();
    }

    void UpdateAllUI()
    {
        if (CoinManager.Instance != null)
        {
            coinDisplayText.text = $"{CoinManager.Instance.GetCoinAmount()}";
        }
        else
        {
            coinDisplayText.text = "N/A";
            Debug.LogError("CoinManager.Instance is not available!");
        }
        
        int currentCoins = (CoinManager.Instance != null) ? CoinManager.Instance.GetCoinAmount() : 0;

        // Move Speed UI
        moveSpeedLevelText.text = $"Level {currentMoveSpeedLevel}";
        float actualMoveSpeed = baseMoveSpeed + currentMoveSpeedLevel * moveSpeedIncrementPerLevel;
        moveSpeedValueText.text = $"Speed: {actualMoveSpeed:F1}";
        if (currentMoveSpeedLevel < moveSpeedMaxLevel)
        {
            moveSpeedCostText.text = $"{GetCost(moveSpeedUpgradeCosts, currentMoveSpeedLevel)}";
            moveSpeedUpgradeButton.interactable = currentCoins >= GetCost(moveSpeedUpgradeCosts, currentMoveSpeedLevel);
        }
        else
        {
            moveSpeedCostText.text = "Max";
            moveSpeedUpgradeButton.interactable = false;
        }

        // Attack Speed UI
        attackSpeedLevelText.text = $"Level {currentAttackSpeedLevel}";
        float actualAttackSpeed = baseAttackSpeed - (currentAttackSpeedLevel * attackSpeedIncrementPerLevel);
        attackSpeedValueText.text = $"Rate: {actualAttackSpeed:F1}";
        if (currentAttackSpeedLevel < attackSpeedMaxLevel)
        {
            attackSpeedCostText.text = $"{GetCost(attackSpeedUpgradeCosts, currentAttackSpeedLevel)}";
            attackSpeedUpgradeButton.interactable = currentCoins >= GetCost(attackSpeedUpgradeCosts, currentAttackSpeedLevel);
        }
        else
        {
            attackSpeedCostText.text = "Max";
            attackSpeedUpgradeButton.interactable = false;
        }

        // Supply Upgrade UI
        supplyUpgradeLevelText.text = $"Level {currentSupplyUpgradeLevel}";
        int healthReduction = currentSupplyUpgradeLevel * supplyHealthReductionPerLevel;
        supplyUpgradeValueText.text = $"Reduce: -{healthReduction}"; // Hiển thị lượng HP giảm
        if (currentSupplyUpgradeLevel < supplyUpgradeMaxLevel)
        {
            supplyUpgradeCostText.text = $"{GetCost(supplyUpgradeCosts, currentSupplyUpgradeLevel)}";
            supplyUpgradeButton.interactable = currentCoins >= GetCost(supplyUpgradeCosts, currentSupplyUpgradeLevel);
        }
        else
        {
            supplyUpgradeCostText.text = "Max";
            supplyUpgradeButton.interactable = false;
        }
    }

    int GetCost(int[] costsArray, int currentLevel)
    {
        if (currentLevel < costsArray.Length)
        {
            return costsArray[currentLevel];
        }
        return int.MaxValue;
    }

    public void AttemptUpgrade(string statName) // Đổi tên từ UpgradeStat
    {
        if (CoinManager.Instance == null)
        {
            Debug.LogError("CoinManager.Instance is not available! Cannot process upgrade.");
            return;
        }

        int cost = 0;
        bool canUpgrade = false;

        switch (statName)
        {
            case "MoveSpeed":
                if (currentMoveSpeedLevel < moveSpeedMaxLevel)
                {
                    cost = GetCost(moveSpeedUpgradeCosts, currentMoveSpeedLevel);
                    if (CoinManager.Instance.GetCoinAmount() >= cost)
                    {
                        currentMoveSpeedLevel++;
                        canUpgrade = true;
                    }
                }
                break;
            case "AttackSpeed":
                if (currentAttackSpeedLevel < attackSpeedMaxLevel)
                {
                    cost = GetCost(attackSpeedUpgradeCosts, currentAttackSpeedLevel);
                    if (CoinManager.Instance.GetCoinAmount() >= cost)
                    {
                        currentAttackSpeedLevel++;
                        canUpgrade = true;
                    }
                }
                break;
            case "Supply":
                if (currentSupplyUpgradeLevel < supplyUpgradeMaxLevel)
                {
                    cost = GetCost(supplyUpgradeCosts, currentSupplyUpgradeLevel);
                    if (CoinManager.Instance.GetCoinAmount() >= cost)
                    {
                        currentSupplyUpgradeLevel++;
                        canUpgrade = true;
                    }
                }
                break;
            default:
                Debug.LogWarning("Unknown stat name: " + statName);
                return;
        }

        if (canUpgrade)
        {
            CoinManager.Instance.SpendCoins(cost); // Trừ tiền thông qua CoinManager
            SaveUpgradeLevels(); // Lưu cấp độ mới
            UpdateAllUI();      // Cập nhật lại toàn bộ UI
            Debug.Log($"Nâng cấp {statName} thành công! Cấp mới: { (statName == "MoveSpeed" ? currentMoveSpeedLevel : (statName == "AttackSpeed" ? currentAttackSpeedLevel : currentSupplyUpgradeLevel)) }");
        }
        else
        {
            Debug.Log($"Không thể nâng cấp {statName}. Đã đạt cấp tối đa hoặc không đủ coin.");
            // (Tùy chọn) Hiển thị thông báo cho người chơi là không đủ tiền hoặc đã max level
        }
    }

    void GoToMenu()
    {
        SceneManager.LoadScene(menuSceneName);
    }

    // Các hàm static để script khác lấy giá trị stat đã nâng cấp
    public static float GetCurrentMoveSpeedValue() { // Đổi tên cho rõ là lấy "Value"
        if (Instance == null) { Debug.LogWarning("UpgradeManager.Instance is null, returning base move speed."); return 5f; } // Giá trị mặc định an toàn
        return Instance.baseMoveSpeed + Instance.currentMoveSpeedLevel * Instance.moveSpeedIncrementPerLevel;
    }
    public static float GetCurrentAttackSpeedValue() { // Đổi tên
        if (Instance == null) { Debug.LogWarning("UpgradeManager.Instance is null, returning base attack speed."); return 1f; }
        return Instance.baseAttackSpeed + Instance.currentAttackSpeedLevel * Instance.attackSpeedIncrementPerLevel;
    }
    // Hàm này sẽ trả về tổng số HP bị giảm cho Supply
    public static int GetCurrentSupplyHealthReduction() {
        if (Instance == null) { Debug.LogWarning("UpgradeManager.Instance is null, returning 0 supply health reduction."); return 0; }
        return Instance.currentSupplyUpgradeLevel * Instance.supplyHealthReductionPerLevel;
    }
}