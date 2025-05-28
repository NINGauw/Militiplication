using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    private Animator animator;
    private CharacterController controller;

    void Start()
    {
        animator = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
        moveSpeed = UpgradeManager.GetCurrentMoveSpeedValue();
    }

    void Update()
    {
        float moveX = Input.GetAxisRaw("Horizontal"); // A/D hoặc Left/Right arrow

        // Di chuyển sang trái/phải
        Vector3 move = new Vector3(moveX, 0, 0); // Di chuyển ngang
        controller.Move(move * moveSpeed * Time.deltaTime);

        // Gửi thông số vào Animator
        animator.SetFloat("moveX", moveX);
    }
}