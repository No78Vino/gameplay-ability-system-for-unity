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
        
        [BoxGroup]
        public AbilityAnimationData AnimationData = new AbilityAnimationData();
        
        [BoxGroup]
        // TODO : 持续性Cue轨道集合(特效，音效包含其中)
        public List<CueTrackData> DurationalCues = new List<CueTrackData>();
        
        //[BoxGroup]
        // TODO : 瞬时性Cue轨道集合(特效，音效包含其中)
        //public List<GameplayCueInstant> InstantCues = new List<GameplayCueInstant>();
        
        // TODO : GameplayEffect发动轨道集合
        public List<GameplayEffectTrackData> GameplayEffects = new List<GameplayEffectTrackData>();
        
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