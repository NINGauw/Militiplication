// File: UpgradeSceneUIController.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class UpgradeSceneUIController : MonoBehaviour
{
    [Header("Coin Display")]
    public TextMeshProUGUI coinDisplayText;

    [Header("Move Speed UI")]
    public TextMeshProUGUI moveSpeedLevelText;
    public TextMeshProUGUI moveSpeedValueText;
    public TextMeshProUGUI moveSpeedCostText;
    public Button moveSpeedUpgradeButton;

    [Header("Attack Speed UI")]
    public TextMeshProUGUI attackSpeedLevelText;
    public TextMeshProUGUI attackSpeedValueText;
    public TextMeshProUGUI attackSpeedCostText;
    public Button attackSpeedUpgradeButton;

    [Header("Supply Health Reduction UI")]
    public TextMeshProUGUI supplyUpgradeLevelText;
    public TextMeshProUGUI supplyUpgradeValueText;
    public TextMeshProUGUI supplyUpgradeCostText;
    public Button supplyUpgradeButton;

    [Header("Navigation")]
    public Button backButton;
    public string menuSceneName = "MenuScene"; // Phải khớp với tên Scene Menu

    void Start()
    {
        if (UpgradeManager.Instance == null || CoinManager.Instance == null)
        {
            Debug.LogError("UpgradeManager hoặc CoinManager chưa được khởi tạo! Hãy đảm bảo chúng tồn tại (ví dụ từ MenuScene).");
            // Vô hiệu hóa các nút nếu manager không tồn tại
            SetAllButtonsInteractable(false);
            if (coinDisplayText != null) coinDisplayText.text = "Lỗi Manager!";
            return;
        }

        RefreshAllUIDisplay();

        // Gán sự kiện cho các nút
        if (moveSpeedUpgradeButton != null) moveSpeedUpgradeButton.onClick.AddListener(() => HandleUpgrade("MoveSpeed"));
        if (attackSpeedUpgradeButton != null) attackSpeedUpgradeButton.onClick.AddListener(() => HandleUpgrade("AttackSpeed"));
        if (supplyUpgradeButton != null) supplyUpgradeButton.onClick.AddListener(() => HandleUpgrade("Supply"));
        if (backButton != null) backButton.onClick.AddListener(GoToMenu);
    }

    void HandleUpgrade(string statName)
    {
        if (UpgradeManager.Instance == null) return;

        bool success = UpgradeManager.Instance.AttemptUpgrade(statName);
        if (success)
        {
            RefreshAllUIDisplay(); // Cập nhật lại UI sau khi nâng cấp thành công
            // (Tùy chọn) Phát âm thanh nâng cấp thành công
        }
        else
        {
            // (Tùy chọn) Phát âm thanh thất bại hoặc hiển thị thông báo không đủ tiền/max level
            Debug.Log($"Nâng cấp {statName} thất bại (không đủ coin hoặc đã max level).");
        }
    }

    void RefreshAllUIDisplay()
    {
        if (UpgradeManager.Instance == null || CoinManager.Instance == null)
        {
             Debug.LogError("RefreshAllUIDisplay: UpgradeManager hoặc CoinManager không tồn tại.");
             return;
        }

        if (coinDisplayText != null)
            coinDisplayText.text = $"Coins: {CoinManager.Instance.GetCoinAmount()}";

        int currentCoins = CoinManager.Instance.GetCoinAmount();

        // Move Speed
        int msLevel = UpgradeManager.Instance.GetStatLevel("MoveSpeed");
        int msMaxLevel = UpgradeManager.Instance.GetStatMaxLevel("MoveSpeed");
        float msValue = UpgradeManager.Instance.GetCalculatedStatValue("MoveSpeed");
        int msCost = UpgradeManager.Instance.GetUpgradeCost("MoveSpeed");

        if (moveSpeedLevelText != null) moveSpeedLevelText.text = $"Level {msLevel}";
        if (moveSpeedValueText != null) moveSpeedValueText.text = $"Speed: {msValue:F1}";
        if (msLevel < msMaxLevel)
        {
            if (moveSpeedCostText != null) moveSpeedCostText.text = $"{msCost}";
            if (moveSpeedUpgradeButton != null) moveSpeedUpgradeButton.interactable = currentCoins >= msCost;
        }
        else
        {
            if (moveSpeedCostText != null) moveSpeedCostText.text = "Max";
            if (moveSpeedUpgradeButton != null) moveSpeedUpgradeButton.interactable = false;
        }

        // Attack Speed
        int asLevel = UpgradeManager.Instance.GetStatLevel("AttackSpeed");
        int asMaxLevel = UpgradeManager.Instance.GetStatMaxLevel("AttackSpeed");
        float asValue = UpgradeManager.Instance.GetCalculatedStatValue("AttackSpeed");
        int asCost = UpgradeManager.Instance.GetUpgradeCost("AttackSpeed");

        if (attackSpeedLevelText != null) attackSpeedLevelText.text = $"Level {asLevel}";
        if (attackSpeedValueText != null) attackSpeedValueText.text = $"Rate: {asValue:F1}";
        if (asLevel < asMaxLevel)
        {
            if (attackSpeedCostText != null) attackSpeedCostText.text = $"{asCost}";
            if (attackSpeedUpgradeButton != null) attackSpeedUpgradeButton.interactable = currentCoins >= asCost;
        }
        else
        {
            if (attackSpeedCostText != null) attackSpeedCostText.text = "Max";
            if (attackSpeedUpgradeButton != null) attackSpeedUpgradeButton.interactable = false;
        }

        // Supply Health Reduction
        int supLevel = UpgradeManager.Instance.GetStatLevel("Supply");
        int supMaxLevel = UpgradeManager.Instance.GetStatMaxLevel("Supply");
        float supValueReduction = UpgradeManager.Instance.GetCalculatedStatValue("Supply"); // Đây là lượng giảm
        int supCost = UpgradeManager.Instance.GetUpgradeCost("Supply");

        if (supplyUpgradeLevelText != null) supplyUpgradeLevelText.text = $"Level {supLevel}";
        if (supplyUpgradeValueText != null) supplyUpgradeValueText.text = $"Supply HP: -{supValueReduction:F0}";
        if (supLevel < supMaxLevel)
        {
            if (supplyUpgradeCostText != null) supplyUpgradeCostText.text = $"{supCost}";
            if (supplyUpgradeButton != null) supplyUpgradeButton.interactable = currentCoins >= supCost;
        }
        else
        {
            if (supplyUpgradeCostText != null) supplyUpgradeCostText.text = "Max";
            if (supplyUpgradeButton != null) supplyUpgradeButton.interactable = false;
        }
    }
    
    void SetAllButtonsInteractable(bool isInteractable)
    {
        if (moveSpeedUpgradeButton != null) moveSpeedUpgradeButton.interactable = isInteractable;
        if (attackSpeedUpgradeButton != null) attackSpeedUpgradeButton.interactable = isInteractable;
        if (supplyUpgradeButton != null) supplyUpgradeButton.interactable = isInteractable;
        // Không vô hiệu hóa nút back
    }


    void GoToMenu()
    {
        SceneManager.LoadScene(menuSceneName);
    }
}