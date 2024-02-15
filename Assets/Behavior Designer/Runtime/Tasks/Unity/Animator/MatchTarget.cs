using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Unity.UnityAnimator
{
    [TaskCategory("Unity/Animator")]
    [TaskDescription("Automatically adjust the gameobject position and rotation so that the AvatarTarget reaches the matchPosition when the current state is at the specified progress. Returns Success.")]
    public class MatchTarget : Action
    {
        [Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
        public SharedGameObject targetGameObject;
        [Tooltip("The position we want the body part to reach")]
        public SharedVector3 matchPosition;
        [Tooltip("The rotation in which we want the body part to be")]
        public SharedQuaternion matchRotation;
        [Tooltip("The body part that is involved in the match")]
        public AvatarTarget targetBodyPart;
        [Tooltip("Weights for matching position")]
        public Vector3 weightMaskPosition;
        [Tooltip("Weights for matching rotation")]
        public float weightMaskRotation;
        [Tooltip("Start time within the animation clip")]
        public float startNormalizedTime;
        [Tooltip("End time within the animation clip")]
        public float targetNormalizedTime = 1;

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

            animator.MatchTarget(matchPosition.Value, matchRotation.Value, targetBodyPart, new MatchTargetWeightMask(weightMaskPosition, weightMaskRotation), startNormalizedTime, targetNormalizedTime);

            return TaskStatus.Success;
        }

        public override void OnReset()
        {
            targetGameObject = null;
            matchPosition = Vector3.zero;
            matchRotation = Quaternion.identity;
            targetBodyPart = AvatarTarget.Root;
            weightMaskPosition = Vector3.zero;
            weightMaskRotation = 0;
            startNormalizedTime = 0;
            targetNormalizedTime = 1;
        }
    }
}