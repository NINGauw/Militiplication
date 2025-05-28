using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public GameObject mainMenuPanel;
    public GameObject settingPanel;

    public Button startGameButton;
    public Button settingButton;
    public Button shopButton;
    public Button upgradeStat;
    public Button supply;

    private void Start()
    {
        // Gán sự kiện cho các nút
        startGameButton.onClick.AddListener(OnStartGameClicked);
        settingButton.onClick.AddListener(OnSettingClicked);
        shopButton.onClick.AddListener(OnBackClicked);
        upgradeStat.onClick.AddListener(OnUpgradeStatClicked);
        supply.onClick.AddListener(OnSupplyClicked);

        // Mặc định hiển thị main menu
        ShowMainMenu();
    }

    private void OnSupplyClicked()
    {
        SceneManager.LoadScene("Supply");
    }

    private void OnUpgradeStatClicked()
    {
        SceneManager.LoadScene("UpgradeStat");
    }

    void OnStartGameClicked()
    {
        // Load scene chơi game
        SceneManager.LoadScene("LevelSelectScene");
    }

    void OnSettingClicked()
    {
        // Hiện panel setting
        mainMenuPanel.SetActive(false);
        settingPanel.SetActive(true);
    }

    void OnBackClicked()
    {
        // Trở lại menu chính
        ShowMainMenu();
    }

    void ShowMainMenu()
    {
        mainMenuPanel.SetActive(true);
        settingPanel.SetActive(false);
    }
}
