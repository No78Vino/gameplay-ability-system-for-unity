using System;
using System.Collections.Generic;
using GAS.Runtime.Cue;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using UnityEngine.Serialization;

namespace GAS.Runtime.Ability.TimelineAbility
{
    public class TimelineAbilityAsset : AbilityAsset
    {
        public override Type AbilityType()
        {
            return typeof(TimelineAbility);
        }

        [BoxGroup] 
        [LabelText("手动结束能力")]
        [LabelWidth(100)]
        public bool manualEndAbility;
        
        [HideInInspector]
        public int FrameCount;// 能力结束时间
        
        [HideInInspector]
        public AbilityAnimationData AnimationData = new AbilityAnimationData();
        
        [HideInInspector]
        public List<DurationalCueTrackData> DurationalCues = new List<DurationalCueTrackData>();
        
        // 即时Cue有且只有一个轨道
        [HideInInspector]
        public InstantCueTrackData InstantCues = new InstantCueTrackData();
        
        // 施放型GameplayEffect轨道（唯一）
        [HideInInspector]
        public ReleaseGameplayEffectTrackData ReleaseGameplayEffect = new ReleaseGameplayEffectTrackData();
        
        // Buff型GameplayEffect
        [HideInInspector]
        public List<BuffGameplayEffectTrackData> BuffGameplayEffects = new List<BuffGameplayEffectTrackData>();
        
        // 任务标记事件（唯一）
        [HideInInspector]
        public TaskMarkEventTrackData InstantTasks = new TaskMarkEventTrackData();
        
        // 任务片段事件
        [HideInInspector]
        public List<TaskClipEventTrackData> OngoingTasks = new List<TaskClipEventTrackData>();
        
#if UNITY_EDITOR
        public void Save()
        {
            UnityEditor.EditorUtility.SetDirty(this);
            UnityEditor.AssetDatabase.SaveAssets();
        }
#endif
    }
}