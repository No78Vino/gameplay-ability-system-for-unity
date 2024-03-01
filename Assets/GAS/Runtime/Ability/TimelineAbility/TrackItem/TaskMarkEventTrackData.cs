using System;
using System.Collections.Generic;
using GAS.Runtime.Ability.TimelineAbility.AbilityTask;

namespace GAS.Runtime.Ability.TimelineAbility
{
    [Serializable]
    public class TaskMarkEventTrackData:TrackDataBase
    {
        public List<TaskMarkEvent> markEvents;
        
        public override void DefaultInit(int index)
        {
            base.DefaultInit(index);
            markEvents = new List<TaskMarkEvent>();
        }
    }
    
    [Serializable]
    public class TaskMarkEvent:MarkEventBase
    {
        public List<InstantAbilityTask> InstantTasks = new List<InstantAbilityTask>();
    }
}