using System.Collections.Generic;
using GAS.Runtime.Component;
using UnityEngine;

namespace GAS.Runtime.Ability.TargetCatcher
{
    public class CatchAreaCircle2D: CatchAreaBase
    {
        public float radius;
        public Vector2 offset;
        public EffectCenterType centerType;
        
        public void Init(AbilitySystemComponent owner, LayerMask tCheckLayer, Vector2 offset, float radius)
        {
            base.Init(owner, tCheckLayer);
            this.offset = offset;
            this.radius = radius;
        }
        
        public override List<AbilitySystemComponent> CatchTargets(AbilitySystemComponent mainTarget)
        {
            var result = new List<AbilitySystemComponent>();

            Collider2D[] targets = centerType switch
            {
                EffectCenterType.SelfOffset => Owner.OverlapCircle2D(offset, radius, checkLayer),
                EffectCenterType.WorldSpace => Physics2D.OverlapCircleAll(offset, radius, checkLayer),
                EffectCenterType.TargetOffset => mainTarget.OverlapCircle2D(offset, radius, checkLayer),
                _ => null
            };

            if (targets == null) return result;
            foreach (var target in targets)
            {
                var targetUnit = target.GetComponent<AbilitySystemComponent>();
                if (targetUnit != null) result.Add(targetUnit);
            }

            return result;
        }
    }
}