using UnityEngine;
using TMPro;

public class SupplyHealth : MonoBehaviour
{
    public int maxHealth = 20;
    private int currentHealth;

    public TextMeshPro healthText; // Gắn UI text hiển thị máu vào đây

    void Start()
    {
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
            UpdateHealthText();

            Destroy(other.gameObject); // Hủy đạn

            if (currentHealth <= 0)
            {
                Debug.Log("Supply destroyed!");
                ActiveSupply();
            }
        }
    }

    private void ActiveSupply(){
        
    }
}