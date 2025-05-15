using UnityEngine;

public class BulletController : MonoBehaviour
{
    public float lifeTime = 8f;

    void Start()
    {
        Destroy(gameObject, lifeTime); // tự hủy sau 8 giây
    }

    void OnCollisionEnter(Collision collision)
    {
         if (collision.gameObject.CompareTag("Environment"))
         {
             Destroy(gameObject);
         }
    }
}