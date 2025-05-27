using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class LoseUIManager : MonoBehaviour
{
    public static LoseUIManager Instance { get; private set; }

    public GameObject losePanel;          // Panel Lose
    public float delayBeforeShow = 1.0f;  // Thời gian chờ trước khi hiện Panel Lose

    [Header("UI Buttons")]
    public Button buttonRestartStageLose; // Nút Chơi Lại
    public Button buttonBackMenuLose;     // Nút Quay Lại Menu

    private bool hasTriggeredLose = false;

    void Awake() // Sử dụng Awake để khởi tạo Singleton sớm
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject); // Hủy các bản sao thừa, đảm bảo chỉ có 1 Instance
            return;
        }
    }

    void Start()
    {
        if (losePanel != null)
        {
            losePanel.SetActive(false); // Ẩn Panel Lose khi bắt đầu
        }
        else
        {
            Debug.LogError("LosePanel chưa được gán trong LoseUIManager!", this);
        }

        // Gán sự kiện cho các nút
        if (buttonRestartStageLose != null)
        {
            buttonRestartStageLose.onClick.AddListener(RestartCurrentStage);
        }
        else
        {
            Debug.LogWarning("Button RestartStageLose chưa được gán trong LoseUIManager.", this);
        }

        if (buttonBackMenuLose != null)
        {
            buttonBackMenuLose.onClick.AddListener(GoToMenuScene);
        }
        else
        {
            Debug.LogWarning("Button BackMenuLose chưa được gán trong LoseUIManager.", this);
        }
    }

    private void RestartCurrentStage()
    {
        Time.timeScale = 1f; // Đảm bảo game chạy lại bình thường
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void GoToMenuScene()
    {
        Time.timeScale = 1f; // Đảm bảo game chạy lại bình thường
        SceneManager.LoadScene("MenuScene"); // Thay "MenuScene" bằng tên Scene Menu của bạn
    }

    public void TriggerLose()
    {
        if (hasTriggeredLose) return; // Nếu đã kích hoạt thì không làm gì nữa
        hasTriggeredLose = true;

        Invoke(nameof(ShowLosePanel), delayBeforeShow);
    }

    private void ShowLosePanel()
    {
        if (losePanel != null)
        {
            losePanel.SetActive(true);
            Time.timeScale = 0f; // Có thể dừng game ở đây nếu muốn
        }
    }
}