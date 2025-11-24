using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RootMotion
{

    public abstract class AnimationModifier : MonoBehaviour
    {
        protected Animator animator;
        protected Baker baker;

        public virtual void OnInitiate(Baker baker, Animator animator)
        {
            this.baker = baker;
            this.animator = animator;
        }

        public virtual void OnStartClip(AnimationClip clip) { }

        public virtual void OnBakerUpdate(float normalizedTime) { }
    }
}
