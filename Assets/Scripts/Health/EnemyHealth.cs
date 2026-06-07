using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
    [Header("生命值")]
    public float maxHealth = 30f;
    private float currentHealth;

    [Header("受伤反馈")]
    public GameObject hitEffect;      // 受击特效
    public AudioClip hitSound;        // 受击音效

    [Header("血条UI")]
    public Slider healthBar;     

    [Header("死亡")]
    public GameObject deathEffect;    
    public float destroyDelay = 2f;   // 死亡后几秒消失

    [Header("移动")]
    public float moveSpeed = 2f;
    public Transform player;          
    private AudioSource audioSource;

    private void Start()
    {
        currentHealth = maxHealth;
        audioSource = GetComponent<AudioSource>();

        if (audioSource == null && hitSound != null)
            audioSource = gameObject.AddComponent<AudioSource>();

        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
        }

        UpdateHealthBar();
    }

    private void Update()
    {
        if (currentHealth > 0 && player != null) {
            Vector3 direction = (player.position - transform.position).normalized;
            direction.y = 0;  // 水平移动
            transform.position += direction * moveSpeed * Time.deltaTime;

            if (direction != Vector3.zero) {
                transform.forward = direction;
            }
        }

#if UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.K))
            {
                EditorApplication.isPaused = !EditorApplication.isPaused;
                Debug.Log("游戏已" + (EditorApplication.isPaused ? "暂停" : "恢复"));
            }
#endif
    }

    public void TakeDamage(float damage) {
        if (currentHealth <= 0) return;

        currentHealth -= damage;
        UpdateHealthBar();

        if (hitEffect != null)
        {
            Instantiate(hitEffect, transform.position + Vector3.up * 1f, Quaternion.identity);
        }

        if (hitSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(hitSound);
        }

        StartCoroutine(FlashRed());

        if (currentHealth <= 0) {
            Die();
        }
    }

    void UpdateHealthBar()
    {
        if (healthBar != null)
        {
            healthBar.value = currentHealth / maxHealth;
        }
    }

    System.Collections.IEnumerator FlashRed()
    {
        Renderer rend = GetComponent<Renderer>();
        if (rend != null)
        {
            Color original = rend.material.color;
            rend.material.color = Color.red;
            yield return new WaitForSeconds(0.1f);
            rend.material.color = original;
        }
    }

    void Die()
    {
        Debug.Log("敌人死亡！");

        if (deathEffect != null)
        {
            Instantiate(deathEffect, transform.position, Quaternion.identity);
        }

        GetComponent<Collider>().enabled = false;
        this.enabled = false; 

        Destroy(gameObject, destroyDelay);
    }
}
