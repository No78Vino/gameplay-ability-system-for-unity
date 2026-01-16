using System;
using System.Collections.Generic;

namespace GAS.Runtime
{
    [Serializable]
    public class TaskClipEventTrackData:TrackDataBase
    {
        public List<TaskClipData> clipDatas = new List<TaskClipData>();

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
}