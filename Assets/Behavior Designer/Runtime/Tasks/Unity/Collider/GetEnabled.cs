using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Unity.Collider
{
    [TaskCategory("Unity/Collider")]
    [TaskDescription("Stores the enabled state of the collider. Returns Success.")]
    public class GetEnabled : Action
    {
        [Tooltip("The Collider to use")]
        public SharedCollider specifiedCollider;
        [Tooltip("The enabled/disabled state")]
        [RequiredField]
        public SharedBool storeValue;

        public override TaskStatus OnUpdate()
        {
            if (specifiedCollider == null) {
                Debug.LogWarning("SpecifiedObject is null");
                return TaskStatus.Failure;
            }

            storeValue.Value = specifiedCollider.Value.enabled;

            return TaskStatus.Success;
        }

        public override void OnReset()
        {
            specifiedCollider.Value = null;
            storeValue = false;
        }
    }
}