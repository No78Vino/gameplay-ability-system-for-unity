using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Unity.UnityCharacterController
{
    [TaskCategory("Unity/CharacterController")]
    [TaskDescription("Sets the slope limit of the CharacterController. Returns Success.")]
    public class SetSlopeLimit : Action
    {
        [Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
        public SharedGameObject targetGameObject;
        [Tooltip("The slope limit of the CharacterController")]
        public SharedFloat slopeLimit;

        private CharacterController characterController;
        private GameObject prevGameObject;

        public override void OnStart()
        {
            var currentGameObject = GetDefaultGameObject(targetGameObject.Value);
            if (currentGameObject != prevGameObject) {
                characterController = currentGameObject.GetComponent<CharacterController>();
                prevGameObject = currentGameObject;
            }
        }

        public override TaskStatus OnUpdate()
        {
            if (characterController == null) {
                Debug.LogWarning("CharacterController is null");
                return TaskStatus.Failure;
            }

            characterController.slopeLimit = slopeLimit.Value;

            return TaskStatus.Success;
        }

        public override void OnReset()
        {
            targetGameObject = null;
            slopeLimit = 0;
        }
    }
}