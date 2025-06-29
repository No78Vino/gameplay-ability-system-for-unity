using System.Collections.Generic;
using GAS.RuntimeWithECS.Ability.Component;

namespace GAS.Runtime
{
    public class AbilityParamTimeline : AbilityParamBase
    {
        public bool ManualEndAbility;
        public int FrameCount; // 技能时长
        public List<CueTrackData> Cues = new List<CueTrackData>();
        public List<TaskClipEventTrackData> Tasks = new List<TaskClipEventTrackData>();
    }
}