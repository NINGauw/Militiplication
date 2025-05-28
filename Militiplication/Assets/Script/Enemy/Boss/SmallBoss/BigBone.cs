using Unity.VisualScripting; // Bạn có thể xóa dòng này nếu không dùng Visual Scripting cho BigBone
using UnityEngine;
using UnityEngine.UI;

public class BigBone : MonoBehaviour
{
    public float moveSpeed = 3f;
    public int maxHealth = 10;
    private float currentHealth;
    private Slider healthSlider;
    private Animator animator;
    private bool isDead = false;

    // --- THUỘC TÍNH MỚI CHO KỸ NĂNG SPAWN QUÁI ---
    [Header("Special Skill Settings")]
    public GameObject smallMonsterPrefab; // Prefab của quái nhỏ
    public Transform[] spawnPoints;       // Mảng các điểm spawn cho quái nhỏ (nên có ít nhất 2)
    public int healthThresholdForSkill = 5; // Ngưỡng máu để kích hoạt kỹ năng (ví dụ: còn 5 máu)
    public int numberOfMonstersToSpawn = 2; // Số lượng quái nhỏ sẽ spawn
    public GameObject spawnFXPrefab; //hiệu ứng FX
    private bool specialSkillUsed = false;  // Cờ để đảm bảo kỹ năng chỉ dùng 1 lần
    // ---------------------------------------------

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        currentHealth = maxHealth;

        GameObject healthBarObject = GameObject.Find("BossHealthBar");
        if (healthBarObject != null)
        {
            healthSlider = healthBarObject.GetComponent<Slider>();
            if (healthSlider != null)
            {
                healthSlider.maxValue = maxHealth;
                healthSlider.value = currentHealth;
            }
            else
            {
                Debug.LogWarning("Slider component not found on BossHealthBar.", this);
            }
        }
        else
        {
            Debug.LogWarning("BossHealthBar GameObject not found in scene.", this);
        }
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
            Destroy(other.gameObject); // Xóa viên đạn
            TakeDamage(1); // Giả sử mỗi viên đạn gây 1 sát thương
        }
    }

    void TakeDamage(int amount)
    {
        if (isDead) return; // Không nhận sát thương nếu đã chết

        currentHealth -= amount;
        UpdateHealthBar();

        Debug.Log($"Boss BigBone took {amount} damage, current health: {currentHealth}/{maxHealth}", this);

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            // --- LOGIC KÍCH HOẠT KỸ NĂNG ---
            if (!specialSkillUsed && currentHealth <= healthThresholdForSkill)
            {
                UseSpecialSpawnSkill();
                specialSkillUsed = true; // Đánh dấu kỹ năng đã được sử dụng
            }
            // ---------------------------------
            // Optional: bạn có thể cho hiệu ứng bị thương ở đây
        }
    }

    void UpdateHealthBar()
    {
        if (healthSlider != null)
        {
            healthSlider.value = currentHealth;
        }
    }

    void UseSpecialSpawnSkill()
    {
        Debug.Log("BigBone uses special skill: Spawning small monsters!", this);
        if (smallMonsterPrefab == null)
        {
            Debug.LogError("Small Monster Prefab not assigned in BigBone script!", this);
            return;
        }

        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogError("Spawn Points not assigned or empty in BigBone script! Cannot spawn small monsters.", this);
            return;
        }

        // Không bắt buộc phải có FX, nên chỉ cảnh báo nếu không có
        if (spawnFXPrefab == null)
        {
            Debug.LogWarning("Spawn FX Prefab not assigned in BigBone script. Monsters will spawn without FX.", this);
        }

        int spawnedCount = 0;
        for (int i = 0; i < spawnPoints.Length && spawnedCount < numberOfMonstersToSpawn; i++)
        {
            if (spawnPoints[i] != null) // Kiểm tra xem điểm spawn có bị null không
            {
                // Spawn quái nhỏ
                Instantiate(smallMonsterPrefab, spawnPoints[i].position, spawnPoints[i].rotation);
                spawnedCount++;
                Debug.Log($"Spawned small monster {spawnedCount} at {spawnPoints[i].name}", this);

                // Spawn hiệu ứng FX tại cùng vị trí và hướng nếu đã gán Prefab FX
                if (spawnFXPrefab != null)
                {
                    Instantiate(spawnFXPrefab, spawnPoints[i].position, spawnPoints[i].rotation);
                    Debug.Log($"Spawned FX at {spawnPoints[i].name}", this);
                }
            }
            else
            {
                Debug.LogWarning($"Spawn point at index {i} is null. Skipping.", this);
            }
        }

        if (spawnedCount < numberOfMonstersToSpawn)
        {
            Debug.LogWarning($"Tried to spawn {numberOfMonstersToSpawn} monsters, but only {spawnedCount} could be spawned due to limited or null spawn points.", this);
        }
    }

    void Die()
    {
        isDead = true;

        if (animator != null)
        {
            animator.SetTrigger("Death"); // Đảm bảo có trigger "Death" trong Animator Controller của Boss
        }

        // Kiểm tra xem GameManager.Instance có tồn tại không trước khi gọi
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnBossDefeated();
        }
        else
        {
            Debug.LogWarning("GameManager.Instance is null. Cannot call OnBossDefeated.", this);
        }


        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = false;

        if (healthSlider != null && healthSlider.gameObject != null)
        {
            healthSlider.gameObject.SetActive(false); // Ẩn toàn bộ UI
        }

        // Di chuyển logic hủy đối tượng để đảm bảo các hành động khác hoàn tất
        Destroy(gameObject, 2f); // Hủy boss sau 2 giây để animation chết có thể phát
    }
}