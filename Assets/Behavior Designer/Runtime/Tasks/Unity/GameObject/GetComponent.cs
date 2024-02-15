using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Unity.UnityGameObject
{
    [TaskCategory("Unity/GameObject")]
    [TaskDescription("Returns the component of Type type if the game object has one attached, null if it doesn't. Returns Success.")]
    public class GetComponent : Action
    {
        [Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
        public SharedGameObject targetGameObject;
        [Tooltip("The type of component")]
        public SharedString type;
        [Tooltip("The component")]
        [RequiredField]
        public SharedObject storeValue;

        public override TaskStatus OnUpdate()
        {
            storeValue.Value = GetDefaultGameObject(targetGameObject.Value).GetComponent(type.Value);

            return TaskStatus.Success;
        }

        public override void OnReset()
        {
            targetGameObject = null;
            type.Value = "";
            storeValue.Value = null;
        }
    }
}