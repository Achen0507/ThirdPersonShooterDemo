using UnityEngine;

public class AnimationEventReceiver : MonoBehaviour
{
    public MeleeWeapon meleeWeapon;

    public void OnHitFrame()
    {
        if (meleeWeapon != null)
            meleeWeapon.OnHitFrame();
    }

    public void FinishReload()
    {
        Rifle rifle = GetComponent<Rifle>();
        if (rifle != null) rifle.FinishReload();
    }
}