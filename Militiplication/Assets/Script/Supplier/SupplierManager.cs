using System;
using System.Collections.Generic;
using UnityEngine;

public class SupplierManager : MonoBehaviour
{
    public GameObject fireRateFXPrefab;
    public static SupplierManager Instance;
    [Header("Supplier List")]
    public List<SupplierData> availableRewards;

    [Header("Selected supplier")]
    public string selectedRewardName;


    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // Gọi hàm này để chọn phần thưởng
    public void SetSelectedReward(string rewardName)
    {
        selectedRewardName = rewardName;
    }

    // Gọi khi phá hủy supply để áp dụng phần thưởng đã chọn
    public void ActivateSelectedReward(GameObject player)
    {
        SupplierData reward = availableRewards.Find(r => r.supplierName == selectedRewardName);
        if (reward != null)
        {
            ApplyReward(player, reward);
        }
        else
        {
            Debug.LogWarning("No reward found with name: " + selectedRewardName);
        }
    }

    // Áp dụng phần thưởng dựa trên SupplierData
    public void ApplyReward(GameObject player, SupplierData reward)
    {
        switch (reward.type)
        {
            case SupplierType.FireRateBoost:
                ApplyFireRateBoost(player, 2f, 20f); // Có thể dùng reward.value nếu cần tùy chỉnh
                break;

            // Thêm các case khác nếu có
            default:
                Debug.LogWarning("Reward type not handled: " + reward.type);
                break;
        }
    }

    // Hàm áp dụng tăng tốc độ bắn
    public void ApplyFireRateBoost(GameObject player, float multiplier, float duration)
    {
        PlayerShoot shooter = player.GetComponent<PlayerShoot>();
        if (shooter != null)
        {
            shooter.ApplyFireRateBoost(multiplier, duration);
        }
        if (fireRateFXPrefab != null)
        {
            GameObject fx = Instantiate(fireRateFXPrefab, player.transform.position, Quaternion.identity, player.transform);
            Destroy(fx, duration); // FX tự hủy sau thời gian buff
        }
    }
    
}