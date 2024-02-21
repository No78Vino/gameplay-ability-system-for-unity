using System;
using UnityEngine;

namespace GAS.Runtime.Ability.AbilityTimeline
{
    [Serializable]
    public class AnimationFrameEvent : FrameEventBase
    {
        public AnimationClip Clip;
        public float TransitionTime;

#if UNITY_EDITOR
        public int DurationFrame;
#endif
    }
}