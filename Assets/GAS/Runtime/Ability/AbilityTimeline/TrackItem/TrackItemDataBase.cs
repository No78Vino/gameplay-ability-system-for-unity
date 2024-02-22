using System;
using UnityEngine.Serialization;

namespace GAS.Runtime.Ability.AbilityTimeline
{
    [Serializable]
    public abstract class FrameEventBase
    {
    }
    
    [Serializable]
    public abstract class ClipEventBase
    {
        public int startFrame;
        public int durationFrame;
        public int EndFrame => startFrame + durationFrame;
    }
}