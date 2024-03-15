using System.Collections.Generic;
using GAS.General;
using GAS.Runtime;
using UnityEngine;

namespace GAS.Runtime
{
    public class CatchAreaCircle2D : CatchAreaBase
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
#if UNITY_EDITOR
        public override void OnEditorPreview(GameObject previewObject)
        {
            // 使用Debug 绘制box预览
            float showTime = 1;
            Color color = Color.green;
            var relativeTransform = previewObject.transform;
            var center = offset;
            switch (centerType)
            {
                case EffectCenterType.SelfOffset:
                    center = relativeTransform.position;
                    center.y += relativeTransform.lossyScale.y > 0 ? offset.y : -offset.y;
                    center.x += relativeTransform.lossyScale.x > 0 ? offset.x : -offset.x;
                    break;
                case EffectCenterType.WorldSpace:
                    center = offset;
                    break;
                case EffectCenterType.TargetOffset:
                    //center = _targetCatcher.Target.transform.position;
                    break;
            }

            DebugExtension.DebugDrawCircle(center, radius, color, showTime);
        }
#endif
    }
}