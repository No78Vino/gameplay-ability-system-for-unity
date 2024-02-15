using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Unity.UnityBehaviour
{
    [TaskCategory("Unity/Behaviour")]
    [TaskDescription("Returns Success if the object is enabled, otherwise Failure.")]
    public class IsEnabled : Conditional
    {
        [Tooltip("The Object to use")]
        public SharedObject specifiedObject;

        public override TaskStatus OnUpdate()
        {
            if (specifiedObject == null && !(specifiedObject.Value is UnityEngine.Behaviour)) {
                Debug.LogWarning("SpecifiedObject is null or not a subclass of UnityEngine.Behaviour");
                return TaskStatus.Failure;
            }

            return (specifiedObject.Value as Behaviour).enabled ? TaskStatus.Success : TaskStatus.Failure;
        }

        public override void OnReset()
        {
            if (specifiedObject != null) {
                specifiedObject.Value = null;
            }
        }
    }
}