using UnityEngine;

public class ObjectMover : MonoBehaviour
{
    [Header("Object to Move")]
    public Transform targetObject;

    [Header("Movement Points (Only X is used)")]
    public Transform leftPoint;
    public Transform rightPoint;

    [Header("Movement Settings")]
    public float moveSpeed = 2f;

    private float targetX;

    void Start()
    {
        if (targetObject == null || leftPoint == null || rightPoint == null)
        {
            Debug.LogError("Missing references in ObjectMover!");
            enabled = false;
            return;
        }

        targetX = rightPoint.position.x;
    }

    void Update()
    {
        if (targetObject == null) return;

        Vector3 currentPos = targetObject.position;
        float newX = Mathf.MoveTowards(currentPos.x, targetX, moveSpeed * Time.deltaTime);
        targetObject.position = new Vector3(newX, currentPos.y, currentPos.z);

        // Khi đã đến gần điểm đích trên trục X
        if (Mathf.Abs(newX - targetX) < 0.05f)
        {
            targetX = (Mathf.Approximately(targetX, leftPoint.position.x))
                ? rightPoint.position.x
                : leftPoint.position.x;
        }
    }
}

