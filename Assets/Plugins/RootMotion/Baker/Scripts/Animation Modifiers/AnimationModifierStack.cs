using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RootMotion
{
    public class AnimationModifierStack : MonoBehaviour
    {
        public AnimationModifier[] modifiers = new AnimationModifier[0];

        private Animator animator;
        private Baker baker;

        private void Start()
        {
            animator = GetComponent<Animator>();
            baker = GetComponent<Baker>();
            baker.OnStartClip += OnBakerStartClip;
            baker.OnUpdateClip += OnBakerUpdateClip;

            foreach (AnimationModifier m in modifiers)
            {
                m.OnInitiate(baker, animator);
            }
        }

        private void OnBakerStartClip(AnimationClip clip, float normalizedTime)
        {
            foreach (AnimationModifier m in modifiers)
            {
                m.OnStartClip(clip);
            }
        }

        private void OnBakerUpdateClip(AnimationClip clip, float normalizedTime)
        {
            foreach (AnimationModifier m in modifiers)
            {
                if (!m.enabled) continue;
                m.OnBakerUpdate(normalizedTime);
            }
        }

        private void LateUpdate()
        {
            if (!animator.enabled && !baker.isBaking) return;
            if (baker.isBaking && baker.mode == Baker.Mode.AnimationClips) return;
            if (animator.runtimeAnimatorController == null) return;

            AnimatorStateInfo info = animator.GetCurrentAnimatorStateInfo(0);
            float n = info.normalizedTime;

            foreach (AnimationModifier m in modifiers)
            {
                if (!m.enabled) continue;
                m.OnBakerUpdate(n);
            }
        }
    }
}
