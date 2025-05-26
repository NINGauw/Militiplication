using UnityEngine;
using System.Collections;
public class PlayerShoot : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletSpeed = 20f;
    public float fireRate = 0.5f;
    public float shootInterval = 0.5f; // thời gian giữa mỗi lần bắn (giây)

    private float shootTimer = 0f;

    void Update()
    {
        shootTimer += Time.deltaTime;
        if (shootTimer >= fireRate)
        {
            Shoot();
            shootTimer = 0f;
        }
    }

    void Shoot()
    {
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = firePoint.forward * bulletSpeed;
        }
    }
    public void ApplyFireRateBoost(float multiplier, float duration)
    {
        StopCoroutine("ResetFireRate");
        fireRate = shootInterval / multiplier;
        StartCoroutine(ResetFireRate(duration));
    }
    private IEnumerator ResetFireRate(float duration)
{
    yield return new WaitForSeconds(duration);
    fireRate = shootInterval;
}
}