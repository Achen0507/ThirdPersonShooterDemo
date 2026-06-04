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
    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null && shootSound != null)
            audioSource = gameObject.AddComponent<AudioSource>();

        UpdateAmmoUI();
    }

    void Update()
    {
        if (Input.GetButtonDown("Fire1") && Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
        }

        // 按 R 换弹
        if (Input.GetKeyDown(KeyCode.R))
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

        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
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
        currentAmmo = maxAmmo;
        UpdateAmmoUI();
        Debug.Log("换弹完成，弹药: " + currentAmmo);
    }

    void UpdateAmmoUI() {
        if (ammoText != null)
        {
            ammoText.text = "弹药: " + currentAmmo + " / " + maxAmmo;
        }
    }
}
