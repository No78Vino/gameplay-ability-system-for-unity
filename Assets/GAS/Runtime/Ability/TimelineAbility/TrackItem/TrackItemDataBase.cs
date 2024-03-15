using System;
using UnityEngine.Serialization;

namespace GAS.Runtime
{
    [Serializable]
    public abstract class TrackEventBase
    {        
        public int startFrame;
    }
    
    [Serializable]
    public abstract class MarkEventBase:TrackEventBase
    {
    }
    
    [Serializable]
    public abstract class ClipEventBase:TrackEventBase
    {
        public int durationFrame;
        public int EndFrame => startFrame + durationFrame;
    }
}