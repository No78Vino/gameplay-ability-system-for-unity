using System.Collections.Generic;
using GAS.Runtime.Component;
using UnityEngine;
using UnityEngine.UIElements;

namespace GAS.Runtime.Ability.TargetCatcher
{
    public class CatchAreaBox2D : CatchAreaBase
    {
        public Vector2 offset;
        public float rotation;
        public Vector2 size;

        public void Init(AbilitySystemComponent owner, LayerMask checkLayer, Vector2 offset, Vector2 size,
            float rotation)
        {
            base.Init(owner, checkLayer);
            this.offset = offset;
            this.size = size;
            this.rotation = rotation;
        }

        public override List<AbilitySystemComponent> CatchTargets(AbilitySystemComponent mainTarget)
        {
            var targets = new List<AbilitySystemComponent>();
            var results = new Collider2D[100];
            var size1 = Physics2D.OverlapBoxNonAlloc(Owner.transform.position + (Vector3)offset, size, rotation,
                results, checkLayer);
            for (var i = 0; i < size1; i++)
            {
                var result = results[i];
                var target = result.GetComponent<AbilitySystemComponent>();
                if (target != null) targets.Add(target);
            }

            return targets;
        }
    }
}