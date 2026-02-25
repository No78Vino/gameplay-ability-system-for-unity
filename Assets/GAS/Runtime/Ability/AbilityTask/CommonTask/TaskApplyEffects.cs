using System;  
using System.Collections.Generic;  
using UnityEngine;  
  
namespace GAS.Runtime  
{  
    public class TaskApplyEffects : AbilityTaskBase<XParamApplyEffects>  
    {  
        private TargetCatcherBase _catcher;  
        private List<AbilitySystemComponent> _catchResults = new List<AbilitySystemComponent>();  
  
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
            
            // _catcher.CatchTargetsNonAllocSafe(Spec.Target, ref _catchResults);  
            // foreach (var target in _catchResults)  
            // foreach (var id in Parameter.IDs)  
            //     Owner.ApplyGameplayEffectTo(id, target); // 根据你的 ASC API 调整  
        }
    }  
}