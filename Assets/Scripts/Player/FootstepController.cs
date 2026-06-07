using UnityEngine;

public class FootstepController : MonoBehaviour
{
    [Header("脚步声设置")]
    public AudioSource audioSource;

    [Header("脚步声音频")]
    public AudioClip[] walkFootsteps;    // 走路脚步声
    public AudioClip[] runFootsteps;     // 跑步脚步声
    public float footstepVolume = 0.5f;

    [Header("脚步间隔参数")]
    public float walkStepInterval = 0.5f;    // 走路时脚步间隔（秒）
    public float runStepInterval = 0.35f;    // 跑步时脚步间隔
    public float aimWalkStepInterval = 0.6f; // 瞄准时走路间隔

    [Header("地面检测")]
    public LayerMask groundLayer;
    public float groundCheckDistance = 0.1f;

    private CharacterController controller;
    private Animator anim;
    private DirectMovement directMovement;

    private float stepTimer = 0f;
    private bool isMoving = false;
    private bool isGrounded = false;
    private bool isAiming = false;
    private bool isRunning = false;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
        directMovement = GetComponent<DirectMovement>();

        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        audioSource.spatialBlend = 0f;  
    }

    private void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        isMoving = (Mathf.Abs(horizontal) > 0.1f || Mathf.Abs(vertical) > 0.1f);

        isGrounded = CheckGrounded();

        if (anim != null)
        {
            isAiming = anim.GetBool("isAiming");
        }

        if (directMovement != null)
        {
            isRunning = directMovement.IsRunning;
        }

        if (!isMoving || !isGrounded)
        {
            stepTimer = 0f;
            return;
        }

        float currentInterval = GetCurrentStepInterval();

        stepTimer += Time.deltaTime;

        if (stepTimer >= currentInterval)
        {
            PlayFootstep();
            stepTimer = 0f;
        }
    }

    private bool CheckGrounded()
    {
        if (controller == null) return false;
        return controller.isGrounded;
    }

    private float GetCurrentStepInterval()
    {
        if (isAiming)
            return aimWalkStepInterval;

        if (isRunning)
            return runStepInterval;

        return walkStepInterval; 
    }

    private void PlayFootstep()
    {
        if (audioSource == null) return;

        AudioClip[] currentFootsteps = (isRunning && !isAiming) ? runFootsteps : walkFootsteps;

        if (currentFootsteps == null || currentFootsteps.Length == 0)
        {
            Debug.LogWarning("FootstepController: 脚步声数组为空！");
            return;
        }

        AudioClip footstep = currentFootsteps[Random.Range(0, currentFootsteps.Length)];
        audioSource.PlayOneShot(footstep, footstepVolume);
    }
}
