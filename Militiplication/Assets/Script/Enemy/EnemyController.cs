using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float moveSpeed = 3f;
    public int maxHealth = 1;
    private Animator animator;
    private bool isDead = false;

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
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
            TakeDamage(1);
        }
    }
    void TakeDamage(int amount)
    {
        if (isDead) return; // Không nhận sát thương nếu đã chết

        maxHealth -= amount;

        if (maxHealth <= 0)
        {
            Die();
        }
    }
    void Die()
    {
        isDead = true;

        if (animator != null)
        {
            animator.SetTrigger("Death"); // Đảm bảo có trigger "Death" trong Animator Controller của Boss
        }
            GameManager.Instance.OnEnemyDefeated();

        if (EnemyManager.Instance != null)
        {
            EnemyManager.Instance.OnEnemyDefeated();
        }
        Collider col = GetComponent<Collider>();
            if (col != null) col.enabled = false;

            Destroy(gameObject, 2f);
    }   
}
