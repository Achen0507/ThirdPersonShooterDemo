using UnityEngine;

public class SimpleFollowCamera : MonoBehaviour
{
    [Header("跟随目标")]
    public Transform target;

    [Header("相机偏移（不跟随上下旋转）")]
    public float height = 2f;        
    public float distance = 2.5f;  
    public float sideOffset = 0f;    

    [Header("鼠标灵敏度")]
    public float mouseSensitivity = 2f;

    [Header("水平旋转限制")]
    public float minYaw = -140f;
    public float maxYaw = 140f;

    [Header("垂直角度限制")]
    public float minPitch = -35f;
    public float maxPitch = 40f;

    private float yaw = 0f;
    private float pitch = 0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void LateUpdate()
    {
        if (target == null) return;

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        yaw += mouseX;
        pitch -= mouseY;

        yaw = Mathf.Clamp(yaw, minYaw, maxYaw);
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        Quaternion horizontalRotation = Quaternion.Euler(0, yaw, 0);
        Vector3 horizontalOffset = horizontalRotation * new Vector3(sideOffset, 0, -distance);
        transform.position = target.position + horizontalOffset + Vector3.up * height;
        transform.rotation = Quaternion.Euler(pitch, yaw, 0);
    }
}