using System;
using System.Collections.Generic;
using GAS.Editor.Ability;
using GAS.Runtime.Ability;
using GAS.Runtime.Ability.TargetCatcher;
using GAS.Runtime.Component;
using UnityEngine;

namespace Demo.Script.GAS.TargetCatcher
{
    [Serializable]
    public class CatchUndefending:CatchAreaBox2D
    {
        public override List<AbilitySystemComponent> CatchTargets(AbilitySystemComponent mainTarget)
        {
            var targets = base.CatchTargets(mainTarget);
            
            // 没有防御成功的判定：1.没有防御  2.防御了，但是方向错误
            LayerMask defendLayerMask = LayerMask.GetMask("DefendArea");
            Collider2D[] defend = centerType switch
            {
                EffectCenterType.SelfOffset => Owner.OverlapBox2D(offset, size, rotation, defendLayerMask),
                EffectCenterType.WorldSpace => Physics2D.OverlapBoxAll(offset, size, rotation, defendLayerMask),
                EffectCenterType.TargetOffset => mainTarget.OverlapBox2D(offset, size, rotation, defendLayerMask),
                _ => null
            };
            
            var result = new List<AbilitySystemComponent>();
            foreach (var target in targets)
            {
                var targetUnit = target.GetComponent<AbilitySystemComponent>();
                if (targetUnit != null) result.Add(targetUnit);
            }
            return result;
        }
    }
    
    public class CatchUndefendingInspector: CatchAreaBox2DInspector
    {
        public CatchUndefendingInspector(CatchAreaBox2D targetCatcherBase) : base(targetCatcherBase)
        {
        }
    }
}