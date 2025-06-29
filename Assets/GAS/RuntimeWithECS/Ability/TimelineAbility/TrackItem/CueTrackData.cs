using System;
using System.Collections.Generic;

namespace GAS.Runtime
{
    [Serializable]
    public class CueTrackData:TrackDataBase
    {
        public List<CueClipEvent> clipEvents = new List<CueClipEvent>();

        public override void AddToAbilityAsset(TimelineAbilityAssetBase abilityAsset)
        {
            base.AddToAbilityAsset(abilityAsset);
            abilityAsset.DurationalCues.Add(this);
        }
    }
    
    [Serializable]
    public class CueClipEvent : TrackEventBase
    {
        public GameplayCueUnit cue;
    }
}