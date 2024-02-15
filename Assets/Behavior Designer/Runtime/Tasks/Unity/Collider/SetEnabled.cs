using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Unity.Collider
{
    [TaskCategory("Unity/Collider")]
    [TaskDescription("Enables/Disables the collider. Returns Success.")]
    public class SetEnabled : Action
    {
        [Tooltip("The Behavior to use")]
        public SharedCollider specifiedCollider;
        [Tooltip("The enabled/disabled state")]
        public SharedBool enabled;

        public override TaskStatus OnUpdate()
        {
            if (specifiedCollider == null) {
                Debug.LogWarning("SpecifiedCollider is null");
                return TaskStatus.Failure;
            }

            specifiedCollider.Value.enabled = enabled.Value;

            return TaskStatus.Success;
        }

        public override void OnReset()
        {
            specifiedCollider.Value = null;
            enabled = false;
        }
    }
}