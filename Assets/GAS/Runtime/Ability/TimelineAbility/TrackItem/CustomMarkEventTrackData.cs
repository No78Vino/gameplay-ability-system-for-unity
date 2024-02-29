using System;
using System.Collections.Generic;

namespace GAS.Runtime.Ability.TimelineAbility
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
        public List<string> customEventKeys = new List<string>();
    }
}