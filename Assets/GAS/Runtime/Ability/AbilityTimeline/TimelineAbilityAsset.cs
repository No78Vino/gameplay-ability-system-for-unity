using System;
using System.Collections.Generic;
using GAS.Runtime.Ability.AbilityTimeline;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace GAS.Runtime.Ability
{
    public class TimelineAbilityAsset : AbilityAsset
    {
        public override Type AbilityType()
        {
            return typeof(GeneralSequentialAbility);
        }
        
        [HideInInspector]
        public int MaxFrameCount;// 能力结束时间
        
        [BoxGroup]
        public AbilityAnimationData AnimationData = new AbilityAnimationData();
        
        // TODO : 能力不可打断（取消）时间段
        
        // TODO : 持续性Cue轨道集合(特效，音效包含其中)
        
        // TODO : 瞬时性Cue轨道集合(特效，音效包含其中)
        
        // TODO : 持续性GameplayEffect发动轨道集合
        
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