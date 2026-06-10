using UnityEngine;

public abstract class WeaponBase : MonoBehaviour
{
    [Header("Œ‰∆˜≈‰÷√")]
    public WeaponConfig config;

    [Header("Œ‰∆˜ƒ£–Õ")]
    public GameObject weaponModel;
    public Transform muzzlePoint;

    [Header("“Ù–ß")]
    public AudioClip shootSound;
    public AudioClip reloadSound;
    public AudioClip equipSound;
    public AudioClip attackSound;

    [Header("UI")]
    public UnityEngine.UI.Text ammoText;

    protected Animator animator;
    protected AudioSource audioSource;
    protected bool isReloading;
    protected int currentAmmo;
    protected float nextFireTime;

    protected GameObject hitEffect;
    protected CrosshairController crosshairController;
    protected CameraShake cameraShake;

    public virtual void Initialize(Animator playerAnimator, GameObject hitEffectPrefab, CrosshairController crosshair, CameraShake shake)
    {
        animator = playerAnimator;
        hitEffect = hitEffectPrefab;
        crosshairController = crosshair;
        cameraShake = shake;

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        currentAmmo = config.maxAmmo;
        UpdateAmmoUI();
    }

    public virtual void OnEquip() {
        if (weaponModel != null) weaponModel.SetActive(true);
        this.enabled = true;
        if (equipSound != null && audioSource != null) audioSource.PlayOneShot(equipSound);
        UpdateAmmoUI();
    }

    public virtual void OnUnequip() {
        if (weaponModel != null) weaponModel.SetActive(false);
        this.enabled = false;
    }

    public virtual void OnUpadate() { }

    public abstract void OnShoot();
    public abstract void OnReload();

    public virtual int GetCurrentAmmo() => currentAmmo;
    public virtual int GetMaxAmmo() => config.maxAmmo;
    public virtual bool IsReloading() => isReloading;
    public WeaponType WeaponType => config.type;
    protected void UpdateAmmoUI()
    {
        if (ammoText != null)
        {
            ammoText.text = "µØ“©: " + currentAmmo + " / " + config.maxAmmo;
        }
    }

    protected void DoShootRaycast()
    {
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        if (Physics.Raycast(ray, out RaycastHit hit, config.range))
        {
            Debug.Log("ª˜÷–: " + hit.transform.name);

            if (hitEffect != null)
            {
                Instantiate(hitEffect, hit.point, Quaternion.LookRotation(hit.normal));
            }

            var damageable = hit.transform.GetComponent<IDamageable>();
            damageable?.TakeDamage(config.damage);
        }
    }

    protected void DoMuzzleFlash()
    {
        if (config.muzzleFlashPrefab != null && muzzlePoint != null)
        {
            GameObject flash = Instantiate(config.muzzleFlashPrefab, muzzlePoint.position, muzzlePoint.rotation);
            Destroy(flash, 0.1f);
        }
    }

    protected void DoCameraShake()
    {
        if (cameraShake != null)
        {
            cameraShake.Shake();
        }
    }

    protected void DoCrosshairFeedback()
    {
        if (crosshairController != null)
        {
            crosshairController.AddShootFeedback();
            crosshairController.AddRecoil();
        }
    }
}
