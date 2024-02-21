using System;
using System.Collections.Generic;
using GAS.Runtime.Ability.AbilityTimeline;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace GAS.Runtime.Ability
{
    public class GeneralSequentialAbilityAsset : AbilityAsset
    {
        public override Type AbilityType()
        {
            return typeof(GeneralSequentialAbility);
        }
        
        [HideInInspector]
        public int MaxFrameCount;
        
        [BoxGroup]
        public AbilityAnimationData AnimationData = new AbilityAnimationData();
        
        
#if UNITY_EDITOR
        public void Save()
        {
            UnityEditor.EditorUtility.SetDirty(this);
            UnityEditor.AssetDatabase.SaveAssets();
        }
#endif
    }
}