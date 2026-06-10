using System.Collections;
using UnityEngine;

/// <summary>
/// 近战武器类，负责处理近战攻击逻辑和相关属性。
/// </summary>
public class MeleeWeapon : WeaponBase
{
    private float nextAttackTime;
    public float attackRadius = 2.5f;
    public Transform attackPoint;

    public override void OnShoot()
    {
        if (Time.time < nextAttackTime) return;

        animator?.SetTrigger("MeleeAttack");

        if (attackSound != null && audioSource != null)
            audioSource.PlayOneShot(attackSound);

        nextAttackTime = Time.time + config.attackCooldown;
    }

    public void OnHitFrame()
    {
        Vector3 center = attackPoint != null ? attackPoint.position : Camera.main.transform.position;
        Collider[] hitColliders = Physics.OverlapSphere(center, attackRadius);

        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.transform == transform) continue;

            var damageable = hitCollider.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(config.damage);
                Debug.Log($"近战击中: {hitCollider.name}");

                if (hitEffect != null)
                {
                    Instantiate(hitEffect, hitCollider.transform.position + Vector3.up, Quaternion.identity);
                }
                break;
            }
        }
    }

    public override void OnReload() { }
}
