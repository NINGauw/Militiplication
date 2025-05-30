using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SupplierManager : MonoBehaviour
{
    public static SupplierManager Instance { get; private set; }

    [Tooltip("Prefab FX cho kỹ năng tăng tốc độ bắn. Các FX khác có thể được quản lý tương tự hoặc trong SupplierData.")]
    public GameObject fireRateFXPrefab; // Bạn có thể mở rộng để mỗi SupplierData có FX riêng nếu cần

    [Header("Supplier List - Cấu hình trong Inspector")]
    [Tooltip("Danh sách tất cả các kỹ năng có thể có trong game.")]
    public List<SupplierData> availableRewards;

    // Tên (ID) của kỹ năng đang được chọn
    private string _selectedRewardName;
    public string SelectedRewardName
    {
        get { return _selectedRewardName; }
        // Không cho phép set từ bên ngoài, chỉ qua hàm SetSelectedReward
    }

    // Keys dùng cho PlayerPrefs
    private const string SELECTED_REWARD_KEY_PP = "PlayerSelectedSupplyID";
    private const string SUPPLY_UNLOCKED_PREFIX_PP = "SupplyUnlocked_";

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Quan trọng: Giữ lại khi chuyển scene
            InitializeManager();
        }
        else if (Instance != this)
        {
            Destroy(gameObject); // Hủy các bản sao thừa
        }
    }

    void InitializeManager()
    {
        LoadUnlockStates();     // Tải trạng thái mở khóa trước
        LoadSelectedReward();   // Sau đó tải kỹ năng đã chọn (và kiểm tra xem nó có hợp lệ không)
        Debug.Log("SupplierManager Initialized. Số lượng kỹ năng có sẵn: " + (availableRewards?.Count ?? 0));
    }

    void LoadSelectedReward()
    {
        _selectedRewardName = PlayerPrefs.GetString(SELECTED_REWARD_KEY_PP, "");

        // Nếu chưa có lựa chọn nào được lưu, hoặc lựa chọn đã lưu không hợp lệ/bị khóa,
        // hãy thử chọn kỹ năng đầu tiên trong danh sách đã được mở khóa theo mặc định.
        if (string.IsNullOrEmpty(_selectedRewardName) || !IsRewardSelectable(_selectedRewardName))
        {
            SupplierData defaultSelectableReward = availableRewards.FirstOrDefault(r => r.isCurrentlyUnlocked);
            if (defaultSelectableReward != null)
            {
                _selectedRewardName = defaultSelectableReward.supplierName;
                Debug.Log($"Không có lựa chọn hợp lệ, chọn mặc định: {defaultSelectableReward.displayName}");
            }
            else
            {
                _selectedRewardName = ""; // Không có gì để chọn mặc định
                Debug.LogWarning("Không có kỹ năng nào được mở khóa để chọn làm mặc định.");
            }
            // Không cần Save() ở đây vì người chơi chưa thực sự chọn.
            // Việc chọn sẽ diễn ra trong Scene Chọn Kỹ Năng.
        }
        Debug.Log($"Kỹ năng đã tải được chọn: {(string.IsNullOrEmpty(_selectedRewardName) ? "Không có" : _selectedRewardName)}");
    }

    // Kiểm tra xem một reward có tồn tại và đã được mở khóa không
    private bool IsRewardSelectable(string rewardName)
    {
        SupplierData reward = availableRewards.Find(r => r.supplierName == rewardName);
        return reward != null && reward.isCurrentlyUnlocked;
    }

    public void SetSelectedReward(string rewardName)
    {
        SupplierData rewardData = availableRewards.Find(r => r.supplierName == rewardName);
        if (rewardData != null && rewardData.isCurrentlyUnlocked)
        {
            _selectedRewardName = rewardName;
            PlayerPrefs.SetString(SELECTED_REWARD_KEY_PP, _selectedRewardName);
            PlayerPrefs.Save();
            Debug.Log($"SupplierManager: Kỹ năng đã chọn và lưu: {_selectedRewardName}");
        }
        else if (rewardData == null)
        {
            Debug.LogWarning($"SupplierManager: Không tìm thấy kỹ năng với tên: {rewardName}");
        }
        else // rewardData != null nhưng !rewardData.isCurrentlyUnlocked
        {
            Debug.LogWarning($"SupplierManager: Cố gắng chọn kỹ năng bị khóa: {rewardName}");
        }
    }

    // --- Logic Mở Khóa ---
    void LoadUnlockStates()
    {
        if (availableRewards == null) return;

        foreach (var reward in availableRewards)
        {
            // Giá trị mặc định khi GetInt là 0 (false). Nếu unlockedByDefault là true, thì mặc định là 1 (true).
            int defaultUnlockState = reward.unlockedByDefault ? 1 : 0;
            reward.isCurrentlyUnlocked = PlayerPrefs.GetInt(SUPPLY_UNLOCKED_PREFIX_PP + reward.supplierName, defaultUnlockState) == 1;

            // Nếu một kỹ năng được đánh dấu unlockedByDefault và chưa có key PlayerPrefs nào cho nó,
            // hãy tạo key đó với trạng thái đã mở khóa.
            if (reward.unlockedByDefault && !PlayerPrefs.HasKey(SUPPLY_UNLOCKED_PREFIX_PP + reward.supplierName))
            {
                PlayerPrefs.SetInt(SUPPLY_UNLOCKED_PREFIX_PP + reward.supplierName, 1);
            }
        }
        PlayerPrefs.Save(); // Lưu lại nếu có key mới được tạo cho các mục unlockedByDefault
        Debug.Log("Trạng thái mở khóa của các kỹ năng đã được tải.");
    }

    public bool IsSupplyUnlocked(string supplierName)
    {
        SupplierData reward = availableRewards.Find(r => r.supplierName == supplierName);
        return reward != null && reward.isCurrentlyUnlocked;
    }

    public void UnlockSupply(string supplierName)
    {
        SupplierData reward = availableRewards.Find(r => r.supplierName == supplierName);
        if (reward != null)
        {
            if (!reward.isCurrentlyUnlocked)
            {
                reward.isCurrentlyUnlocked = true;
                PlayerPrefs.SetInt(SUPPLY_UNLOCKED_PREFIX_PP + supplierName, 1);
                PlayerPrefs.Save();
                Debug.Log($"SupplierManager: Kỹ năng '{supplierName}' đã được mở khóa.");
            }
            else
            {
                Debug.Log($"SupplierManager: Kỹ năng '{supplierName}' đã được mở khóa từ trước.");
            }
        }
        else
        {
            Debug.LogWarning($"SupplierManager: Không tìm thấy kỹ năng '{supplierName}' để mở khóa.");
        }
    }

    // --- Logic Kích Hoạt Kỹ Năng ---
    public void ActivateSelectedReward(GameObject player)
    {
        if (string.IsNullOrEmpty(_selectedRewardName))
        {
            Debug.LogWarning("SupplierManager: Không có kỹ năng nào được chọn để kích hoạt.");
            return;
        }

        SupplierData reward = availableRewards.Find(r => r.supplierName == _selectedRewardName);
        if (reward != null && reward.isCurrentlyUnlocked)
        {
            ApplyReward(player, reward);
        }
        else
        {
            Debug.LogWarning($"SupplierManager: Kỹ năng đã chọn '{_selectedRewardName}' không tìm thấy hoặc bị khóa. Không có kỹ năng nào được áp dụng.");
        }
    }

    public void ApplyReward(GameObject player, SupplierData rewardData)
    {
        Debug.Log($"Đang áp dụng kỹ năng: {rewardData.displayName} (Loại: {rewardData.type}) cho người chơi.");
        switch (rewardData.type)
        {
            case SupplierType.FireRateBoost:
                // Sử dụng giá trị từ SupplierData, nếu không có thì dùng mặc định
                float duration = rewardData.duration > 0 ? rewardData.duration : 20f;
                float multiplier = rewardData.value > 0 ? rewardData.value : 2f;
                ApplyFireRateBoost(player, multiplier, duration);
                break;

            /*case SupplierType.HealthBoost:
                PlayerHealth playerHealth = player.GetComponent<PlayerHealth>(); // Giả sử bạn có script PlayerHealth
                if (playerHealth != null)
                {
                    playerHealth.Heal((int)rewardData.value); // Giả sử value là lượng máu hồi
                    Debug.Log($"Người chơi được hồi {(int)rewardData.value} máu.");
                } else { Debug.LogWarning("Không tìm thấy PlayerHealth component trên người chơi."); }
                break;
            
            case SupplierType.Shield:
                PlayerShield playerShield = player.GetComponent<PlayerShield>(); // Giả sử bạn có script PlayerShield
                if (playerShield != null)
                {
                    playerShield.ActivateShield(rewardData.value, rewardData.duration); // value là độ bền khiên, duration là thời gian
                    Debug.Log($"Kích hoạt khiên với độ bền {rewardData.value} trong {rewardData.duration} giây.");
                } else { Debug.LogWarning("Không tìm thấy PlayerShield component trên người chơi."); }
                break;
            */
            // Thêm các case cho các SupplierType khác ở đây
            // case SupplierType.CoinBonus:
            //     if (CoinManager.Instance != null) {
            //         CoinManager.Instance.AddCoins((int)rewardData.value); // Giả sử value là số coin bonus
            //     }
            //     break;

            default:
                Debug.LogWarning($"SupplierManager: Loại kỹ năng chưa được xử lý: {rewardData.type}");
                break;
        }
    }

    public void ApplyFireRateBoost(GameObject player, float multiplier, float duration)
    {
        PlayerShoot shooter = player.GetComponent<PlayerShoot>(); // Giả sử bạn có script PlayerShoot
        if (shooter != null)
        {
            shooter.ApplyFireRateBoost(multiplier, duration); // Giả sử PlayerShoot có hàm này
        }
        else
        {
            Debug.LogWarning("Không tìm thấy PlayerShoot component trên người chơi để áp dụng FireRateBoost.");
        }

        if (fireRateFXPrefab != null)
        {
            GameObject fxInstance = Instantiate(fireRateFXPrefab, player.transform.position, Quaternion.identity, player.transform);
            Destroy(fxInstance, duration); // FX tự hủy theo thời gian buff
        }
    }

    // Hàm để UI Controller lấy danh sách tất cả các kỹ năng (bao gồm trạng thái mở khóa)
    public List<SupplierData> GetAllAvailableRewards()
    {
        // Đảm bảo trạng thái isCurrentlyUnlocked là mới nhất từ PlayerPrefs
        // Mặc dù LoadUnlockStates trong Awake đã làm, nhưng gọi lại ở đây để chắc chắn nếu cần
        // Tuy nhiên, vì isCurrentlyUnlocked được cập nhật trong LoadUnlockStates và UnlockSupply,
        // nên không cần load lại ở đây mỗi lần gọi.
        // foreach (var reward in availableRewards) {
        //    int defaultUnlockState = reward.unlockedByDefault ? 1 : 0;
        //    reward.isCurrentlyUnlocked = PlayerPrefs.GetInt(SUPPLY_UNLOCKED_PREFIX_PP + reward.supplierName, defaultUnlockState) == 1;
        // }
        return availableRewards;
    }
}