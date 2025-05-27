using UnityEngine;
using UnityEngine.UI; // Cần thiết nếu bạn dùng Button chuẩn
using UnityEngine.SceneManagement;
using TMPro; // Cần thiết cho TextMeshPro
using System.Collections.Generic; // Nếu bạn muốn dùng List thay vì mảng

// (Tùy chọn) Tạo một struct hoặc class để chứa thông tin chi tiết hơn cho mỗi màn chơi
[System.Serializable]
public class LevelData
{
    public string levelName;      // Tên hiển thị cho người chơi (ví dụ: "Màn 1: Rừng Xanh")
    public string sceneToLoad;    // Tên scene thực tế cần load (ví dụ: "Level_01_Forest")
    public bool isUnlocked = true; // (Mở rộng) Trạng thái mở khóa, mặc định là true
    // Bạn có thể thêm các thông tin khác: hình ảnh thumbnail, sao đạt được, v.v.
}

public class LevelSelectManager : MonoBehaviour
{
    public static LevelSelectManager Instance; // Singleton đơn giản

    [Header("Level Configuration")]
    public List<LevelData> levels; // Danh sách các màn chơi, cấu hình trong Inspector

    [Header("UI Elements")]
    public GameObject levelButtonPrefab;   // Kéo Prefab Nút Màn Chơi vào đây
    public Transform levelButtonParent;    // Kéo Panel chứa các nút (LevelButtonsPanel) vào đây
    public Button backButton;              // Kéo Nút Quay Lại Menu vào đây

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        if (levelButtonPrefab == null || levelButtonParent == null)
        {
            Debug.LogError("LevelButtonPrefab hoặc LevelButtonParent chưa được gán trong LevelSelectManager!", this);
            return;
        }

        PopulateLevelButtons();

        if (backButton != null)
        {
            backButton.onClick.AddListener(GoToMenuScene);
        }
        else
        {
            Debug.LogWarning("BackButton chưa được gán trong LevelSelectManager.", this);
        }
    }

    void PopulateLevelButtons()
    {
        // Xóa các nút cũ (nếu có) trước khi tạo mới, phòng trường hợp gọi lại hàm này
        foreach (Transform child in levelButtonParent)
        {
            Destroy(child.gameObject);
        }

        if (levels == null || levels.Count == 0)
        {
            Debug.LogWarning("Danh sách 'levels' rỗng hoặc chưa được cấu hình.", this);
            return;
        }

        for (int i = 0; i < levels.Count; i++)
        {
            LevelData currentLevel = levels[i]; // Lấy dữ liệu màn chơi hiện tại

            GameObject buttonGO = Instantiate(levelButtonPrefab, levelButtonParent);
            buttonGO.name = "LevelButton_" + currentLevel.levelName; // Đặt tên cho dễ debug

            Button buttonComponent = buttonGO.GetComponent<Button>();
            TextMeshProUGUI buttonText = buttonGO.GetComponentInChildren<TextMeshProUGUI>();

            if (buttonText != null)
            {
                buttonText.text = currentLevel.levelName; // Hiển thị tên màn chơi
            }

            if (buttonComponent != null)
            {
                // (Mở rộng) Kiểm tra trạng thái isUnlocked
                if (currentLevel.isUnlocked)
                {
                    // Tạo một biến cục bộ để closure bắt đúng giá trị
                    string sceneNameToLoad = currentLevel.sceneToLoad;
                    buttonComponent.onClick.AddListener(() => LoadLevel(sceneNameToLoad));
                }
                else
                {
                    buttonComponent.interactable = false; // Vô hiệu hóa nút nếu màn chơi bị khóa
                    if (buttonText != null)
                    {
                        buttonText.text += "lock"; // Thêm icon (Locked)
                    }
                }
            }
        }
    }

    public void LoadLevel(string sceneName)
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogError("Tên scene để load không hợp lệ!", this);
            return;
        }
        Debug.Log($"Đang tải màn chơi: {sceneName}");
        SceneManager.LoadScene(sceneName);
    }

    public void GoToMenuScene()
    {
        Debug.Log("Quay lại Menu Scene...");
        SceneManager.LoadScene("MenuScene"); // Thay "MenuScene" bằng tên Scene Menu của bạn
    }

    // (Mở rộng) Hàm để mở khóa màn chơi (ví dụ, được gọi khi hoàn thành màn trước đó)
    public void UnlockLevel(string sceneNameToUnlock)
    {
        LevelData levelToUnlock = levels.Find(level => level.sceneToLoad == sceneNameToUnlock);
        if (levelToUnlock != null)
        {
            levelToUnlock.isUnlocked = true;
            // (Mở rộng hơn nữa) Lưu trạng thái mở khóa này vào PlayerPrefs để không bị mất khi tắt game
            // PlayerPrefs.SetInt("LevelUnlocked_" + sceneNameToUnlock, 1);
            // PlayerPrefs.Save();
            Debug.Log($"Màn chơi {sceneNameToUnlock} đã được mở khóa!");

            // Refresh lại UI nút bấm để cập nhật trạng thái (nếu cần)
            // PopulateLevelButtons(); // Sẽ xóa và tạo lại các nút
        }
    }

    // (Mở rộng) Kiểm tra trạng thái mở khóa từ PlayerPrefs khi bắt đầu
    void LoadUnlockStates()
    {
        // foreach (LevelData level in levels)
        // {
        //     if (PlayerPrefs.HasKey("LevelUnlocked_" + level.sceneToLoad))
        //     {
        //         level.isUnlocked = PlayerPrefs.GetInt("LevelUnlocked_" + level.sceneToLoad) == 1;
        //     }
        //     // Màn đầu tiên thường được mở khóa sẵn, hoặc bạn có thể set isUnlocked = true cho nó trong Inspector
        // }
    }
}