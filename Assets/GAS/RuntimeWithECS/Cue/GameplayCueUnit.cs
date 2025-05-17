using System;
using GAS.Runtime;
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
        
        private static EntityManager EntityManager=>GASManager.EntityManager;
        
        public GameplayCueUnit(Type cueType,ICueParameter parameter)
        {
            _cueType = cueType;
            _cueParameter = parameter;
        }

        public void Create()
        {
            if (_cueEntity != Entity.Null)
            {
#if UNITY_EDITOR
                Debug.LogError($"[EX] Cue已经创建过了，不能重复创建。");
#endif
                return;
            }
            _cueEntity = EntityManager.CreateEntity();
            EntityManager.SetName(_cueEntity,$"Cue_{_cueType.Name}_{_cueEntity.Version}_{_cueEntity.Index}");
            
            var mcCue = new MCCue(CueHelper.TryCreateCue(_cueType, _cueParameter));
            EntityManager.AddComponentData(_cueEntity,mcCue);
            EntityManager.AddComponentData(_cueEntity, new ECCuePlayable());
            EntityManager.AddComponentData(_cueEntity, new ECCuePlaying());
            EntityManager.AddComponentData(_cueEntity, new ECKillCue());
            
            mcCue.cue.OnAdd(Time.time);
        }
        
        public void Play()
        {
            EntityManager.SetComponentEnabled<ECCuePlayable>(_cueEntity,true);
        }
        
        public void Stop()
        {
            EntityManager.SetComponentEnabled<ECCuePlayable>(_cueEntity,false);
        }
        
        public void Destroy()
        {
            EntityManager.SetComponentEnabled<ECKillCue>(_cueEntity,true);
            var mcCue = EntityManager.GetComponentData<MCCue>(_cueEntity);
            mcCue.cue.OnRemove(Time.time);
            _cueEntity = Entity.Null;
        }
    }
}