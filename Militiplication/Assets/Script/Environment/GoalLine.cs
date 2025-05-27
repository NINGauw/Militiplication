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
            GameManager.Instance.OnEnemyDefeated();

            UpdateHealthUI();

            if (health <= 0)
            {
                Lose();
            }
        }
        if (other.CompareTag("Boss"))
        {
            // Hủy quái
            Destroy(other.gameObject);
            GameManager.Instance.OnEnemyDefeated();
            //Lose immediately
            Lose();
        }
        if (other.CompareTag("SpawnedEnemy"))
        {
            // Giảm máu
            health--;

            // Hủy quái
            Destroy(other.gameObject);
            UpdateHealthUI();

            if (health <= 0)
            {
                Lose();
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
    private void Lose() {
        //xu ly khi thua
    }
}
