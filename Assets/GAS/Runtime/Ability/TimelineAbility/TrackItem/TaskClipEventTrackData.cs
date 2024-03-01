using System;
using System.Collections.Generic;
using GAS.Runtime.Ability.TimelineAbility.AbilityTask;

namespace GAS.Runtime.Ability.TimelineAbility
{
    [Serializable]
    public class TaskClipEventTrackData:TrackDataBase
    {
        public string trackName;
        public List<TaskClipEvent> clipEvents;

        public override void AddToAbilityAsset(TimelineAbilityAsset abilityAsset)
        {
            base.AddToAbilityAsset(abilityAsset);
            abilityAsset.taskClips.Add(this);
        }

        public override void DefaultInit(int index)
        {
            base.DefaultInit(index);
            trackName = "Task Clips";
            clipEvents = new List<TaskClipEvent>();
        }
    }
    
    [Serializable]
    public class TaskClipEvent : ClipEventBase
    {
        public OngoingAbilityTask task;
    }
}