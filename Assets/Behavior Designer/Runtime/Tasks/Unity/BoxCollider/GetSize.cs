using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Unity.UnityBoxCollider
{
    [TaskCategory("Unity/BoxCollider")]
    [TaskDescription("Stores the size of the BoxCollider. Returns Success.")]
    public class GetSize : Action
    {
        [Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
        public SharedGameObject targetGameObject;
        [Tooltip("The size of the BoxCollider")]
        [RequiredField]
        public SharedVector3 storeValue;

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

            storeValue.Value = boxCollider.size;

            return TaskStatus.Success;
        }

        public override void OnReset()
        {
            targetGameObject = null;
            storeValue = Vector3.zero;
        }
    }
}