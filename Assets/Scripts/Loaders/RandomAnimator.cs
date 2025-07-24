using UnityEngine;

public class RandomAnimator : MonoBehaviour
{
    public AnimatorOverrideController[] OverrideControllers;
    
    void Awake()
    {
        if (OverrideControllers == null || OverrideControllers.Length == 0)
        {
            Debug.LogError("Chưa thiết lập overrideControllers!");
            return;
        }
        Animator Anim = GetComponent<Animator>();
        if (Anim == null)
        {
            Debug.LogError("Prefab thiếu component Animator!");
            return;
        }
        int idx = Random.Range(0, OverrideControllers.Length);
        Anim.runtimeAnimatorController = OverrideControllers[idx];
    }
}
