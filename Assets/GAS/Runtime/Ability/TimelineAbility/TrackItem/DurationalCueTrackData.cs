using System;
using System.Collections.Generic;
using GAS.Runtime.Ability;
using GAS.Runtime.Cue;

namespace GAS.Runtime
{
    [Serializable]
    public class DurationalCueTrackData:TrackDataBase
    {
        public List<DurationalCueClipEvent> clipEvents = new List<DurationalCueClipEvent>();

        public override void AddToAbilityAsset(TimelineAbilityAsset abilityAsset)
        {
            base.AddToAbilityAsset(abilityAsset);
            abilityAsset.DurationalCues.Add(this);
        }
    }
    
    [Serializable]
    public class DurationalCueClipEvent : ClipEventBase
    {
        public GameplayCueDurational cue;
    }
}