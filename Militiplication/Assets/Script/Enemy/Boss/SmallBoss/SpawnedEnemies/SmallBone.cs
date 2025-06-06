using UnityEngine;

public class SmallBone : MonoBehaviour
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
            transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isDead) return;

        if (other.CompareTag("Bullet"))
        {
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
