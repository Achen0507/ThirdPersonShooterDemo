using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 数据容器，存储行为树节点需要的数据
/// </summary>
public class Blackboard : MonoBehaviour
{
    private Dictionary<string,object> data = new Dictionary<string, object>();
    public void Set(string key,object value) {
        data[key] = value;
    }

    public T Get<T>(string key) {
        if (data.TryGetValue(key, out object value)) {
            return (T)value;
        }
        return default;
    }

    public bool Has(string key) {
        return data.ContainsKey(key);
    }

    public void Clear() {
        data.Clear();
    }

    // 常用快捷方法
    public Transform GetPlayer() => Get<Transform>("Player");
    public void SetPlayer(Transform player) => Set("Player", player);

    public float GetHealth() => Get<float>("Health");
    public void SetHealth(float health) => Set("Health", health);

    public Transform GetTarget() => Get<Transform>("Target");
    public void SetTarget(Transform target) => Set("Target", target);

}
