using System.Collections.Generic;
using GAS.RuntimeWithECS;

namespace GAS.Runtime
{
    public class AbilityParamTimeline : IAbilityParam
    {
        public bool ManualEndAbility;
        public int FrameCount; // 技能时长
        public List<CueTrackData> Cues = new List<CueTrackData>();
        public List<TaskClipEventTrackData> Tasks = new List<TaskClipEventTrackData>();
        
#if UNITY_EDITOR
        public void DecodeExcelData(List<object> paramData)
        {
        }

        public List<object> EncodeExcelData()
        {
            return new List<object>();
        }
#endif
    }
}