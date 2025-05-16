using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float moveSpeed = 3f;
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
            transform.Translate(transform.forward * moveSpeed * Time.deltaTime, Space.World);
        }
    }

    private void OnTriggerEnter(Collider other)
{
    if (isDead) return;

    if (other.CompareTag("Bullet"))
    {
          Debug.Log("Enemy trigger entered by: " + other.gameObject.name);
        isDead = true;

        if (animator != null)
        {
            animator.SetTrigger("Death");
        }

        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = false;

        Destroy(gameObject, 2f);
    }
}

}