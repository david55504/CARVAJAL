using UnityEngine;

public class NPCDecorativo : MonoBehaviour
{
    [Header("Animación")]
    [Tooltip("Clip de idle que se asignará a este NPC.")]
    public AnimationClip clipIdle;

    void Awake()
    {
        Animator anim = GetComponent<Animator>();
        if (anim == null || anim.runtimeAnimatorController == null || clipIdle == null) return;

        var overrideController = new AnimatorOverrideController(anim.runtimeAnimatorController);
        anim.runtimeAnimatorController = overrideController;

        var overrides = new System.Collections.Generic.List<System.Collections.Generic.KeyValuePair<AnimationClip, AnimationClip>>();
        overrideController.GetOverrides(overrides);

        if (overrides.Count > 0 && overrides[0].Key != null)
            overrides[0] = new System.Collections.Generic.KeyValuePair<AnimationClip, AnimationClip>(overrides[0].Key, clipIdle);

        overrideController.ApplyOverrides(overrides);
    }
}