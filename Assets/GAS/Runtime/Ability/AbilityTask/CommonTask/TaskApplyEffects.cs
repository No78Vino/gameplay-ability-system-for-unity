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

            if (string.IsNullOrEmpty(Parameter.CatcherType)) return;
            
            _catcher = TargetCatcherHelper.TryCreateTargetCatcher(Parameter.CatcherType);  
            if (_catcher != null)
            {
                if (Parameter.Param != null)  
                    _catcher.InitParameters(Parameter.Param);  
            }
        }  
  
        protected override void OnBegin(int startFrame)  
        {  
            if (_catcher == null || Parameter?.IDs == null) return;  
            
            // 延迟初始化：确保 Owner 已被正确设置  
            if (_catcher.Owner == null)  
                _catcher.Init(Owner);  
            
            _catcher.CatchTargetsNonAllocSafe(Owner, ref _catchResults);  
            foreach (var target in _catchResults)  
            {  
                foreach (var id in Parameter.IDs)  
                {  
                    var effectCfg = GameplayEffectHelper.GetConfigByID(id);  
                    var geEntity = GameplayEffectHelper.CreateGameplayEffectEntity(effectCfg.ComponentConfigs);  
                    GameplayEffectHelper.ApplyGameplayEffectTo(geEntity, target.Entity, Owner.Entity);  
                }  
            }  
        }

        public override void OnEditorPreview(GameObject target, int frame, int startFrame, int endFrame)
        {
            base.OnEditorPreview(target, frame, startFrame, endFrame);
            _catcher.OnEditorPreview(target);
        }
    }  
}