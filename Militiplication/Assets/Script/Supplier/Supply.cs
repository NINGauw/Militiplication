using UnityEngine;
using TMPro;

public class SupplyHealth : MonoBehaviour
{
    public int maxHealth = 12;
    public GameObject destroyFXPrefab;
    public Transform locationFX;
    private int currentHealth;

    public TextMeshPro healthText; // Gắn UI text hiển thị máu vào đây

    void Start()
    {
        maxHealth -= UpgradeManager.GetCurrentSupplyHealthReduction();
        currentHealth = maxHealth;
        UpdateHealthText();
    }

    void UpdateHealthText()
    {
        if (healthText != null)
        {
            healthText.text = $"{currentHealth} / {maxHealth}";
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bullet"))
        {
            currentHealth = Mathf.Max(0, currentHealth - 1);

            Destroy(other.gameObject); // Hủy đạn

            if (currentHealth <= 0)
            {
                Debug.Log("Supply destroyed!");
                ActiveSupply();
                currentHealth = maxHealth;
            }
            UpdateHealthText();
        }
    }

    private void ActiveSupply()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        SupplierManager.Instance.ActivateSelectedReward(player);
        if (destroyFXPrefab != null)
        {
            Instantiate(destroyFXPrefab, locationFX.position, Quaternion.identity);
        }
    }
}