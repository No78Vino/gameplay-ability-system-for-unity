using System;
using System.Collections.Generic;
using GAS.General;
using GAS.Runtime;
using UnityEngine;
using UnityEngine.Profiling;

namespace GAS.Runtime
{
    [Serializable]
    public class ReleaseGameplayEffectTrackData : TrackDataBase
    {
        public List<ReleaseGameplayEffectMarkEvent> markEvents = new List<ReleaseGameplayEffectMarkEvent>();

        public override void AddToAbilityAsset(TimelineAbilityAssetBase abilityAsset)
        {
            base.AddToAbilityAsset(abilityAsset);
            abilityAsset.ReleaseGameplayEffect.Add(this);
        }
    }

    [Serializable]
    public class ReleaseGameplayEffectMarkEvent : MarkEventBase
    {
        public JsonData jsonTargetCatcher = new JsonData()
        {
            Type = typeof(CatchSelf).FullName // 默认 CatchSelf
        };

        public List<GameplayEffectAsset> gameplayEffectAssets = new List<GameplayEffectAsset>();


        private TargetCatcherBase _targetCatcher;
        public TargetCatcherBase TargetCatcher
        {
            get
            {
                // 如果是反序列化的数据，没有执行构造函数, 需要加载
                _targetCatcher ??= LoadTargetCatcher();
                return _targetCatcher;
            }
        }

        public void CacheTargetCatcher()
        {
            _targetCatcher = LoadTargetCatcher();
        }

        public void SaveTargetCatcher(TargetCatcherBase targetCatcher)
        {
            var jsonData = JsonUtility.ToJson(targetCatcher);
            var dataType = targetCatcher.GetType().FullName;
            jsonTargetCatcher = new JsonData
            {
                Type = dataType,
                Data = jsonData
            };
        }

        public TargetCatcherBase LoadTargetCatcher()
        {
            TargetCatcherBase targetCatcher = null;
            var jsonData = jsonTargetCatcher.Data;
            var dataType = jsonTargetCatcher.Type;

            Type type = null;
            foreach (var t in TargetCatcherSonTypes)
            {
                if (t.FullName == dataType)
                {
                    type = t;
                    break;
                }
            }

            if (type == null)
            {
                Debug.LogError("[EX] TargetCatcherBase SonType not found: " + dataType);
            }
            else
            {
                if (string.IsNullOrEmpty(jsonData))
                {
                    targetCatcher = Activator.CreateInstance(type) as TargetCatcherBase;
                }
                else
                {
                    targetCatcher = JsonUtility.FromJson(jsonData, type) as TargetCatcherBase;
                }
            }

            return targetCatcher;
        }

        #region TargetCatcher SonTypes

        private static Type[] _targetCatcherSonTypes;

        public static Type[] TargetCatcherSonTypes =>
            _targetCatcherSonTypes ??= TypeUtil.GetAllSonTypesOf(typeof(TargetCatcherBase));

        #endregion
    }


    // public enum LockMethod
    // {
    //     Self,
    //     Circle2D,
    //     Box2D,
    //     Sphere3D,
    //     Box3D,
    //     Custom
    // }
    //
    // public enum CenterType
    // {
    //     Relative,
    //     WorldSpace
    // }
    //
    // [Serializable]
    // public class LockOnTargetMethod
    // {
    //     public LockMethod method;
    //
    //     // 检测碰撞
    //     public LayerMask checkLayer;
    //     public CenterType centerType;
    //
    //     public Vector3 center;
    //
    //     // Circle2D,Sphere3D
    //     public float radius;
    //
    //     // Box2D,Box3D
    //     public Vector3 size;
    //
    //     // Custom
    //     public string customMethodRegisterKey;
    // }
}