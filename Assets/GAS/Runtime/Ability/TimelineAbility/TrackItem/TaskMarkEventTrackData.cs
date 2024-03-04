using System;
using System.Collections.Generic;

namespace GAS.Runtime.Ability.TimelineAbility
{
    [Serializable]
    public class TaskMarkEventTrackData : TrackDataBase
    {
        public List<TaskMarkEvent> markEvents = new List<TaskMarkEvent>();
    }

    [Serializable]
    public class TaskMarkEvent:MarkEventBase
    {
        public List<InstantTaskData> InstantTasks = new List<InstantTaskData>();
    }
}