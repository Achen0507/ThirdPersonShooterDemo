using UnityEngine;
using UnityEngine.UI;

public class PlayerShoot : MonoBehaviour
{
    [Header("射击设置")]
    public float damage = 10f;
    public float range = 100f;
    public float fireRate = 0.2f;      // 射速间隔（秒）

    [Header("命中反馈")]
    public GameObject hitEffect;    
    public AudioClip shootSound;

    public GameObject muzzleFlashPrefab;
    public Transform muzzlePoint;

    [Header("UI")]
    public Text ammoText;

    [Header("枪械模型")]
    public GameObject weaponModel;

    [Header("准星扩散")]
    public CrosshairController crosshairController;

    [Header("相机震动")]
    public CameraShake cameraShake;

    private float nextFireTime = 0f;
    private int currentAmmo = 30;
    private int maxAmmo = 30;
    private bool isReloading = false;
    private AudioSource audioSource;
    private Animator anim;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null && shootSound != null)
            audioSource = gameObject.AddComponent<AudioSource>();

        anim = GetComponent<Animator>();

        if (weaponModel == null)
        {
            weaponModel = GameObject.Find("MGP7");

            if (weaponModel != null)
                Debug.Log("自动找到枪: " + weaponModel.name);
            else
                Debug.LogError("找不到枪！请手动拖拽赋值");
        }

        UpdateAmmoUI();
    }

    void Update()
    {
        if (isReloading) return;

        if (Input.GetButtonDown("Fire1") && Time.time >= nextFireTime && anim.GetBool("isAiming"))
        {
            Shoot();
        }

        //举枪换弹
        if (Input.GetKeyDown(KeyCode.R) && anim.GetBool("isAiming")&& currentAmmo < maxAmmo && !isReloading)
        {
            Reload();
        }
    }

    void Shoot() {
        if (currentAmmo <= 0)
        {
            Debug.Log("没子弹了！按 R 换弹");
            return;
        }

        currentAmmo--;
        UpdateAmmoUI();
        nextFireTime = Time.time + fireRate;
        anim.SetTrigger("Shoot");

        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, range))
        {
            Debug.Log("击中: " + hit.transform.name);

            // 命中特效
            if (hitEffect != null)
            {
                Instantiate(hitEffect, hit.point, Quaternion.LookRotation(hit.normal));
            }

            EnemyHealth enemy = hit.transform.GetComponent<EnemyHealth>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
        }

        if (muzzleFlashPrefab != null && muzzlePoint != null)
        {
            GameObject flash = Instantiate(muzzleFlashPrefab, muzzlePoint.position, muzzlePoint.rotation);
            Destroy(flash, 0.1f); 
        }

        if (cameraShake != null)
        {
            Debug.Log("调用相机震动");
            cameraShake.Shake();
        }

        if (shootSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(shootSound);
        }

        if (crosshairController != null)
        {
            crosshairController.AddShootFeedback();
            crosshairController.AddRecoil();
        }
    }

    void Reload() {
        if (isReloading) return;
        if (currentAmmo >= maxAmmo) return;
        if (!anim.GetBool("isAiming")) return;

        isReloading = true;
        anim.SetTrigger("Reload");

        if (weaponModel != null)
            weaponModel.SetActive(false);

        Debug.Log("开始换弹...");
        StartCoroutine(DelayedReload());
    }

    System.Collections.IEnumerator DelayedReload()
    {
        float reloadTime = 3.6f; 
        yield return new WaitForSeconds(reloadTime);
        FinishReload();
    }

    public void FinishReload()
    {
        currentAmmo = maxAmmo;
        UpdateAmmoUI();
        isReloading = false;

        if (weaponModel != null)
            weaponModel.SetActive(true);
    }

    void UpdateAmmoUI() {
        if (ammoText != null)
        {
            ammoText.text = "弹药: " + currentAmmo + " / " + maxAmmo;
        }
    }
}
