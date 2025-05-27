using UnityEngine;

public class EquipedEnemy : MonoBehaviour
{
    public float moveSpeed = 3f;
    public int maxHealth = 3; 
    private int currentHealth; // Máu hiện tại của enemy
    private Animator animator;
    private bool isDead = false;

    [Header("Hat Settings")]
    public GameObject hatObject;
    public int damageToLoseHat = 2; // Số máu mất đi để trang văng ra
    public float hatFlyOffForce = 5f;   // Lực làm trang bị văng
    public float hatFlyOffTorque = 10f; // Lực xoay làm trang bị văng
    private bool hatIsLost = false;
    private int accumulatedDamage = 0; // Tổng sát thương đã nhận

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        currentHealth = maxHealth; // Khởi tạo máu hiện tại
    }

    void Update()
    {
        if (!isDead)
        {
            transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isDead) return;

        if (other.CompareTag("Bullet"))
        {
            Destroy(other.gameObject); 
            TakeDamage(1);
        }
    }

    void TakeDamage(int amount)
    {
        if (isDead) return;

        currentHealth -= amount;
        accumulatedDamage += amount; // Cộng dồn sát thương đã nhận

        // Kiểm tra để làm văng nón
        if (!hatIsLost && hatObject != null && accumulatedDamage >= damageToLoseHat)
        {
            LoseHat();
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void LoseHat()
    {
        hatIsLost = true;

        // Tách nón ra khỏi enemy (không còn là con của enemy nữa)
        hatObject.transform.SetParent(null);

        // Thêm Rigidbody nếu chưa có để nón có thể chịu tác động vật lý
        Rigidbody hatRb = hatObject.GetComponent<Rigidbody>();
        if (hatRb == null)
        {
            hatRb = hatObject.AddComponent<Rigidbody>();
        }
        hatRb.isKinematic = false; // Đảm bảo nó không phải là kinematic để lực có tác dụng

        // Tạo hướng văng ngẫu nhiên (chủ yếu là lên và ra sau)
        Vector3 forceDirection = (Vector3.up * 0.7f + Random.insideUnitSphere * 0.3f).normalized;
        hatRb.AddForce(forceDirection * hatFlyOffForce, ForceMode.Impulse);

        // Thêm lực xoay ngẫu nhiên
        hatRb.AddTorque(Random.insideUnitSphere * hatFlyOffTorque, ForceMode.Impulse);
        Destroy(hatObject, 5f); //Xóa nón sau 5s
    }

    void Die()
    {
        isDead = true;

        if (animator != null)
        {
            animator.SetTrigger("Death");
        }

        if (GameManager.Instance != null) // Kiểm tra null cho GameManager
        {
            GameManager.Instance.OnEnemyDefeated();
        }
        else
        {
            Debug.LogWarning("GameManager.Instance is null.");
        }


        if (EnemyManager.Instance != null) // Kiểm tra null cho EnemyManager
        {
            EnemyManager.Instance.OnEnemyDefeated();
        }
        else
        {
            Debug.LogWarning("EnemyManager.Instance is null.");
        }


        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = false;

        Destroy(gameObject, 2f); // Hủy enemy sau 2 giây
    }
}