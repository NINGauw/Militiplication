using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameUIManager : MonoBehaviour
{
    public GameObject menuPanel;
    public GameObject gameSettingPanel;
    public Button buttonOpenMenu;
    public Button buttonSetting;
    public Button buttonRestart;
    public Button buttonMenu;
    public Button buttonCloseMenu;
    public Button xSettingButton;

    void Start()
    {
        menuPanel.SetActive(false);
        gameSettingPanel.SetActive(false);

        buttonOpenMenu.onClick.AddListener(ToggleMenuPanel);
        buttonSetting.onClick.AddListener(OpenSettingPanel);
        buttonRestart.onClick.AddListener(RestartLevel);
        buttonMenu.onClick.AddListener(ReturnToMainMenu);
        xSettingButton.onClick.AddListener(TurnOffSettingMenu);
        buttonCloseMenu.onClick.AddListener(TurnOffMenu);
    }

    private void TurnOffMenu()
    {
        menuPanel.SetActive(false);
    }

    private void TurnOffSettingMenu()
    {
        gameSettingPanel.SetActive(false);
    }

    void ToggleMenuPanel()
    {
        bool isActive = menuPanel.activeSelf;
        menuPanel.SetActive(!isActive);
        gameSettingPanel.SetActive(false); // Tắt setting khi menu bị đóng
    }

    void OpenSettingPanel()
    {
        gameSettingPanel.SetActive(true);
    }

    void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    void ReturnToMainMenu()
    {
        SceneManager.LoadScene("MenuScene");
    }
}