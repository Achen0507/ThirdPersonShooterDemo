using UnityEngine;

public enum MovementState
{
    Idle,   
    Walk,   
    Run     
}

public class DirectMovement : MonoBehaviour
{
    [Header("移动速度")]
    public float walkSpeed = 2f;
    public float runSpeed = 4f;          
    public float aimWalkSpeed = 1.5f;

    [Header("相机参考")]
    public Transform cameraTransform;     

    [Header("角色旋转平滑度")]
    public float rotationSmoothTime = 0.1f;

    [Header("跳跃")]
    public float jumpHeight = 1.5f;
    public float gravity = -9.81f;

    [Header("地面检测")]
    public GroundChecker groundChecker;

    [Header("跑步设置")]
    public KeyCode runKey = KeyCode.LeftShift;  
    public float sprintStaminaDrain = 20f;
    public float sprintStaminaRecover = 10f;
    public float maxStamina = 100f;

    private CharacterController controller;
    private Animator anim;
    private float currentSpeed;
    private Vector3 velocity;
    private float currentStamina;
    private MovementState currentMovementState = MovementState.Idle;  // 当前状态

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
        currentStamina = maxStamina;
    }

    void Update()
    {
        if (cameraTransform == null) return;

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        bool hasMovementInput = (Mathf.Abs(horizontal) > 0.1f || Mathf.Abs(vertical) > 0.1f);

        bool isAiming = Input.GetButton("Fire2");
        anim.SetBool("isAiming", isAiming);

        // 更新耐久值
        UpdateStamina();

        MovementState desiredState = CalculateDesiredState(hasMovementInput, isAiming);
        UpdateMovementState(desiredState);
        anim.SetFloat("Speed", currentSpeed);

        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;
        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        Vector3 moveDirection = forward * vertical + right * horizontal;

      //if (moveDirection.magnitude > 0.1f)
      //{
      //    float targetAngle = Mathf.Atan2(moveDirection.x, moveDirection.z) * Mathf.Rad2Deg;
      //    Quaternion targetRotation = Quaternion.Euler(0, targetAngle, 0);
      //    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSmoothTime * 10f);
      //}

        if (hasMovementInput)
        {
            controller.Move(moveDirection * currentSpeed * Time.deltaTime);
        }
        else
        {
            controller.Move(Vector3.zero);
        }

        bool isGrounded = groundChecker.IsGrounded;
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            anim.SetTrigger("Jump");
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

    }

    private MovementState CalculateDesiredState(bool hasMovementInput, bool isAiming)
    {
        if (!hasMovementInput) return MovementState.Idle;
        if (isAiming) return MovementState.Walk;

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        bool isMovingForward = vertical > 0.1f;
        bool isMovingSideways = Mathf.Abs(horizontal) > 0.1f;

        if (!isMovingForward) return MovementState.Walk;

        bool wantToRun = Input.GetKey(runKey);
        bool canRun = currentStamina > 0;

        if (wantToRun && canRun) return MovementState.Run;
        return MovementState.Walk;
    }

    private void UpdateMovementState(MovementState desiredState)
    {
        if (currentMovementState != desiredState)
        {
            currentMovementState = desiredState;
        }

        switch (currentMovementState)
        {
            case MovementState.Idle:
                currentSpeed = 0f;
                break;
            case MovementState.Walk:
                bool isAiming = anim.GetBool("isAiming");
                currentSpeed = isAiming ? aimWalkSpeed : walkSpeed;
                break;
            case MovementState.Run:
                currentSpeed = runSpeed;
                break;
        }
    }

    private void UpdateStamina()
    {
        if (currentMovementState == MovementState.Run)
        {
            currentStamina -= sprintStaminaDrain * Time.deltaTime;
            currentStamina = Mathf.Max(currentStamina, 0);
        }
        else
        {
            currentStamina += sprintStaminaRecover * Time.deltaTime;
            currentStamina = Mathf.Min(currentStamina, maxStamina);
        }
    }

    public float GetStamina() => currentStamina;
    public float GetMaxStamina() => maxStamina;
    public bool IsRunning => currentMovementState == MovementState.Run;
}
