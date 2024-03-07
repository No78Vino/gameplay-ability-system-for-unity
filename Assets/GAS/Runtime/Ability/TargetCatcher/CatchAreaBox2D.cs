using System;
using System.Collections.Generic;
using GAS.Runtime.Component;
using UnityEngine;
using UnityEngine.UIElements;

namespace GAS.Runtime.Ability.TargetCatcher
{
    [Serializable]
    public class CatchAreaBox2D : CatchAreaBase
    {
        public Vector2 offset;
        public float rotation;
        public Vector2 size;
        public EffectCenterType centerType;
        
        public void Init(AbilitySystemComponent owner, LayerMask tCheckLayer, Vector2 offset, Vector2 size,
            float rotation)
        {
            base.Init(owner, tCheckLayer);
            this.offset = offset;
            this.size = size;
            this.rotation = rotation;
        }

        public override List<AbilitySystemComponent> CatchTargets(AbilitySystemComponent mainTarget)
        {
            var result = new List<AbilitySystemComponent>();

            Collider2D[] targets = centerType switch
            {
                EffectCenterType.SelfOffset => Owner.OverlapBox2D(offset, size, rotation, checkLayer),
                EffectCenterType.WorldSpace => Physics2D.OverlapBoxAll(offset, size, rotation, checkLayer),
                EffectCenterType.TargetOffset => mainTarget.OverlapBox2D(offset, size, rotation, checkLayer),
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