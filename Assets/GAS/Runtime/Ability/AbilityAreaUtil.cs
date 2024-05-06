using System;
using UnityEngine;

namespace GAS.Runtime
{
    public enum EffectCenterType
    {
        SelfOffset,
        WorldSpace,
        TargetOffset
    }

    public static class AbilityAreaUtil
    {
        [Obsolete("请使用OverlapBox2DNonAlloc方法来避免产生垃圾收集（GC）。")]
        public static Collider2D[] OverlapBox2D(this AbilitySystemComponent asc, Vector2 offset, Vector2 size,
            float angle, int layerMask, Transform relativeTransform = null)
        {
            relativeTransform ??= asc.transform;
            var center = (Vector2)relativeTransform.position;
            offset.x *= relativeTransform.lossyScale.x > 0 ? 1 : -1;
            center += offset;
            angle += asc.transform.eulerAngles.z;

            return Physics2D.OverlapBoxAll(center, size, angle, layerMask);
        }

        public static int OverlapBox2DNonAlloc(this AbilitySystemComponent asc, Vector2 offset, Vector2 size,
            float angle, Collider2D[] results, int layerMask, Transform relativeTransform = null)
        {
            relativeTransform ??= asc.transform;
            var center = (Vector2)relativeTransform.position;
            offset.x *= relativeTransform.lossyScale.x > 0 ? 1 : -1;
            center += offset;
            angle += asc.transform.eulerAngles.z;

            var count = Physics2D.OverlapBoxNonAlloc(center, size, angle, results, layerMask);
            return count;
        }

        public static int TimelineAbilityOverlapBox2D(this TimelineAbilitySpec spec,
            Vector2 offset, Vector2 size, float angle, int layerMask, Collider2D[] results,
            EffectCenterType centerType, Transform relativeTransform = null)
        {
            return centerType switch
            {
                EffectCenterType.SelfOffset => spec.Owner.OverlapBox2DNonAlloc(offset, size, angle, results, layerMask, relativeTransform),
                EffectCenterType.WorldSpace => Physics2D.OverlapBoxNonAlloc(offset, size, angle, results, layerMask),
                EffectCenterType.TargetOffset => spec.Target.OverlapBox2DNonAlloc(offset, size, angle, results, layerMask, relativeTransform),
                _ => 0
            };
        }

        [Obsolete("请使用OverlapCircle2DNonAlloc方法来避免产生垃圾收集（GC）。")]
        public static Collider2D[] OverlapCircle2D(this AbilitySystemComponent asc, Vector2 offset, float radius,
            int layerMask, Transform relativeTransform = null)
        {
            relativeTransform ??= asc.transform;
            var center = (Vector2)relativeTransform.position;
            offset.x *= relativeTransform.lossyScale.x > 0 ? 1 : -1;
            center += offset;

            return Physics2D.OverlapCircleAll(center, radius, layerMask);
        }

        public static int OverlapCircle2DNonAlloc(this AbilitySystemComponent asc, Vector2 offset, float radius,
            Collider2D[] results, int layerMask, Transform relativeTransform = null)
        {
            relativeTransform ??= asc.transform;
            var center = (Vector2)relativeTransform.position;
            offset.x *= relativeTransform.lossyScale.x > 0 ? 1 : -1;
            center += offset;

            var count = Physics2D.OverlapCircleNonAlloc(center, radius, results, layerMask);
            return count;
        }
    }
}