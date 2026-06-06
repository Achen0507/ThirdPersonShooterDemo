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

    [Header("UI")]
    public Text ammoText;             

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

            // 尝试对击中物体造成伤害
           //EnemyHealth enemy = hit.transform.GetComponent<EnemyHealth>();
           //if (enemy != null)
           //{
           //    enemy.TakeDamage(damage);
           //}
            else
            {
                // 临时：打中其他物体就变红
                Renderer rend = hit.transform.GetComponent<Renderer>();
                if (rend != null)
                {
                    rend.material.color = Color.red;
                }
            }
        }

        if (shootSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(shootSound);
        }
    }

    void Reload() {
        isReloading = true;
        anim.SetTrigger("Reload");
        Debug.Log("开始换弹...");
    }

    public void FinishReload()
    {
        currentAmmo = maxAmmo;
        UpdateAmmoUI();
        isReloading = false;
        Debug.Log("换弹完成，弹药: " + currentAmmo);
    }

    void UpdateAmmoUI() {
        if (ammoText != null)
        {
            ammoText.text = "弹药: " + currentAmmo + " / " + maxAmmo;
        }
    }
}
