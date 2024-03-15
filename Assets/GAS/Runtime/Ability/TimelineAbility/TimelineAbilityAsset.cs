using System;
using System.Collections.Generic;
using GAS.General;
using GAS.Runtime;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace GAS.Runtime
{
    public class TimelineAbilityAsset : AbilityAsset
    {
        [BoxGroup] [LabelText(GASTextDefine.ABILITY_MANUAL_ENDABILITY)] [LabelWidth(100)]
        public bool manualEndAbility;

        [HideInInspector] public int FrameCount; // 能力结束时间

        [HideInInspector] public List<DurationalCueTrackData> DurationalCues = new List<DurationalCueTrackData>();

        [HideInInspector] public List<InstantCueTrackData> InstantCues = new List<InstantCueTrackData>();

        [HideInInspector] public List<ReleaseGameplayEffectTrackData> ReleaseGameplayEffect = new List<ReleaseGameplayEffectTrackData>();

        [HideInInspector] public List<BuffGameplayEffectTrackData> BuffGameplayEffects = new List<BuffGameplayEffectTrackData>();

        [HideInInspector] public List<TaskMarkEventTrackData> InstantTasks = new List<TaskMarkEventTrackData>();

        [HideInInspector] public List<TaskClipEventTrackData> OngoingTasks = new List<TaskClipEventTrackData>();

        public override Type AbilityType()
        {
            return typeof(TimelineAbility);
        }

#if UNITY_EDITOR
        public void Save()
        {
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
        }
#endif
    }
}