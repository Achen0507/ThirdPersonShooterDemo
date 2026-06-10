using UnityEngine;

public class WeaponSwitcher : MonoBehaviour
{
    [Header("武器列表")]
    public WeaponBase[] weapons;

    [Header("共享资源")]
    public GameObject hitEffect;
    public CrosshairController crosshairController;
    public CameraShake cameraShake;

    private int currentIndex = 0;
    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();

        for (int i = 0; i < weapons.Length; i++)
        {
            weapons[i].Initialize(animator, hitEffect, crosshairController, cameraShake);
            weapons[i].gameObject.SetActive(i == currentIndex);
            if (i == currentIndex) weapons[i].OnEquip();
        }
    }

    private void Update()
    {
        WeaponType currentType = weapons[currentIndex].config.type;
        // ========== 根据武器类型决定可用动作 ==========

        if (currentType == WeaponType.Rifle)
        {
            if (Input.GetButton("Fire2"))
            {
                if (!animator.GetBool("isAiming"))
                    animator.SetBool("isAiming", true);
            }
            else
            {
                if (animator.GetBool("isAiming"))
                    animator.SetBool("isAiming", false);
            }
        }
        else
        {
            // 近战武器：强制退出瞄准，且不检测 Fire2 输入
            if (animator.GetBool("isAiming"))
                animator.SetBool("isAiming", false);
        }

        if (Input.GetButtonDown("Fire1"))
        {
            if (currentType == WeaponType.Rifle && animator.GetBool("isAiming"))
            {
                weapons[currentIndex].OnShoot();  // 步枪射击
            }
            else if (currentType == WeaponType.Melee)
            {
                weapons[currentIndex].OnShoot();  // 近战挥刀
            }
        }

        if (Input.GetKeyDown(KeyCode.R) && currentType == WeaponType.Rifle && animator.GetBool("isAiming"))
        {
            weapons[currentIndex].OnReload();
        }

        for (int i = 0; i < weapons.Length; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                SwitchTo(i);
                break;
            }
        }

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0) {
            int newIndex = currentIndex + (scroll > 0 ? 1 : -1);
            if (newIndex >= weapons.Length) newIndex = 0;
            if (newIndex < 0) newIndex = weapons.Length - 1;
            SwitchTo(newIndex);
        }
    }

    void SwitchTo(int index)
    {
        if (index == currentIndex) return;

        weapons[currentIndex].OnUnequip();
        weapons[currentIndex].gameObject.SetActive(false);

        currentIndex = index;

        weapons[currentIndex].gameObject.SetActive(true);
        weapons[currentIndex].OnEquip();

        if (weapons[currentIndex].config.type != WeaponType.Rifle)
        {
            animator.SetBool("isAiming", false);
        }
    }

    public WeaponBase GetCurrentWeapon() {
        return weapons[currentIndex];
    }
}
