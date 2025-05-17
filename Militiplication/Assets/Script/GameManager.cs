using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public int totalEnemies = 20;
    private int enemiesRemaining;

    public TextMeshProUGUI enemiesLeftText;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        enemiesRemaining = totalEnemies;
        UpdateEnemyCountUI();
    }

    public void OnEnemySpawned()
    {
        // Gọi nếu bạn muốn cập nhật số lượng spawn còn lại (nếu cần)
    }

    public void OnEnemyDefeated()
    {
        enemiesRemaining--;
        UpdateEnemyCountUI();

        if (enemiesRemaining <= 0)
        {
            // Kiểm tra nếu không còn enemy nào trên map
            if (GameObject.FindGameObjectsWithTag("Enemy").Length == 0)
            {
                Debug.Log("🎉 You Win!");
                // Thêm UI hoặc chuyển màn tại đây nếu cần
            }
        }
    }

    private void UpdateEnemyCountUI()
    {
        enemiesLeftText.text = $"Enemies left: {enemiesRemaining}";
    }
}
