using System;
using System.Collections.Generic;
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
        [ShowInInspector]
        [NonSerialized,OdinSerialize]
        public AbilityAnimationData AnimationData = new AbilityAnimationData();
        
        
#if UNITY_EDITOR
        public void Save()
        {
            UnityEditor.EditorUtility.SetDirty(this);
            UnityEditor.AssetDatabase.SaveAssets();
        }
#endif
    }

    [Serializable]
    public class AbilityAnimationData
    {
        [NonSerialized,OdinSerialize]
        [ShowInInspector]
        [DictionaryDrawerSettings( KeyLabel = "Frame", ValueLabel = "Event")]
        public Dictionary<int, AnimationFrameEvent> FrameData = new Dictionary<int, AnimationFrameEvent>();
    }

    [Serializable]
    public abstract class FrameEventBase
    {
        
    }

    [Serializable]
    public class AnimationFrameEvent : FrameEventBase
    {
        public AnimationClip Clip;
        public float TransitionTime;

#if UNITY_EDITOR
        public int DurationFrame;
#endif
    }
}