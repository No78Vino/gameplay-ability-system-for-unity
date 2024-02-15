using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Unity.UnityAnimator
{
    [TaskCategory("Unity/Animator")]
    [TaskDescription("Plays an animator state. Returns Success.")]
    public class Play : Action
    {
        [Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
        public SharedGameObject targetGameObject;
        [Tooltip("The name of the state")]
        public SharedString stateName;
        [Tooltip("The layer where the state is")]
        public int layer = -1;
        [Tooltip("The normalized time at which the state will play")]
        public float normalizedTime = float.NegativeInfinity;

        private Animator animator;
        private GameObject prevGameObject;

        public override void OnStart()
        {
            var currentGameObject = GetDefaultGameObject(targetGameObject.Value);
            if (currentGameObject != prevGameObject) {
                animator = currentGameObject.GetComponent<Animator>();
                prevGameObject = currentGameObject;
            }
        }

        public override TaskStatus OnUpdate()
        {
            if (animator == null) {
                Debug.LogWarning("Animator is null");
                return TaskStatus.Failure;
            }

            animator.Play(stateName.Value, layer, normalizedTime);

            return TaskStatus.Success;
        }

        public override void OnReset()
        {
            targetGameObject = null;
            stateName = "";
            layer = -1;
            normalizedTime = float.NegativeInfinity;
        }
    }
}
