using UnityEngine;

public class BulletController : MonoBehaviour
{
    public float lifeTime = 8f;
    public bool hasPassedGate = false;
    public bool isSpawnedFromGate = false;
    void Start()
    {
        Destroy(gameObject, lifeTime); // tự hủy sau 8 giây
    }

    void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Destroy(gameObject); // Hủy đạn khi va chạm enemy
        }
        if (collision.gameObject.CompareTag("SpawnedEnemy"))
        {
            Destroy(gameObject); // Hủy đạn khi va chạm spawned enemy
        }
    }
}