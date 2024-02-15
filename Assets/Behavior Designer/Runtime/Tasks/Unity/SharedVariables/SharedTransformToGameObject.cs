using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Unity.SharedVariables
{
    [TaskCategory("Unity/SharedVariable")]
    [TaskDescription("Gets the GameObject from the Transform component. Returns Success.")]
    public class SharedTransformToGameObject : Action
    {
        [Tooltip("The Transform component")]
        public SharedTransform sharedTransform;
        [RequiredField]
        [Tooltip("The GameObject to set")]
        public SharedGameObject sharedGameObject;

        public override TaskStatus OnUpdate()
        {
            if (sharedTransform.Value == null) {
                return TaskStatus.Failure;
            }

            sharedGameObject.Value = sharedTransform.Value.gameObject;

            return TaskStatus.Success;
        }

        public override void OnReset()
        {
            sharedTransform = null;
            sharedGameObject = null;
        }
    }
}