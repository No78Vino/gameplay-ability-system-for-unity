using System;

namespace GAS.Runtime
{
    [Serializable]
    public abstract class TrackEventBase
    {        
        public int startFrame;
        public int durationFrame;
        public int EndFrame => startFrame + durationFrame;
    }
}