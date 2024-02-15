using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Unity.Math
{
    [TaskCategory("Unity/Math")]
    [TaskDescription("Lerp the float by an amount.")]
    public class Lerp : Action
    {
        [Tooltip("The from value")]
        public SharedFloat fromValue;
        [Tooltip("The to value")]
        public SharedFloat toValue;
        [Tooltip("The amount to lerp")]
        public SharedFloat lerpAmount;
        [Tooltip("The lerp resut")]
        [RequiredField]
        public SharedFloat storeResult;

        public override TaskStatus OnUpdate()
        {
            storeResult.Value = Mathf.Lerp(fromValue.Value, toValue.Value, lerpAmount.Value);
            return TaskStatus.Success;
        }

        public override void OnReset()
        {
            fromValue = 0;
            toValue = 0;
            lerpAmount = 0;
            storeResult = 0;
        }
    }
}