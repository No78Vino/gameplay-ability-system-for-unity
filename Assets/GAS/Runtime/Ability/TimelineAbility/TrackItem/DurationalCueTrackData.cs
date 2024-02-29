using System;
using System.Collections.Generic;
using GAS.Runtime.Cue;

namespace GAS.Runtime.Ability.TimelineAbility
{
    [Serializable]
    public class DurationalCueTrackData:TrackDataBase
    {
        public string trackName;
        public List<DurationalCueClipEvent> clipEvents;

        public override void AddToAbilityAsset(TimelineAbilityAsset abilityAsset)
        {
            base.AddToAbilityAsset(abilityAsset);
            abilityAsset.DurationalCues.Add(this);
        }

        public override void DefaultInit(int index)
        {
            base.DefaultInit(index);
            trackName = "DurationalCueTrack";
            clipEvents = new List<DurationalCueClipEvent>();
        }
    }
    
    [Serializable]
    public class DurationalCueClipEvent : ClipEventBase
    {
        public GameplayCueDurational cue;
    }
}