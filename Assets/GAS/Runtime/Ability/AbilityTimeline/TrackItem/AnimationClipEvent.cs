using System;
using UnityEngine;

namespace GAS.Runtime.Ability.AbilityTimeline
{
    [Serializable]
    public class AnimationClipEvent : ClipEventBase
    {
        public AnimationClip Clip;
        public float TransitionTime;
        //public int DurationFrame;
    }
}