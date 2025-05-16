using UnityEngine;

public enum GateOperator { Add, Multiply }

public class GateModifier : MonoBehaviour
{
    public GateOperator gateOperator = GateOperator.Add;
    public int value = 1;

    public GameObject bulletPrefab; // Gán prefab đạn vào trong Inspector
    public float bulletSpeed = 10f; // Tốc độ đạn mới

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bullet"))
        {
            BulletController bullet = other.GetComponent<BulletController>();

            // Ngăn đạn được tạo ra từ chính gate này lặp lại trigger
            if (bullet != null && !bullet.hasPassedGate && !bullet.isSpawnedFromGate)
            {
                bullet.hasPassedGate = true;

                int newBulletCount = 0;
                if (gateOperator == GateOperator.Add)
                    newBulletCount = value;
                else if (gateOperator == GateOperator.Multiply)
                    newBulletCount = Mathf.Max(0, value - 1);

                if (newBulletCount <= 0) return;

                Vector3 direction = other.transform.forward; // Lấy hướng bay của viên đạn tới cổng
                float spreadAngle = 10f; // Góc lệch giữa các viên đạn mới

                for (int i = 0; i < newBulletCount; i++)
                {
                    // Tính góc lệch cho viên đạn thứ i, sao cho đều nhau theo chiều ngang (trục Y)
                    float angleOffset = spreadAngle * (i - (newBulletCount - 1) / 2f);
                    
                    // Xoay theo hướng đạn gốc + góc lệch
                    Quaternion rotation = Quaternion.LookRotation(direction) * Quaternion.Euler(0, angleOffset, 0);

                    // Spawn viên đạn mới tại vị trí cổng, với rotation đã tính
                    GameObject newBullet = Instantiate(bulletPrefab, transform.position, rotation);

                    // Đánh dấu viên đạn này là do gate spawn ra
                    BulletController newBulletCtrl = newBullet.GetComponent<BulletController>();
                    if (newBulletCtrl != null)
                    {
                        newBulletCtrl.isSpawnedFromGate = true;
                    }

                    Rigidbody rb = newBullet.GetComponent<Rigidbody>();
                    if (rb != null)
                    {
                        // Cho viên đạn bay theo forward hiện tại
                        rb.velocity = newBullet.transform.forward * bulletSpeed;
                    }
                }

                Debug.Log($"Gate spawned {newBulletCount} bullets.");
            }
        }
    }
}
