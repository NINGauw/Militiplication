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

    private void Start()
    {
        // Gán sự kiện cho các nút
        startGameButton.onClick.AddListener(OnStartGameClicked);
        settingButton.onClick.AddListener(OnSettingClicked);
        shopButton.onClick.AddListener(OnBackClicked);

        // Mặc định hiển thị main menu
        ShowMainMenu();
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
