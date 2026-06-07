using UnityEngine;

public class DirectMovement : MonoBehaviour
{
    [Header("移动速度")]
    public float walkSpeed = 5f;
    public float aimWalkSpeed = 2f;  //举枪时
    //public float runSpeed = 8f;

    [Header("相机参考")]
    public Transform cameraTransform;     

    [Header("角色旋转平滑度")]
    public float rotationSmoothTime = 0.1f;

    [Header("跳跃")]
    public float jumpHeight = 1.5f;
    public float gravity = -9.81f;

    [Header("地面检测")]
    public GroundChecker groundChecker;

    private CharacterController controller;
    private Animator anim;
    private float currentSpeed;
    private Vector3 velocity;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        if (controller == null)
            controller = gameObject.AddComponent<CharacterController>();

        anim = GetComponent<Animator>();

        controller.height = 1.8f;
        controller.center = new Vector3(0, 0.9f, 0);
        controller.radius = 0.3f;

        currentSpeed = walkSpeed;
    }

    void Update()
    {
        if (cameraTransform == null) return;

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        float targetSpeed = walkSpeed;

        if (Input.GetButton("Fire2"))  
        {
            targetSpeed = aimWalkSpeed;
            anim.SetBool("isAiming", true);
        }
        else
        {
            anim.SetBool("isAiming", false);
        }
        currentSpeed = targetSpeed;

        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;
        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        Vector3 moveDirection = forward * vertical + right * horizontal;

        float hasInput = (Mathf.Abs(horizontal) > 0.1f || Mathf.Abs(vertical) > 0.1f) ? 1f : 0f;
        anim.SetFloat("Speed", hasInput);

        // 让角色朝向移动方向（而不是相机方向）
        if (moveDirection.magnitude > 0.1f)
        {
            // 计算移动方向的角度
            float targetAngle = Mathf.Atan2(moveDirection.x, moveDirection.z) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.Euler(0, targetAngle, 0);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSmoothTime * 10f);
        }

        controller.Move(moveDirection * currentSpeed * Time.deltaTime);  

        bool isGrounded = groundChecker.IsGrounded;

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            anim.SetTrigger("Jump");
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}
