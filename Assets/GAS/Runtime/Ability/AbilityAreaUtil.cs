using GAS.Runtime;
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
        public static Collider2D[] OverlapBox2D(this AbilitySystemComponent asc, Vector2 offset, Vector2 size,
            float angle, int layerMask,Transform relativeTransform = null)
        {
            relativeTransform ??= asc.transform;
            var center = (Vector2)relativeTransform.position;
            offset.x *= relativeTransform.lossyScale.x > 0 ? 1 : -1;
            center += offset;
            angle += asc.transform.eulerAngles.z;

            return Physics2D.OverlapBoxAll(center, size, angle, layerMask);
        }

        public static Collider2D[] TimelineAbilityOverlapBox2D(this TimelineAbilitySpec spec, Vector2 offset,
            Vector2 size,
            float angle, int layerMask, EffectCenterType centerType,Transform relativeTransform = null)
        {
            switch (centerType)
            {
                case EffectCenterType.SelfOffset:
                    return spec.Owner.OverlapBox2D(offset, size, angle, layerMask,relativeTransform);
                case EffectCenterType.WorldSpace:
                    return Physics2D.OverlapBoxAll(offset, size, angle, layerMask);
                case EffectCenterType.TargetOffset:
                    return spec.Target.OverlapBox2D(offset, size, angle, layerMask,relativeTransform);
            }

            return null;
        }
        
        public static Collider2D[] OverlapCircle2D(this AbilitySystemComponent asc, Vector2 offset, float radius,
            int layerMask,Transform relativeTransform = null)
        {
            relativeTransform ??= asc.transform;
            var center = (Vector2)relativeTransform.position;
            offset.x *= relativeTransform.lossyScale.x > 0 ? 1 : -1;
            center += offset;

            return Physics2D.OverlapCircleAll(center, radius, layerMask);
        }
    }
}