using System;  
using System.Collections.Generic;  
using UnityEngine;  
  
namespace GAS.Runtime  
{  
    public class TaskApplyEffects : AbilityTaskBase<XParamApplyEffects>  
    {  
        private TargetCatcherBase _catcher;  
        private List<AbilitySystemCell> _catchResults = new List<AbilitySystemCell>();  
  
        public TaskApplyEffects(AbilityLogicBase logic) : base(logic)  
        {  
        }  
  
        public override void InitParameters(XParam parameter)  
        {  
            base.InitParameters(parameter);  
            if (Parameter == null) return;  
  
            if (!string.IsNullOrEmpty(Parameter.CatcherType))  
            {  
                _catcher = TargetCatcherHelper.TryCreateTargetCatcher(Parameter.CatcherType);  
                if (_catcher != null)  
                {  
                    _catcher.Init(Owner); // Owner 来自 AbilityTaskBase  
                    if (Parameter.Param != null)  
                        _catcher.InitParameters(Parameter.Param);  
                }  
            }  
        }  
  
        protected override void OnBegin(int startFrame)  
        {  
            if (_catcher == null || Parameter?.IDs == null) return;  
            
            _catcher.CatchTargetsNonAllocSafe(Owner, ref _catchResults);  
            foreach (var target in _catchResults)  
            {  
                foreach (var id in Parameter.IDs)  
                {  
                    var effectCfg = GameplayEffectHelper.GetConfigByID(id);  
                    var geEntity = EffectUtil.CreateGameplayEffectEntity(effectCfg.ComponentConfigs);  
                    EffectUtil.ApplyGameplayEffectTo(geEntity, target.Entity, Owner.Entity);  
                }  
            }  
        }
    }  
}