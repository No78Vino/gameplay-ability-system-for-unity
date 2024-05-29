using System;
using System.Collections.Generic;
using GAS.General;
using UnityEngine;

namespace GAS.Runtime
{
    [Serializable]
    public sealed class CatchAreaBox2D : CatchAreaBase
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

        private static readonly Collider2D[] Collider2Ds = new Collider2D[32];

        protected override void CatchTargetsNonAlloc(AbilitySystemComponent mainTarget, List<AbilitySystemComponent> results)
        {
            int count = centerType switch
            {
                EffectCenterType.SelfOffset => Owner.OverlapBox2DNonAlloc(offset, size, rotation, Collider2Ds, checkLayer),
                EffectCenterType.WorldSpace => Physics2D.OverlapBoxNonAlloc(offset, size, rotation, Collider2Ds, checkLayer),
                EffectCenterType.TargetOffset => mainTarget.OverlapBox2DNonAlloc(offset, size, rotation, Collider2Ds, checkLayer),
                _ => 0
            };

            for (var i = 0; i < count; ++i)
            {
                var targetUnit = Collider2Ds[i].GetComponent<AbilitySystemComponent>();
                if (targetUnit != null)
                {
                    results.Add(targetUnit);
                }
            }
        }

#if UNITY_EDITOR
        public override void OnEditorPreview(GameObject previewObject)
        {
            // 使用Debug 绘制box预览
            float showTime = 1;
            Color color = Color.green;
            var relativeTransform = previewObject.transform;
            var center = offset;
            var angle = rotation + relativeTransform.eulerAngles.z;
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
                    //center = _spec.Target.transform.position + (Vector3)_task.Offset;
                    break;
            }

            DebugExtension.DebugBox(center, size, angle, color, showTime);
        }
#endif
    }
}