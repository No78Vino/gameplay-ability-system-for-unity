using System;
using System.Collections.Generic;
using UnityEngine.Serialization;

namespace GAS.Runtime.Ability.AbilityTimeline
{
    [Serializable]
    public class AbilityAnimationData
    {
        public List<AnimationFrameEventInfo> frameData = new List<AnimationFrameEventInfo>();
    }
    
    [Serializable]
    public class AnimationFrameEventInfo
    {
        public int Frame;
        public AnimationFrameEvent Event;
    }
}