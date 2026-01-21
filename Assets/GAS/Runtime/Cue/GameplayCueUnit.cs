using System;
using Unity.Entities;
using UnityEngine;

namespace GAS.Runtime
{
    /// <summary>
    /// 面向使用EX-GAS开发者的GameplayCue控制单位，可以理解为Cue面向对象开发的伪装类
    /// GameplayCue设计上允许作为一个独立系统被使用。因此给出了这个类，用于GAS外部使用。
    /// </summary>
    public class GameplayCueUnit
    {
        private Entity _cueEntity;
        private Type _cueType;
        private IExParameterBase _exParameterBase;
        private int[] _requiredTags;
        private int[] _immunityTags;
        
        private static EntityManager EntityManager=>GASManager.EntityManager;
        
        /// <summary>
        /// GameplayCue独立控制单位
        /// </summary>
        /// <param name="cueType">Cue 类型</param>
        /// <param name="exParameterBase">Cue 对应的自定义参数</param>
        /// <param name="requiredTags">可选：添加到ASC时，ASC播放需求的tag</param>
        /// <param name="immunityTags">>可选：添加到ASC时，ASC播放免疫的tag</param>
        public GameplayCueUnit(Type cueType,IExParameterBase exParameterBase,int[] requiredTags = null, int[] immunityTags = null)
        {
            _cueType = cueType;
            _exParameterBase = exParameterBase;
            _requiredTags = requiredTags;
            _immunityTags = immunityTags;
        }
        
        public GameplayCueUnit(GameplayCueConfig config)
        {
            _cueType = config.CueType;
            _exParameterBase = config.Param;
            _requiredTags = config.RequiredTags;
            _immunityTags = config.ImmunityTags;
        }

        private bool CheckCueEntity()
        {
            if (_cueEntity == Entity.Null || !EntityManager.Exists(_cueEntity))
            {
#if UNITY_EDITOR
                Debug.LogError($"[EX] cue运行用实例不存在，请先创建cue. 【 调用Create() 】");
#endif
                return false;
            }

            return true;
        }

        /// <summary>
        /// 创建GameplayCue运行用的实例
        /// </summary>
        public void Create()
        {
            if (_cueEntity != Entity.Null && EntityManager.Exists(_cueEntity))
            {
#if UNITY_EDITOR
                Debug.LogError($"[EX] Cue已经创建过了，不能重复创建。");
#endif
                return;
            }
            _cueEntity = EntityManager.CreateEntity();
            EntityManager.SetName(_cueEntity,$"Cue_{_cueType.Name}_{_cueEntity.Version}_{_cueEntity.Index}");
            
            var mcCue = new MCCue(CueHelper.TryCreateCue(_cueType, _exParameterBase));
            mcCue.cue.SetCueEntity(_cueEntity);
            mcCue.cue.SetSourceEntity(Entity.Null, CueSourceType.None);
            EntityHelper.AddManagedComponent<MCCue>(_cueEntity);
            EntityHelper.SetManagedComponent(_cueEntity,mcCue);
            
            EntityHelper.AddComponent<ECCuePlayable>(_cueEntity);
            EntityHelper.SetComponentEnabled<ECCuePlayable>(_cueEntity,false);
            
            EntityHelper.AddComponent<ECCuePlaying>(_cueEntity);
            EntityHelper.SetComponentEnabled<ECCuePlaying>(_cueEntity,false);
            
            EntityHelper.AddComponent<ECKillCue>(_cueEntity);
            EntityHelper.SetComponentEnabled<ECKillCue>(_cueEntity,false);
        }
        
        /// <summary>
        ///  销毁GameplayCue运行用的实例
        /// </summary>
        public void Destroy()
        {
            
            if (!CheckCueEntity())
            {
#if UNITY_EDITOR
                Debug.LogError($"[EX] Cue没有创建过或已被销毁，不能重复销毁。");
#endif
                return;
            }

            EntityManager.SetComponentEnabled<ECKillCue>(_cueEntity,true);
            var mcCue = EntityManager.GetComponentData<MCCue>(_cueEntity);
            mcCue.cue.OnRemove(Time.time);
            _cueEntity = Entity.Null;
        }
        
        /// <summary>
        ///    添加GameplayCue到ASC
        /// </summary>
        /// <param name="asc"></param>
        /// <returns></returns>
        public bool AddToAsc(AbilitySystemCell asc)
        {
            return CheckCueEntity() && AddToAsc(asc.Entity);
        }
        
        /// <summary>
        ///   添加GameplayCue到ASC
        /// </summary>
        /// <param name="asc"></param>
        /// <returns></returns>
        public bool AddToAsc(Entity asc)
        {
            if (!CheckCueEntity()) return false;
            
            if (_requiredTags != null)
            {
                if(!ASCHelper.HasAllTags(asc,_requiredTags)) return false;
            }
            
            if (_immunityTags != null)
            {
                if(ASCHelper.HasAnyTags(asc,_immunityTags)) return false;
            }

            var mcCue = EntityManager.GetComponentData<MCCue>(_cueEntity);
            mcCue.cue.AddToTargetAsc(asc);
            return true;
        }
        
        
        /// <summary>
        ///     从ASC移除GameplayCue
        /// </summary>
        public void RemoveFromAsc()
        {
            if (!CheckCueEntity()) return;
            
            var mcCue = EntityManager.GetComponentData<MCCue>(_cueEntity);
            mcCue.cue.RemoveFromTargetAsc();
        }
        
        /// <summary>
        ///   播放GameplayCue
        /// </summary>
        public void Play()
        {
            if (!CheckCueEntity()) return;
            EntityManager.SetComponentEnabled<ECCuePlayable>(_cueEntity,true);
        }
        
        /// <summary>
        ///     停止GameplayCue
        /// </summary>
        public void Stop()
        {
            if (!CheckCueEntity()) return;
            EntityManager.SetComponentEnabled<ECCuePlayable>(_cueEntity,false);
        }
        
        /// <summary>
        ///     手动Tick
        /// </summary>
        public void Tick()
        {
            if (!CheckCueEntity()) return;
            // TODO: 直接调用Cue的Tick方法
            //EntityManager.SetComponentEnabled<ECCuePlayable>(_cueEntity,false);
        }
        
        /// <summary>
        ///   设置GameplayCue来源
        /// </summary>
        /// <param name="source"></param>
        /// <param name="sourceType"></param>
        public void SetSource(Entity source, CueSourceType sourceType)
        {
            if (!CheckCueEntity()) return;
            var mcCue = EntityManager.GetComponentData<MCCue>(_cueEntity);
            mcCue.cue.SetSourceEntity(source, sourceType);
            EntityHelper.SetManagedComponent(_cueEntity,mcCue);
        }
    }
}