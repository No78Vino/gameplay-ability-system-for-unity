using UnityEngine;

namespace BehaviorDesigner.Runtime
{
    [System.Serializable]
    public class SharedAnimationCurve : SharedVariable<AnimationCurve>
    {
        public static implicit operator SharedAnimationCurve(AnimationCurve value) { return new SharedAnimationCurve { mValue = value }; }
    }
}