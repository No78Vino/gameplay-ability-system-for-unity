using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Unity.UnityCapsuleCollider
{
    [TaskCategory("Unity/CapsuleCollider")]
    [TaskDescription("Stores the direction of the CapsuleCollider. Returns Success.")]
    public class GetDirection : Action
    {
        [Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
        public SharedGameObject targetGameObject;
        [Tooltip("The direction of the CapsuleCollider")]
        [RequiredField]
        public SharedInt storeValue;

        private CapsuleCollider capsuleCollider;
        private GameObject prevGameObject;

        public override void OnStart()
        {
            var currentGameObject = GetDefaultGameObject(targetGameObject.Value);
            if (currentGameObject != prevGameObject) {
                capsuleCollider = currentGameObject.GetComponent<CapsuleCollider>();
                prevGameObject = currentGameObject;
            }
        }

        public override TaskStatus OnUpdate()
        {
            if (capsuleCollider == null) {
                Debug.LogWarning("CapsuleCollider is null");
                return TaskStatus.Failure;
            }

            storeValue.Value = capsuleCollider.direction;

            return TaskStatus.Success;
        }

        public override void OnReset()
        {
            targetGameObject = null;
            storeValue = 0;
        }
    }
}