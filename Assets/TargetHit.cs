using UnityEngine;

public class TargetHit : MonoBehaviour
{
    public void Hit() {
        Debug.Log("°Ð×Ó±»»÷ÖÐ£¡");
        GetComponent<Renderer>().material.color = Color.red;
    }
}
