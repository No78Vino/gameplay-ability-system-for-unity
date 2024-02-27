using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace GAS.Runtime.Ability.AbilityTimeline
{
    [Serializable]
    public class AbilityAnimationData:TrackDataBase
    {
        // TODO 绑定的动画机
        public Animator Animator;
        public List<AnimationClipEvent> animationClipData = new List<AnimationClipEvent>();
    }
}