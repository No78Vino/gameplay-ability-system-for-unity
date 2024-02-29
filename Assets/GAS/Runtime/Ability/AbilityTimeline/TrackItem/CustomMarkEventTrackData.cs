using System;
using System.Collections.Generic;

namespace GAS.Runtime.Ability.AbilityTimeline
{
    [Serializable]
    public class CustomMarkEventTrackData:TrackDataBase
    {
        public List<CustomMarkEvent> markEvents;
        
        public override void DefaultInit(int index)
        {
            base.DefaultInit(index);
            markEvents = new List<CustomMarkEvent>();
        }
    }
    
    [Serializable]
    public class CustomMarkEvent:MarkEventBase
    {
    }
}