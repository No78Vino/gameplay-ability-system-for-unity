using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Unity.UnityBehaviour
{
    [TaskCategory("Unity/Behaviour")]
    [TaskDescription("Enables/Disables the object. Returns Success.")]
    public class SetEnabled : Action
    {
        [Tooltip("The Behavior to use")]
        public SharedBehaviour specifiedObject;
        [Tooltip("The enabled/disabled state")]
        public SharedBool enabled;

        public override TaskStatus OnUpdate()
        {
            if (specifiedObject == null) {
                Debug.LogWarning("SpecifiedObject is null");
                return TaskStatus.Failure;
            }

            specifiedObject.Value.enabled = enabled.Value;

            return TaskStatus.Success;
        }

        public override void OnReset()
        {
            specifiedObject.Value = null;
            enabled = false;
        }
    }
}