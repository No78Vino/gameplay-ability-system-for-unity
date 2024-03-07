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
        public List<DurationalCueTrackData> DurationalCues = new List<DurationalCueTrackData>();

        [HideInInspector] public List<InstantCueTrackData> InstantCues = new List<InstantCueTrackData>();

        [HideInInspector]
        public List<ReleaseGameplayEffectTrackData> ReleaseGameplayEffect = new List<ReleaseGameplayEffectTrackData>();
        
        [HideInInspector]
        public List<BuffGameplayEffectTrackData> BuffGameplayEffects = new List<BuffGameplayEffectTrackData>();

        [HideInInspector] public List<TaskMarkEventTrackData> InstantTasks = new List<TaskMarkEventTrackData>();
        
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