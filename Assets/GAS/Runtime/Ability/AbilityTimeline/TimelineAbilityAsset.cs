using System;
using System.Collections.Generic;
using GAS.Runtime.Ability.AbilityTimeline;
using GAS.Runtime.Cue;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace GAS.Runtime.Ability
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
        public List<BuffGameplayEffectTrackData> BuffGameplayEffects = new List<BuffGameplayEffectTrackData>();
        
        // TODO : 单位施加GameplayEffect发动轨道集合（Target生效函数覆写）
        
        // TODO : 自定义事件轨道集合
        
#if UNITY_EDITOR
        public void Save()
        {
            UnityEditor.EditorUtility.SetDirty(this);
            UnityEditor.AssetDatabase.SaveAssets();
        }
#endif
    }
}