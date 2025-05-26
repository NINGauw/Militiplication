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
       
    }

    public void OnEnemyDefeated()
    {
        enemiesRemaining--;
        UpdateEnemyCountUI();

        if (enemiesRemaining <= 0)
        {
            Debug.Log("🎉 You Win!");
            WinUIManager.Instance.TriggerWin();
            CoinManager.Instance.AddCoins(5);
        }
    }

    private void UpdateEnemyCountUI()
    {
        enemiesLeftText.text = $"Enemies left: {enemiesRemaining}";
    }
}
