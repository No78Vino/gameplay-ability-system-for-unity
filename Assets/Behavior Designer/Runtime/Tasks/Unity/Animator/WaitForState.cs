using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Unity.UnityAnimator
{
    [TaskCategory("Unity/Animator")]
    [TaskDescription("Waits for the Animator to reach the specified state.")]
    public class WaitForState : Action
    {
        [Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
        public SharedGameObject targetGameObject;
        [Tooltip("The name of the state")]
        public SharedString stateName;
        [Tooltip("The layer where the state is")]
        public SharedInt layer = -1;

        private Animator animator;
        private GameObject prevGameObject;
        private int stateHash;

        public override void OnAwake()
        {
            stateHash = Animator.StringToHash(stateName.Value);
        }

        public override void OnStart()
        {
            var currentGameObject = GetDefaultGameObject(targetGameObject.Value);
            if (currentGameObject != prevGameObject) {
                animator = currentGameObject.GetComponent<Animator>();
                prevGameObject = currentGameObject;

                if (!animator.HasState(layer.Value, stateHash)) {
                    Debug.LogError("Error: The Animator does not have the state " + stateName.Value + " on layer " + layer.Value);
                }
            }
        }

        public override TaskStatus OnUpdate()
        {
            if (animator == null) {
                Debug.LogWarning("Animator is null");
                return TaskStatus.Failure;
            }

            var state = animator.GetCurrentAnimatorStateInfo(layer.Value);
            if (state.shortNameHash == stateHash) {
                return TaskStatus.Success;
            }

            return TaskStatus.Running;
        }

        public override void OnReset()
        {
            targetGameObject = null;
            stateName = "";
            layer = -1;
        }
    }
}
