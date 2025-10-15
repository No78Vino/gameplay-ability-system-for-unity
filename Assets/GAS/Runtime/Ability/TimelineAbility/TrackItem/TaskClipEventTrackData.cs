using System;
using System.Collections.Generic;

namespace GAS.Runtime
{
    [Serializable]
    public class TaskClipEventTrackData:TrackDataBase
    {
        public List<TaskClipEvent> clipEvents = new List<TaskClipEvent>();

        // public override void AddToAbilityAsset(TimelineAbilityAssetBase abilityAsset)
        // {
        //     base.AddToAbilityAsset(abilityAsset);
        //     abilityAsset.OngoingTasks.Add(this);
        // }

        public override void DefaultInit()
        {
            base.DefaultInit();
            trackName = "Task Clips";
        }
    }
    
    [Serializable]
    public class TaskClipEvent : TrackEventBase
    {
        public TaskData task;

        public AbilityTaskBase Load()
        {
            return task.Load();
        }
    }
}