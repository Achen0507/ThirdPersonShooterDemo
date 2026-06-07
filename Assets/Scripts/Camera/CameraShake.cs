using System.Collections;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    [Header("≤‚ ‘")]
    public float jumpDistance = 1f;   
    public float jumpDuration = 0.1f;

    public void Shake()
    {
        StopAllCoroutines();
        StartCoroutine(DoJump());
    }

    private IEnumerator DoJump()
    {
        Vector3 startPos = transform.position;
        Vector3 endPos = startPos + transform.forward * (-jumpDistance);

        float elapsed = 0f;
        while (elapsed < jumpDuration)
        {
            float t = elapsed / jumpDuration;
            float randomX = Random.Range(-0.1f, 0.1f);
            float randomY = Random.Range(-0.1f, 0.1f);
            transform.position = Vector3.Lerp(startPos, endPos, t) 
                               + new Vector3(randomX, randomY, 0);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = startPos;
    }
}