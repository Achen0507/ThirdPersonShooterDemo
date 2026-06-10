using UnityEngine;

public class Rifle : WeaponBase
{
    private void Update()
    {
        if (isReloading) return;
    }

    public override void OnShoot()
    {
        if (isReloading) return;
        if (currentAmmo <= 0) {
            Debug.Log("没子弹了！按 R 换弹");
            return;
        }

        if (Time.time < nextFireTime) return;
        currentAmmo--;
        UpdateAmmoUI();
        nextFireTime = Time.time + config.attackCooldown;

        animator?.SetTrigger("Shoot");

        // 枪口特效
        DoMuzzleFlash();

        // 相机震动
        DoCameraShake();

        // 准星反馈
        DoCrosshairFeedback();

        // 射线检测
        DoShootRaycast();

        // 射击音效
        if (shootSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(shootSound);
        }
    }

    public override void OnReload()
    {
        if (isReloading) return;
        if (currentAmmo >= config.maxAmmo) return;

        isReloading = true;
        animator?.SetTrigger("Reload");

        if (weaponModel != null)
            weaponModel.SetActive(false);

        if (reloadSound != null && audioSource != null)
            audioSource.PlayOneShot(reloadSound);

        Debug.Log("开始换弹...");
    }

    public void FinishReload()
    {
        currentAmmo = config.maxAmmo;
        UpdateAmmoUI();
        isReloading = false;

        if (weaponModel != null)
            weaponModel.SetActive(true);

        Debug.Log("换弹完成，弹药: " + currentAmmo);
    }
}
