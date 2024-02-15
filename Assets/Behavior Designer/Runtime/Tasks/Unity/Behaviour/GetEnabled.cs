using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Unity.UnityBehaviour
{
    [TaskCategory("Unity/Behaviour")]
    [TaskDescription("Stores the enabled state of the object. Returns Success.")]
    public class GetEnabled : Action
    {
        [Tooltip("The Behavior to use")]
        public SharedBehaviour specifiedObject;
        [Tooltip("The enabled/disabled state")]
        [RequiredField]
        public SharedBool storeValue;

        public override TaskStatus OnUpdate()
        {
            if (specifiedObject == null) {
                Debug.LogWarning("SpecifiedObject is null");
                return TaskStatus.Failure;
            }

            storeValue.Value = specifiedObject.Value.enabled;

            return TaskStatus.Success;
        }

        public override void OnReset()
        {
            specifiedObject.Value = null;
            storeValue = false;
        }
    }
}