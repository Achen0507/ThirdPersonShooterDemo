using UnityEngine;

[CreateAssetMenu(fileName = "NewWeaponConfig", menuName = "Game/Weapon/WeaponConfig")]
public class WeaponConfig : ScriptableObject
{
    [Header("基础信息")]
    public string weaponName;
    public WeaponType type;

    [Header("战斗属性")]
    public float damage = 10f;
    public float range = 20f;
    public float fireRate = 0.2f;
    public int maxAmmo = 30;

    [Header("近战专用")]
    public float attackCooldown = 0.5f;

    [Header("特效")]
    public GameObject muzzleFlashPrefab;
}
