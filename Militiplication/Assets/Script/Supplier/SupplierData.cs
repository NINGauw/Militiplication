using UnityEngine;

public enum SupplierType
{
    FireRateBoost, //Tăng tốc đánh
    Giant,         //Buff đạn to
    Shield,        //Tạo khiên
    CoinBonus      // + coin
}

[System.Serializable]
public class SupplierData
{
    [Tooltip("ID duy nhất cho kỹ năng, dùng cho việc lưu trữ và tham chiếu nội bộ.")]
    public string supplierName; // Ví dụ: "FIRE_RATE_V1", "SHIELD_BASIC"

    [Tooltip("Tên hiển thị cho người chơi trong UI.")]
    public string displayName; // Tên display

    [Tooltip("Mô tả chi tiết về kỹ năng, hiển thị trong UI.")]
    [TextArea(3, 5)]
    public string description;

    [Tooltip("Icon hiển thị cho kỹ năng trong UI.")]
    public Sprite icon;

    [Tooltip("Loại kỹ năng, dùng để xác định logic áp dụng.")]
    public SupplierType type;

    [Tooltip("Kỹ năng này có được mở khóa ngay từ đầu không?")]
    public bool unlockedByDefault = false;

    // Các giá trị tùy chỉnh cho từng loại kỹ năng
    [Tooltip("Giá trị chính của kỹ năng (ví dụ: mức độ tăng tốc bắn, lượng máu hồi, thời gian tồn tại khiên).")]
    public float value; // Ví dụ: 2 (cho 2x tốc độ bắn) hoặc 50 (cho 50 máu)

    [Tooltip("Thời gian hiệu lực của kỹ năng (nếu có, tính bằng giây).")]
    public float duration; // Ví dụ: 10 (cho 10 giây hiệu lực)

    // Trường này không lưu vào PlayerPrefs trực tiếp như một phần của đối tượng này,
    // mà SupplierManager sẽ quản lý trạng thái isUnlocked của nó bằng key riêng.
    [System.NonSerialized]
    public bool isCurrentlyUnlocked = false;
}