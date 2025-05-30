using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class SupplySelectUIController : MonoBehaviour
{
    [Header("UI References - Gán từ Inspector")]
    public GameObject supplyButtonPrefab;      // Prefab của nút hiển thị một kỹ năng
    public Transform supplyButtonParent;       // Parent để chứa các nút kỹ năng (ví dụ: một Panel với VerticalLayoutGroup)
    public TextMeshProUGUI selectedSupplyNameText; // Hiển thị tên kỹ năng đang chọn
    public TextMeshProUGUI selectedSupplyDescriptionText; // Hiển thị mô tả kỹ năng đang chọn
    public Button backButton;                  // Nút để quay lại Menu

    [Header("Scene Navigation")]
    public string menuSceneName = "MenuScene"; // Tên Scene Menu của bạn

    [Header("Visual Feedback (Tùy chọn)")]
    public Color selectedButtonColor = Color.yellow; // Màu cho nút được chọn
    public Color unlockedButtonColor = Color.white;  // Màu cho nút đã mở khóa (chưa chọn)
    public Color lockedButtonColor = new Color(0.7f, 0.7f, 0.7f, 0.5f); // Màu cho nút bị khóa

    private List<GameObject> instantiatedButtons = new List<GameObject>();

    void Start()
    {
        if (SupplierManager.Instance == null)
        {
            Debug.LogError("SupplySelectUIController: SupplierManager.Instance không tồn tại! Hãy đảm bảo nó đã được khởi tạo (ví dụ: từ MenuScene).");
            if (selectedSupplyNameText != null) selectedSupplyNameText.text = "LỖI: SupplierManager không tìm thấy!";
            SetAllButtonsInteractable(false); // Vô hiệu hóa UI nếu Manager lỗi
            return;
        }

        if (supplyButtonPrefab == null || supplyButtonParent == null)
        {
            Debug.LogError("SupplySelectUIController: SupplyButtonPrefab hoặc SupplyButtonParent chưa được gán trong Inspector!", this);
            return;
        }

        PopulateSupplyButtons();
        UpdateSelectedSupplyInfo();

        if (backButton != null)
        {
            backButton.onClick.AddListener(GoToMenu);
        }
    }

    void PopulateSupplyButtons()
    {
        // Xóa các nút cũ nếu có (quan trọng nếu hàm này được gọi lại để refresh)
        foreach (GameObject btnGO in instantiatedButtons)
        {
            Destroy(btnGO);
        }
        instantiatedButtons.Clear();

        List<SupplierData> allSupplies = SupplierManager.Instance.GetAllAvailableRewards();

        if (allSupplies == null || allSupplies.Count == 0)
        {
            Debug.LogWarning("SupplySelectUIController: Không có kỹ năng nào trong SupplierManager.");
            return;
        }

        foreach (SupplierData supply in allSupplies)
        {
            GameObject buttonInstance = Instantiate(supplyButtonPrefab, supplyButtonParent);
            instantiatedButtons.Add(buttonInstance);
            buttonInstance.name = "Button_" + supply.supplierName;

            // Giả sử prefab của bạn có các component con để hiển thị thông tin
            // Bạn cần điều chỉnh các lệnh Find() này cho khớp với cấu trúc Prefab của bạn
            Image iconImage = buttonInstance.transform.Find("Icon")?.GetComponent<Image>(); // Ví dụ: tìm con tên "Icon"
            TextMeshProUGUI nameText = buttonInstance.transform.Find("NameText")?.GetComponent<TextMeshProUGUI>(); // Ví dụ: tìm con tên "NameText"
            Button buttonComponent = buttonInstance.GetComponent<Button>();
            Image buttonBackgroundImage = buttonInstance.GetComponent<Image>(); // Image của chính Button để đổi màu
            GameObject lockOverlay = buttonInstance.transform.Find("LockOverlay")?.gameObject; // Ví dụ: một Image ổ khóa

            if (nameText != null) nameText.text = supply.displayName;
            if (iconImage != null && supply.icon != null) iconImage.sprite = supply.icon;
            else if (iconImage != null) iconImage.enabled = false; // Ẩn nếu không có icon

            bool isCurrentlySelected = (SupplierManager.Instance.SelectedRewardName == supply.supplierName);

            if (supply.isCurrentlyUnlocked)
            {
                if (buttonComponent != null)
                {
                    buttonComponent.interactable = true;
                    // Tạo biến cục bộ cho closure
                    string currentSupplyName = supply.supplierName;
                    buttonComponent.onClick.AddListener(() => OnSupplyButtonClicked(currentSupplyName));
                }
                if (buttonBackgroundImage != null)
                {
                    buttonBackgroundImage.color = isCurrentlySelected ? selectedButtonColor : unlockedButtonColor;
                }
                if (lockOverlay != null) lockOverlay.SetActive(false);
            }
            else // Bị khóa
            {
                if (buttonComponent != null) buttonComponent.interactable = false;
                if (buttonBackgroundImage != null) buttonBackgroundImage.color = lockedButtonColor;
                if (lockOverlay != null) lockOverlay.SetActive(true);
            }
        }
    }

    void OnSupplyButtonClicked(string supplierName)
    {
        if (SupplierManager.Instance == null) return;

        SupplierManager.Instance.SetSelectedReward(supplierName);
        UpdateSelectedSupplyInfo();
        RefreshButtonVisuals(); // Cập nhật lại màu sắc/trạng thái của tất cả các nút
    }

    void RefreshButtonVisuals()
    {
        // Cách đơn giản nhất là tạo lại toàn bộ nút, nhưng không hiệu quả nếu danh sách dài
        // PopulateSupplyButtons(); 
        // Cách hiệu quả hơn: duyệt qua instantiatedButtons và cập nhật màu sắc/trạng thái của chúng
        string currentSelectedName = SupplierManager.Instance.SelectedRewardName;
        foreach (GameObject buttonGO in instantiatedButtons)
        {
            // Giả sử tên của buttonGO được đặt là "Button_" + supplierName khi tạo
            string buttonSupplyName = buttonGO.name.Replace("Button_", "");
            SupplierData supplyForThisButton = SupplierManager.Instance.GetAllAvailableRewards().Find(s => s.supplierName == buttonSupplyName);

            if (supplyForThisButton != null && supplyForThisButton.isCurrentlyUnlocked)
            {
                Image buttonBackgroundImage = buttonGO.GetComponent<Image>();
                if (buttonBackgroundImage != null)
                {
                    buttonBackgroundImage.color = (currentSelectedName == buttonSupplyName) ? selectedButtonColor : unlockedButtonColor;
                }
            }
            // Các cập nhật khác cho lock overlay không cần thiết ở đây vì trạng thái khóa không đổi khi chỉ chọn
        }
    }

    void UpdateSelectedSupplyInfo()
    {
        if (SupplierManager.Instance == null) return;

        string selectedName = SupplierManager.Instance.SelectedRewardName;
        if (!string.IsNullOrEmpty(selectedName))
        {
            SupplierData selectedData = SupplierManager.Instance.GetAllAvailableRewards().Find(s => s.supplierName == selectedName);
            if (selectedData != null)
            {
                if (selectedSupplyNameText != null) selectedSupplyNameText.text = selectedData.displayName;
                if (selectedSupplyDescriptionText != null) selectedSupplyDescriptionText.text = selectedData.description;
            }
            else // Trường hợp selectedName đã lưu không còn tồn tại hoặc bị lỗi
            {
                if (selectedSupplyNameText != null) selectedSupplyNameText.text = "Chưa chọn";
                if (selectedSupplyDescriptionText != null) selectedSupplyDescriptionText.text = "Vui lòng chọn một kỹ năng.";
            }
        }
        else
        {
            if (selectedSupplyNameText != null) selectedSupplyNameText.text = "Chưa chọn";
            if (selectedSupplyDescriptionText != null) selectedSupplyDescriptionText.text = "Vui lòng chọn một kỹ năng từ danh sách.";
        }
    }
    
    void SetAllButtonsInteractable(bool isInteractable) // Dùng khi có lỗi Manager
    {
        if(supplyButtonParent != null)
        {
            foreach(Transform child in supplyButtonParent)
            {
                Button btn = child.GetComponent<Button>();
                if(btn != null) btn.interactable = isInteractable;
            }
        }
        if(backButton != null) backButton.interactable = isInteractable;
    }

    void GoToMenu()
    {
        SceneManager.LoadScene(menuSceneName);
    }
}