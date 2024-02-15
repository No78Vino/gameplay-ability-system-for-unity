using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Unity.UnityAnimator
{
    [TaskCategory("Unity/Animator")]
    [TaskDescription("Converts the state name to its corresponding hash code. Returns Success.")]
    public class GetStringToHash : Action
    {
        [Tooltip("The name of the state to convert to a hash code")]
        public SharedString stateName;
        [Tooltip("The hash value")]
        [RequiredField]
        public SharedInt storeValue;

        public override TaskStatus OnUpdate()
        {
            storeValue.Value = Animator.StringToHash(stateName.Value);

            return TaskStatus.Success;
        }

        public override void OnReset()
        {
            stateName = "";
            storeValue = 0;
        }
    }
}