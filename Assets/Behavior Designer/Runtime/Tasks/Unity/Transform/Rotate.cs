using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Unity.UnityTransform
{
    [TaskCategory("Unity/Transform")]
    [TaskDescription("Applies a rotation. Returns Success.")]
    public class Rotate : Action
    {
        [Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
        public SharedGameObject targetGameObject;
        [Tooltip("Amount to rotate")]
        public SharedVector3 eulerAngles;
        [Tooltip("Specifies which axis the rotation is relative to")]
        public Space relativeTo = Space.Self;

        private Transform targetTransform;
        private GameObject prevGameObject;

        public override void OnStart()
        {
            var currentGameObject = GetDefaultGameObject(targetGameObject.Value);
            if (currentGameObject != prevGameObject) {
                targetTransform = currentGameObject.GetComponent<Transform>();
                prevGameObject = currentGameObject;
            }
        }

        public override TaskStatus OnUpdate()
        {
            if (targetTransform == null) {
                Debug.LogWarning("Transform is null");
                return TaskStatus.Failure;
            }

            targetTransform.Rotate(eulerAngles.Value, relativeTo);

            return TaskStatus.Success;
        }

        public override void OnReset()
        {
            targetGameObject = null;
            eulerAngles = Vector3.zero;
            relativeTo = Space.Self;
        }
    }
}