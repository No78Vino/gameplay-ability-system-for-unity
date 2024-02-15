using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Unity.UnityTransform
{
    [TaskCategory("Unity/Transform")]
    [TaskDescription("Moves the transform in the direction and distance of translation. Returns Success.")]
    public class Translate : Action
    {
        [Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
        public SharedGameObject targetGameObject;
        [Tooltip("Move direction and distance")]
        public SharedVector3 translation;
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

            targetTransform.Translate(translation.Value, relativeTo);

            return TaskStatus.Success;
        }

        public override void OnReset()
        {
            targetGameObject = null;
            translation = Vector3.zero;
            relativeTo = Space.Self;
        }
    }
}