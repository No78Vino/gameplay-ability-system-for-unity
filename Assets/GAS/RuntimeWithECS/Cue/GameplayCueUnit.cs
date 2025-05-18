using System;
using GAS.RuntimeWithECS.AbilitySystemCell;
using GAS.RuntimeWithECS.Cue;
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
        private ICueParameter _cueParameter;
        private int[] _requiredTags;
        private int[] _immunityTags;
        
        private static EntityManager EntityManager=>GASManager.EntityManager;
        
        public GameplayCueUnit(Type cueType,ICueParameter parameter,int[] requiredTags = null, int[] immunityTags = null)
        {
            _cueType = cueType;
            _cueParameter = parameter;
            _requiredTags = requiredTags;
            _immunityTags = immunityTags;
        }

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
            
            var mcCue = new MCCue(CueHelper.TryCreateCue(_cueType, _cueParameter));
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
        
        public void Destroy()
        {
            EntityManager.SetComponentEnabled<ECKillCue>(_cueEntity,true);
            var mcCue = EntityManager.GetComponentData<MCCue>(_cueEntity);
            mcCue.cue.OnRemove(Time.time);
            _cueEntity = Entity.Null;
        }
        
        public bool AddToAsc(AbilitySystemCell asc)
        {
           return AddToAsc(asc.Entity);
        }
        
        public bool AddToAsc(Entity asc)
        {
            if (_requiredTags != null)
            {
                if(!ASCUtil.HasAllTags(asc,_requiredTags)) return false;
            }
            
            if (_immunityTags != null)
            {
                if(ASCUtil.HasAnyTags(asc,_immunityTags)) return false;
            }

            var mcCue = EntityManager.GetComponentData<MCCue>(_cueEntity);
            mcCue.cue.AddToTargetAsc(asc);
            return true;
        }
        
        public void RemoveFromAsc()
        {
            var mcCue = EntityManager.GetComponentData<MCCue>(_cueEntity);
            mcCue.cue.RemoveFromTargetAsc();
        }
        
        public void Play()
        {
            EntityManager.SetComponentEnabled<ECCuePlayable>(_cueEntity,true);
        }
        
        public void Stop()
        {
            EntityManager.SetComponentEnabled<ECCuePlayable>(_cueEntity,false);
        }
    }
}