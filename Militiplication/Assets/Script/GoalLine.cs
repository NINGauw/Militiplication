using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GoalLine : MonoBehaviour
{
    public int health = 5; // Số máu ban đầu
    public TextMeshProUGUI healthText; // (tùy chọn) Text hiển thị máu, gắn từ Inspector

    void Start()
    {
        UpdateHealthUI();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            // Giảm máu
            health--;

            // Hủy quái
            Destroy(other.gameObject);

            Debug.Log($"Goal bị quái tấn công! Máu còn lại: {health}");
            UpdateHealthUI();

            if (health <= 0)
            {
                Debug.Log("Vạch đã bị phá hủy!");
                // Xử lý Game Over hoặc hiệu ứng
                // Destroy(gameObject); // nếu muốn xóa vạch
            }
        }
    }

    void UpdateHealthUI()
    {
        if (healthText != null)
        {
            healthText.text = "Goal HP: " + health;
        }
    }
}
