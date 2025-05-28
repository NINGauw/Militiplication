// File: UpgradeManager.cs
using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager Instance { get; private set; }

    // Các thông số cơ bản và gia tăng cho từng chỉ số
    // Những giá trị này sẽ được thiết lập một lần trong Inspector của UpgradeManager ở MenuScene
    [Header("Move Speed Config")]
    public float baseMoveSpeed = 6f;
    public float moveSpeedIncrementPerLevel = 0.25f;
    public int[] moveSpeedUpgradeCosts;
    public int moveSpeedMaxLevel = 5;
    private int currentMoveSpeedLevel;

    [Header("Attack Speed Config")]
    public float baseAttackSpeed = 1.5f; // Ví dụ: 1 phát/giây
    public float attackSpeedIncrementPerLevel = 0.2f;
    public int[] attackSpeedUpgradeCosts;
    public int attackSpeedMaxLevel = 5;
    private int currentAttackSpeedLevel;

    [Header("Supply Health Reduction Config")]
    public int supplyHealthReductionPerLevel = 1; // Mỗi cấp giảm 1 HP của Supply
    public int[] supplyUpgradeCosts;
    public int supplyUpgradeMaxLevel = 5;
    private int currentSupplyUpgradeLevel;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Quan trọng: Giữ lại khi chuyển scene
            LoadUpgradeLevels(); // Tải dữ liệu nâng cấp ngay khi được tạo
        }
        else if (Instance != this)
        {
            Destroy(gameObject); // Hủy các bản sao
        }
    }

    void LoadUpgradeLevels()
    {
        currentMoveSpeedLevel = PlayerPrefs.GetInt("MoveSpeedLevel", 0);
        currentAttackSpeedLevel = PlayerPrefs.GetInt("AttackSpeedLevel", 0);
        currentSupplyUpgradeLevel = PlayerPrefs.GetInt("SupplyUpgradeLevel", 0);
        Debug.Log("UpgradeManager: Dữ liệu cấp độ nâng cấp đã được tải.");
    }

    void SaveUpgradeLevels()
    {
        PlayerPrefs.SetInt("MoveSpeedLevel", currentMoveSpeedLevel);
        PlayerPrefs.SetInt("AttackSpeedLevel", currentAttackSpeedLevel);
        PlayerPrefs.SetInt("SupplyUpgradeLevel", currentSupplyUpgradeLevel);
        PlayerPrefs.Save();
        Debug.Log("UpgradeManager: Dữ liệu cấp độ nâng cấp đã được lưu.");
    }

    public int GetUpgradeCost(string statName)
    {
        int currentLevel = 0;
        int[] costsArray = null;

        switch (statName)
        {
            case "MoveSpeed":
                currentLevel = currentMoveSpeedLevel;
                costsArray = moveSpeedUpgradeCosts;
                break;
            case "AttackSpeed":
                currentLevel = currentAttackSpeedLevel;
                costsArray = attackSpeedUpgradeCosts;
                break;
            case "Supply":
                currentLevel = currentSupplyUpgradeLevel;
                costsArray = supplyUpgradeCosts;
                break;
            default:
                return int.MaxValue;
        }

        if (costsArray == null || currentLevel >= costsArray.Length)
        {
            return int.MaxValue; // Đã max hoặc không có chi phí định nghĩa
        }
        return costsArray[currentLevel];
    }

    public bool AttemptUpgrade(string statName)
    {
        if (CoinManager.Instance == null)
        {
            Debug.LogError("CoinManager.Instance không tồn tại! Không thể nâng cấp.");
            return false;
        }

        int cost = GetUpgradeCost(statName);
        int currentLevel = GetStatLevel(statName);
        int maxLevel = GetStatMaxLevel(statName);

        if (currentLevel >= maxLevel)
        {
            Debug.Log($"Chỉ số {statName} đã đạt cấp tối đa.");
            return false;
        }

        if (CoinManager.Instance.GetCoinAmount() >= cost)
        {
            if (CoinManager.Instance.SpendCoins(cost))
            {
                switch (statName)
                {
                    case "MoveSpeed": currentMoveSpeedLevel++; break;
                    case "AttackSpeed": currentAttackSpeedLevel++; break;
                    case "Supply": currentSupplyUpgradeLevel++; break;
                    default: return false; // Không nên xảy ra
                }
                SaveUpgradeLevels();
                Debug.Log($"Nâng cấp {statName} thành công! Cấp mới: {GetStatLevel(statName)}");
                return true;
            }
            else
            {
                Debug.LogWarning($"SpendCoins cho {statName} thất bại dù đã kiểm tra đủ coin.");
                return false;
            }
        }
        else
        {
            Debug.Log($"Không đủ coin để nâng cấp {statName}. Cần: {cost}, Hiện có: {CoinManager.Instance.GetCoinAmount()}");
            return false;
        }
    }

    // Các hàm Getter cho UpgradeSceneUIController
    public int GetStatLevel(string statName)
    {
        switch (statName)
        {
            case "MoveSpeed": return currentMoveSpeedLevel;
            case "AttackSpeed": return currentAttackSpeedLevel;
            case "Supply": return currentSupplyUpgradeLevel;
            default: return -1;
        }
    }

    public int GetStatMaxLevel(string statName)
    {
        switch (statName)
        {
            case "MoveSpeed": return moveSpeedMaxLevel;
            case "AttackSpeed": return attackSpeedMaxLevel;
            case "Supply": return supplyUpgradeMaxLevel;
            default: return -1;
        }
    }
    
    public float GetCalculatedStatValue(string statName)
    {
        switch (statName)
        {
            case "MoveSpeed": return baseMoveSpeed + currentMoveSpeedLevel * moveSpeedIncrementPerLevel;
            case "AttackSpeed": return baseAttackSpeed - currentAttackSpeedLevel * attackSpeedIncrementPerLevel;
            case "Supply": return supplyHealthReductionPerLevel * currentSupplyUpgradeLevel; // Đây là lượng giảm HP
            default: return -1f;
        }
    }


    // Các hàm static để script trong màn chơi (PlayerController, SupplyHealth) có thể lấy giá trị cuối cùng
    public static float GetCurrentMoveSpeedValue()
    {
        if (Instance == null) { Debug.LogWarning("UpgradeManager.Instance is null. Trả về giá trị MoveSpeed cơ bản."); return 5f; } // Giá trị mặc định an toàn
        return Instance.baseMoveSpeed + Instance.currentMoveSpeedLevel * Instance.moveSpeedIncrementPerLevel;
    }

    public static float GetCurrentAttackSpeedValue()
    {
        if (Instance == null) { Debug.LogWarning("UpgradeManager.Instance is null. Trả về giá trị AttackSpeed cơ bản."); return 1f; }
        return Instance.baseAttackSpeed - Instance.currentAttackSpeedLevel * Instance.attackSpeedIncrementPerLevel;
    }

    public static int GetCurrentSupplyHealthReduction()
    {
        if (Instance == null) { Debug.LogWarning("UpgradeManager.Instance is null. Trả về giá trị SupplyHealthReduction là 0."); return 0; }
        return Instance.currentSupplyUpgradeLevel * Instance.supplyHealthReductionPerLevel;
    }
}