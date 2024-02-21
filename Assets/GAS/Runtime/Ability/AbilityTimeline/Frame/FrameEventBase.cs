using System;

namespace GAS.Runtime.Ability.AbilityTimeline
{
    [Serializable]
    public struct FrameEvent
    {
        public int Frame;
        public FrameEventBase Event;
    }
    
    [Serializable]
    public abstract class FrameEventBase
    {
    }
}