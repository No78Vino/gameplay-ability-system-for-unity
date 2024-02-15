using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Unity.UnityAnimator
{
    [TaskCategory("Unity/Animator")]
    [TaskDescription("Sets the look at position. Returns Success.")]
    public class SetLookAtPosition : Action
    {
        [Tooltip("The position to lookAt")]
        public SharedVector3 position;

        private Animator animator;
        private bool positionSet;

        public override void OnStart()
        {
            animator = GetComponent<Animator>();
            positionSet = false;
        }

        public override TaskStatus OnUpdate()
        {
            if (animator == null) {
                Debug.LogWarning("Animator is null");
                return TaskStatus.Failure;
            }

            return positionSet ? TaskStatus.Success : TaskStatus.Running;
        }

        public override void OnAnimatorIK()
        {
            if (animator == null) {
                return;
            }
            animator.SetLookAtPosition(position.Value);
            positionSet = true;
        }

        public override void OnReset()
        {
            position = Vector3.zero;
        }
    }
}