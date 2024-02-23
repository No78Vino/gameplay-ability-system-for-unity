using System;
using UnityEngine.Serialization;

namespace GAS.Runtime.Ability.AbilityTimeline
{
    [Serializable]
    public abstract class TrackEventBase
    {        
        public int startFrame;
    }
    
    [Serializable]
    public abstract class FrameEventBase:TrackEventBase
    {
    }
    
    [Serializable]
    public abstract class ClipEventBase:TrackEventBase
    {
        public int durationFrame;
        public int EndFrame => startFrame + durationFrame;
    }
}