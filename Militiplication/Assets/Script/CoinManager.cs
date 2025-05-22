using UnityEngine;
using TMPro;

public class CoinManager : MonoBehaviour
{
    public static CoinManager Instance;
    public int currentCoins = 0;
    public TextMeshProUGUI coinText;

    private const string COIN_KEY = "TotalCoins";

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Nếu bạn muốn giữ giữa scene
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        LoadCoins();
    }

    public void AddCoins(int amount)
    {
        currentCoins += amount;
        SaveCoins();
    }

    public bool SpendCoins(int amount)
    {
        if (currentCoins >= amount)
        {
            currentCoins -= amount;
            SaveCoins();
            return true;
        }

        return false;
    }

    void SaveCoins()
    {
        PlayerPrefs.SetInt(COIN_KEY, currentCoins);
        PlayerPrefs.Save();
    }

    void LoadCoins()
    {
        currentCoins = PlayerPrefs.GetInt(COIN_KEY, 0);
    }

    public int GetCoinAmount()
    {
        return currentCoins;
    }
}