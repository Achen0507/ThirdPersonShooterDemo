using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    [Header("生命值")]
    public float maxHealth = 100f;
    private float currentHealth;

    [Header("UI")]
    public Slider healthBar;
    public GameObject gameOverPanel;

    [Header("受伤反馈")]
    public GameObject hitEffect;
    public AudioClip hitSound;
    public float hitFlashDuration = 0.1f;

    [Header("死亡")]
    public GameObject deathEffect;
    public float destroyDelay = 1.5f;

    private AudioSource audioSource;
    private bool isDead = false;

    private void Start()
    {
        currentHealth = maxHealth;
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null && hitSound != null)
            audioSource = gameObject.AddComponent<AudioSource>();

        UpdateHealthBar();
    }
    public void TakeDamage(float damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        UpdateHealthBar();

        // 受伤反馈
        if (hitEffect != null)
        {
            Instantiate(hitEffect, transform.position + Vector3.up * 1f, Quaternion.identity);
        }

        if (hitSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(hitSound);
        }

        Debug.Log($"玩家受伤！剩余生命: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(float amount)
    {
        if (isDead) return;
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        UpdateHealthBar();
        Debug.Log($"玩家治疗！当前生命: {currentHealth}");
    }

    void UpdateHealthBar()
    {
        if (healthBar != null)
        {
            healthBar.value = currentHealth / maxHealth;
        }
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;

        Debug.Log("玩家死亡！");

        if (deathEffect != null)
        {
            Instantiate(deathEffect, transform.position, Quaternion.identity);
        }

        // 显示 Game Over 面板
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }

        // 禁用玩家控制
        DisablePlayer();

        Destroy(gameObject, destroyDelay);
    }

    void DisablePlayer()
    {
        // 禁用移动、射击等组件
        var controllers = GetComponents<MonoBehaviour>();
        foreach (var c in controllers)
        {
            if (c != this)
                c.enabled = false;
        }

        // 禁用碰撞体
        var collider = GetComponent<Collider>();
        if (collider != null)
            collider.enabled = false;
    }

    public float GetCurrentHealth() => currentHealth;
    public float GetMaxHealth() => maxHealth;
    public bool IsDead() => isDead;
}
