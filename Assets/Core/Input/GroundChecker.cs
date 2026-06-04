using UnityEngine;

public class GroundChecker : MonoBehaviour
{
    [Header("检测设置")]
    [Tooltip("检测半径（略小于角色半径）")]
    public float checkRadius = 0.5f;

    [Tooltip("检测距离（从脚底向下）")]
    public float checkDistance = 0.1f;

    [Tooltip("地面的 Layer")]
    public LayerMask groundLayers = -1;  // -1 表示所有层

    [Header("调试")]
    public bool showDebug = true;

    private CharacterController controller;
    private bool wasGroundedLastFrame = false;

    public bool IsGrounded { get; private set; }

    public bool JustLanded { get; private set; }

    public bool JustLeftGround { get; private set; }

    void Start()
    {
        controller = GetComponent<CharacterController>();
        if (controller == null)
            controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        wasGroundedLastFrame = IsGrounded;

        IsGrounded = CheckGrounded();

        JustLanded = IsGrounded && !wasGroundedLastFrame;
        JustLeftGround = !IsGrounded && wasGroundedLastFrame;
    }

    bool CheckGrounded()
    {
        bool result1 = controller != null && controller.isGrounded;

        Vector3 spherePosition = transform.position + Vector3.down * (controller.height / 2 - 0.05f);
        bool result2 = Physics.CheckSphere(spherePosition, checkRadius, groundLayers);

        return result1 || result2;
    }

    void OnDrawGizmos()
    {
        if (!showDebug) return;
        if (controller == null) controller = GetComponent<CharacterController>();
        if (controller == null) return;

        Vector3 feetPos2 = transform.position;
        feetPos2.y -= 0f;

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(feetPos2, 0.3f);
    }
}
