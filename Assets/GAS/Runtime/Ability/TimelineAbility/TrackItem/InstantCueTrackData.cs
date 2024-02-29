using System;
using System.Collections.Generic;
using GAS.Runtime.Cue;

namespace GAS.Runtime.Ability.TimelineAbility
{
    [Serializable]
    public class InstantCueTrackData:TrackDataBase
    {
        public List<InstantCueMarkEvent> markEvents;
        
        public override void DefaultInit(int index)
        {
            base.DefaultInit(index);
            markEvents = new List<InstantCueMarkEvent>();
        }
    }
    
    [Serializable]
    public class InstantCueMarkEvent:MarkEventBase
    {
        public List<GameplayCueInstant> cues = new List<GameplayCueInstant>();
    }
}