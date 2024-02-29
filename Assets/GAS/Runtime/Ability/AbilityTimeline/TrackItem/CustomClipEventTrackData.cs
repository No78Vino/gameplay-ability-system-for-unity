using System;
using System.Collections.Generic;

namespace GAS.Runtime.Ability.AbilityTimeline
{
    [Serializable]
    public class CustomClipEventTrackData:TrackDataBase
    {
        public string trackName;
        public List<CustomClipEvent> clipEvents;

        public override void AddToAbilityAsset(TimelineAbilityAsset abilityAsset)
        {
            base.AddToAbilityAsset(abilityAsset);
            abilityAsset.CustomClips.Add(this);
        }

        public override void DefaultInit(int index)
        {
            base.DefaultInit(index);
            trackName = "Custom Clips";
            clipEvents = new List<CustomClipEvent>();
        }
    }
    
    [Serializable]
    public class CustomClipEvent : ClipEventBase
    {
        public string customEventKey;
    }
}