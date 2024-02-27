using System;
using System.Collections.Generic;
using GAS.Runtime.Cue;

namespace GAS.Runtime.Ability.AbilityTimeline
{
    [Serializable]
    public class CueTrackData:TrackDataBase
    {
        public string trackName;
        public List<DurationalCueClipEvent> clipEvents = new List<DurationalCueClipEvent>();
    }
    
    [Serializable]
    public class DurationalCueClipEvent : ClipEventBase
    {
        public GameplayCueDurational cue;
    }
}