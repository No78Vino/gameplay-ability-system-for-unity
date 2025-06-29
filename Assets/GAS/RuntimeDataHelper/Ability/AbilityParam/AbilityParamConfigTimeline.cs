using System;
using System.Collections.Generic;
using System.Reflection;
using GAS.Runtime;
using GAS.RuntimeDataHelper.Ability.AbilityParam;
using GAS.RuntimeWithECS.Ability.Component;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GAS.Editor
{
    [Serializable]
    public class AbilityParamConfigTimeline: AbilityParamConfigBase<AbilityParamTimeline>
    {
        public bool manualEndAbility;
        
        //[HideInInspector]
        public int frameCount; // 能力结束时间
        
        //[HideInInspector]
        public List<CueTrackData> cues = new List<CueTrackData>();
        
        //[HideInInspector]
        public List<TaskClipEventTrackData> tasks = new List<TaskClipEventTrackData>();
        
        [Button("查看/编辑能力时间轴", ButtonSizes.Large, Icon = SdfIconType.Hammer)]
        private void EditAbility()
        {
            try
            {
                var assembly = Assembly.Load("com.exhard.exgas.editor");
                var type = assembly.GetType("GAS.Editor.AbilityTimelineEditorWindow");
                var methodInfo = type.GetMethod("ShowWindow", BindingFlags.Public | BindingFlags.Static);
                methodInfo!.Invoke(null, new object[] { this });
            }
            catch (Exception e)
            {
                Debug.LogError(
                    $"调用\"GAS.Editor.AbilityTimelineEditorWindow\"类的静态方法ShowWindow(TimelineAbilityAsset asset)失败, 代码可能被重构了: {e}");
            }
        }
        
        public override AbilityParamBase GetConfig()
        {
            return new AbilityParamTimeline
            {
                ManualEndAbility = manualEndAbility,
                FrameCount = frameCount,
                Cues = cues,
                Tasks = tasks
            };
        }
    }
}