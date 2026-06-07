using System.Collections;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    [Header("暴力测试")]
    public float jumpDistance = 1f;      // 先设大一点，比如 1 米
    public float jumpDuration = 0.1f;

    public void Shake()
    {
        StopAllCoroutines();
        StartCoroutine(DoJump());
    }

    // 可选：添加更多维度的抖动（左右+上下）
    private IEnumerator DoJump()
    {
        Vector3 startPos = transform.position;
        // 沿相机自己的 forward 方向后退（相对于相机朝向）
        Vector3 endPos = startPos + transform.forward * (-jumpDistance);

        float elapsed = 0f;
        while (elapsed < jumpDuration)
        {
            float t = elapsed / jumpDuration;
            // 添加一些随机抖动
            float randomX = Random.Range(-0.1f, 0.1f);
            float randomY = Random.Range(-0.1f, 0.1f);
            transform.position = Vector3.Lerp(startPos, endPos, t) 
                               + new Vector3(randomX, randomY, 0);
            elapsed += Time.deltaTime;
            yield return null;
        }
         
        // 瞬间归位（不用平滑，看震动是否执行）
        transform.position = startPos;
    }
}