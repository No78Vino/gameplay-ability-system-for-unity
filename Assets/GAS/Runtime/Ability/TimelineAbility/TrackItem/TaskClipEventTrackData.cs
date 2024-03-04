using System;
using System.Collections.Generic;
using UnityEngine.Serialization;

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
            abilityAsset.OngoingTasks.Add(this);
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
        public OngoingTaskData ongoingTask;

        public OngoingAbilityTask Load()
        {
            return ongoingTask.Load() as OngoingAbilityTask;
        }
    }
}