using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CoinUIManager : MonoBehaviour
{
    public TextMeshProUGUI coinText;
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        UpdateCoinDisplay(); // Cập nhật ngay khi bật UI
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        UpdateCoinDisplay(); // Cập nhật lại khi load scene
    }

    public void UpdateCoinDisplay()
    {
        if (CoinManager.Instance != null)
        {
            coinText.text = $"{CoinManager.Instance.GetCoinAmount()}";
        }
    }
}