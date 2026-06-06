using UnityEngine;

public class SimpleFollowCamera : MonoBehaviour
{
    [Header("跟随目标")]
    public Transform target;

    [Header("相机参数")]
    public float distance = 3f;
    public float height = 1.5f;
    public float smoothSpeed = 10f;
    public float horizontalOffset = 0.6f; 

    [Header("鼠标控制")]
    public float mouseSensitivityX = 2f;   // 水平
    public float mouseSensitivityY = 1f; // 垂直
    public float minYaw = -360f;
    public float maxYaw = 360f;
    public float minPitch = -30f;
    public float maxPitch = 60f;

    [Header("角色旋转")]
    public bool rotateCharacter = true;      
    public float characterRotateSpeed = 15f; // 角色旋转平滑速度

    [Header("碰撞检测")]
    public LayerMask collisionLayers = -1;
    public float collisionRadius = 0.2f;

    private float currentYaw = 0f;
    private float currentPitch = 20f;
    private Vector3 currentVelocity;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (target != null)
        {
            currentYaw = target.eulerAngles.y;
        }
    }

    void LateUpdate()
    {
        if (target == null) return;

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivityX;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivityY;

        currentYaw += mouseX;
        currentPitch -= mouseY;
        currentPitch = Mathf.Clamp(currentPitch, minPitch, maxPitch);

        if (rotateCharacter)
        {
            Quaternion targetRotation = Quaternion.Euler(0, currentYaw, 0);
            target.rotation = Quaternion.Slerp(target.rotation, targetRotation, characterRotateSpeed * Time.deltaTime);
        }

        Quaternion rotation = Quaternion.Euler(currentPitch, currentYaw, 0);
        Vector3 idealPosition = target.position + rotation * new Vector3(horizontalOffset, height, -distance);

        Vector3 lookDirection = idealPosition - target.position;
        float actualDistance = distance;

        if (Physics.SphereCast(target.position + Vector3.up * height, collisionRadius, lookDirection, out RaycastHit hit, distance, collisionLayers))
        {
            actualDistance = Mathf.Clamp(hit.distance - 0.2f, 0.5f, distance);
            idealPosition = target.position + rotation * new Vector3(0, height, -actualDistance);
        }

        transform.position = Vector3.SmoothDamp(transform.position, idealPosition, ref currentVelocity, 1f / smoothSpeed);

        transform.rotation = rotation;
    }
}