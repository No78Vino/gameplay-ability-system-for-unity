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
        
        [HideInInspector]
        public int MaxFrameCount;// 能力结束时间
        
        [HideInInspector]
        public AbilityAnimationData AnimationData = new AbilityAnimationData();
        
        [BoxGroup]
        public List<DurationalCueTrackData> DurationalCues = new List<DurationalCueTrackData>();
        
        // 即时Cue有且只有一个轨道
        [BoxGroup]
        public InstantCueTrackData InstantCues = new InstantCueTrackData();
        
        // 施放型GameplayEffect轨道（唯一）
        [BoxGroup]
        public ReleaseGameplayEffectTrackData ReleaseGameplayEffect = new ReleaseGameplayEffectTrackData();
        
        // Buff型GameplayEffect
        [BoxGroup]
        public List<BuffGameplayEffectTrackData> BuffGameplayEffects = new List<BuffGameplayEffectTrackData>();
        
        // 任务标记事件（唯一）
        [BoxGroup]
        public TaskMarkEventTrackData taskMarks = new TaskMarkEventTrackData();
        
        // 任务片段事件
        [BoxGroup]
        public List<TaskClipEventTrackData> taskClips = new List<TaskClipEventTrackData>();
        
#if UNITY_EDITOR
        public void Save()
        {
            UnityEditor.EditorUtility.SetDirty(this);
            UnityEditor.AssetDatabase.SaveAssets();
        }
#endif
    }
}