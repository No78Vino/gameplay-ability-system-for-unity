using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Unity.UnityBoxCollider
{
    [TaskCategory("Unity/BoxCollider")]
    [TaskDescription("Sets the size of the BoxCollider. Returns Success.")]
    public class SetSize : Action
    {
        [Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
        public SharedGameObject targetGameObject;
        [Tooltip("The size of the BoxCollider")]
        public SharedVector3 size;

        private BoxCollider boxCollider;
        private GameObject prevGameObject;

        public override void OnStart()
        {
            var currentGameObject = GetDefaultGameObject(targetGameObject.Value);
            if (currentGameObject != prevGameObject) {
                boxCollider = currentGameObject.GetComponent<BoxCollider>();
                prevGameObject = currentGameObject;
            }
        }

        public override TaskStatus OnUpdate()
        {
            if (boxCollider == null) {
                Debug.LogWarning("BoxCollider is null");
                return TaskStatus.Failure;
            }

            boxCollider.size = size.Value;

            return TaskStatus.Success;
        }

        public override void OnReset()
        {
            targetGameObject = null;
            size = Vector3.zero;
        }
    }
}