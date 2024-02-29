using System;
using System.Collections.Generic;
using GAS.Runtime.Cue;
using GAS.Runtime.Effects;
using UnityEngine;
using UnityEngine.Serialization;

namespace GAS.Runtime.Ability.TimelineAbility
{
    [Serializable]
    public class ReleaseGameplayEffectTrackData:TrackDataBase
    {
        public List<ReleaseGameplayEffectMarkEvent> markEvents = new List<ReleaseGameplayEffectMarkEvent>();
    }
    
    [Serializable]
    public class ReleaseGameplayEffectMarkEvent:MarkEventBase
    {
        public LockOnTargetMethod method = new LockOnTargetMethod();
        public List<GameplayEffectAsset> gameplayEffectAssets = new List<GameplayEffectAsset>();
    }

    public enum LockMethod
    {
        Self,
        Circle2D,
        Box2D,
        Sphere3D,
        Box3D,
        Custom
    }
    
    public enum CenterType
    {
        Relative,
        WorldSpace,
    }
    
    [Serializable]
    public class LockOnTargetMethod
    {
        public LockMethod method;
        
        // 检测碰撞
        public LayerMask checkLayer;
        public CenterType centerType;
        public Vector3 center;
        // Circle2D,Sphere3D
        public float radius;
        // Box2D,Box3D
        public Vector3 size;
        
        // Custom
        public string customMethodRegisterKey;
    }
}