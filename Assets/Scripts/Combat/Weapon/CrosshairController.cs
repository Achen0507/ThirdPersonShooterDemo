using UnityEngine;

public class CrosshairController : MonoBehaviour
{
    [Header("准星UI")]
    public RectTransform crosshair;

    [Header("基础大小")]
    public float normalSize = 256f;    
    public float aimingSize = 180f;      // 举枪时的大小

    [Header("走路振幅")]
    public float walkAmplitude = 20f;   
    public float walkSpeed = 5f;          // 振幅频率

    [Header("射击反馈")]
    public float shootAmplitude = 60f;   
    public float shootRecoverSpeed = 10f; // 射击后恢复速度

    [Header("后坐力")]
    public float recoilAmount = 30f;   
    public float recoilRecoverSpeed = 15f; // 恢复速度

    private float shootOffset = 0f;       // 射击偏移
    private float currentRecoil = 0f;
    private Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();
        if (crosshair == null)
        {
            Debug.LogWarning("CrosshairController: 未指定准星！");
        }
    }

    private void Update()
    {
        if (crosshair == null) return;

        currentRecoil = Mathf.Lerp(currentRecoil, 0, Time.deltaTime * recoilRecoverSpeed);

        bool isAiming = anim != null && anim.GetBool("isAiming");
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        bool isWalking = (Mathf.Abs(horizontal) > 0.1f || Mathf.Abs(vertical) > 0.1f);

        float baseSize = isAiming ? aimingSize : normalSize;

        float walkOffset = 0f;
        if (isWalking && !isAiming)
        {

            walkOffset = Mathf.Sin(Time.time * walkSpeed) * walkAmplitude;
            walkOffset = Mathf.Abs(walkOffset);
        }

        shootOffset = Mathf.Lerp(shootOffset, 0, Time.deltaTime * shootRecoverSpeed);

        float finalSize = baseSize + walkOffset + shootOffset + currentRecoil;
        finalSize = Mathf.Clamp(finalSize, aimingSize, normalSize + walkAmplitude + shootAmplitude + recoilAmount);
        crosshair.sizeDelta = new Vector2(finalSize, finalSize);
    }

    public void AddShootFeedback()
    {
        shootOffset = shootAmplitude;
    }

    public void AddRecoil()
    {
        currentRecoil = recoilAmount;
    }
}
