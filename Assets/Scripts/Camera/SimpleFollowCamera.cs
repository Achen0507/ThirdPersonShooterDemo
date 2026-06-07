using UnityEngine;

public class SimpleFollowCamera : MonoBehaviour
{
    [Header("跟随目标")]
    public Transform target;

    [Header("常规相机参数")]
    public float normalDistance = 5f;   
    public float normalHeight = 1.5f;
    public float horizontalOffset = 0.6f;
    public float smoothSpeed = 10f;

    [Header("瞄准模式")]
    public float aimDistance = 1.5f;      // 瞄准时拉近
    public float aimHeight = 1.8f;        // 瞄准时抬高一点
    public bool useFirstPersonAim = false; // 是否切换到第一视角瞄准

    [Header("平滑参数")]
    public float distanceSmoothSpeed = 8f;
    public float pitchSmoothSpeed = 15f;   

    [Header("鼠标控制")]
    public float mouseSensitivityX = 2f;   
    public float mouseSensitivityY = 1f;  
    public float minPitch = -30f;
    public float maxPitch = 60f;

    [Header("角色旋转")]
    public bool rotateCharacter = false;    
    public float characterRotateSpeed = 15f;
    public float backwardRotateThreshold = 0.3f;

    [Header("碰撞检测")]
    public LayerMask collisionLayers = -1;
    public float collisionRadius = 0.2f;
    public float collisionBuffer = 0.2f;

    [Header("动态 FOV")]
    public bool useDynamicFOV = true;
    public float normalFOV = 60f;
    public float aimFOV = 40f;
    public float fovSmoothSpeed = 5f;

    private float currentYaw = 0f;
    private float currentPitch = 20f;
    private float targetPitch = 20f;
    private Vector3 currentVelocity;
    private float currentDistance;
    private float targetDistance;
    private float currentHeight;
    private float targetHeight;

    private bool isAiming = false;
    private bool isBackwardMoving = false;  // 追踪后退状态，防止持续转身

    private Camera mainCamera;
    private Animator characterAnimator;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        mainCamera = GetComponent<Camera>();
        if (mainCamera == null)
            mainCamera = Camera.main;

        if (target != null)
        {
            characterAnimator = target.GetComponent<Animator>();
            currentYaw = target.eulerAngles.y;
            currentDistance = normalDistance;
            targetDistance = normalDistance;
            currentHeight = normalHeight;
            targetHeight = normalHeight;
        }
    }

    void LateUpdate()
    {
        if (target == null) return;

        isAiming = characterAnimator != null && characterAnimator.GetBool("isAiming");

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivityX;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivityY;

        currentYaw += mouseX;
        targetPitch -= mouseY;
        targetPitch = Mathf.Clamp(targetPitch, minPitch, maxPitch);

        currentPitch = Mathf.Lerp(currentPitch, targetPitch, Time.deltaTime * pitchSmoothSpeed);

        HandleBackwardRotation();

        // 角色旋转
        if (rotateCharacter)
        {
            Quaternion targetRotation = Quaternion.Euler(0, currentYaw, 0);
            target.rotation = Quaternion.Slerp(target.rotation, targetRotation, characterRotateSpeed * Time.deltaTime);
        }

        CalculateTargetDistanceAndHeight();

        currentDistance = Mathf.Lerp(currentDistance, targetDistance, Time.deltaTime * distanceSmoothSpeed);
        currentHeight = Mathf.Lerp(currentHeight, targetHeight, Time.deltaTime * distanceSmoothSpeed);

        Quaternion rotation = Quaternion.Euler(currentPitch, currentYaw, 0);
        Vector3 idealPosition = target.position + rotation * new Vector3(horizontalOffset, currentHeight, -currentDistance);

        CheckCollisionAndAdjust(ref idealPosition, rotation);

        transform.position = Vector3.SmoothDamp(transform.position, idealPosition, ref currentVelocity, 1f / smoothSpeed);

        transform.rotation = rotation;

        UpdateDynamicFOV();
    }

    /// <summary>
    /// 处理后退时的强制转身逻辑
    /// </summary>
    private void HandleBackwardRotation()
    {
        if (!rotateCharacter) return;

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        bool shouldMoveBackward = vertical < -backwardRotateThreshold;

        if (shouldMoveBackward && !isBackwardMoving)
        {
            isBackwardMoving = true;
        }
        else if (!shouldMoveBackward && isBackwardMoving)
        {
            // 从后退转为前进，状态复位
            isBackwardMoving = false;
        }
    }

    /// <summary>
    /// 根据瞄准状态计算目标距离和高度
    /// </summary>
    private void CalculateTargetDistanceAndHeight()
    {
        if (isAiming)
        {
            targetDistance = aimDistance;
            targetHeight = aimHeight;
        }
        else
        {
            targetDistance = normalDistance;
            targetHeight = normalHeight;
        }
    }

    /// <summary>
    /// 检测碰撞并平滑调整相机位置
    /// </summary>
    private void CheckCollisionAndAdjust(ref Vector3 idealPosition, Quaternion rotation)
    {
        Vector3 lookDirection = (idealPosition - target.position).normalized;
        float desiredDistance = currentDistance;

        if (Physics.SphereCast(
            target.position + Vector3.up * normalHeight,
            collisionRadius,
            lookDirection,
            out RaycastHit hit,
            desiredDistance,
            collisionLayers))
        {
            float safeDistance = Mathf.Max(hit.distance - collisionBuffer, 0.5f);
            safeDistance = Mathf.Min(safeDistance, desiredDistance);

            float smoothedDistance = Mathf.Lerp(currentDistance, safeDistance, Time.deltaTime * distanceSmoothSpeed);
            idealPosition = target.position + rotation * new Vector3(horizontalOffset, normalHeight, -smoothedDistance);
        }
    }

    /// <summary>
    /// 动态调整 FOV
    /// </summary>
    private void UpdateDynamicFOV()
    {
        if (!useDynamicFOV || mainCamera == null) return;

        float targetFOV = isAiming ? aimFOV : normalFOV;
        mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView, targetFOV, Time.deltaTime * fovSmoothSpeed);
    }

    /// <summary>
    /// 外部调用：获取当前瞄准状态
    /// </summary>
    public bool IsAiming => isAiming;
}