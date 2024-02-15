using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Unity.UnityAnimation
{
    [TaskCategory("Unity/Animation")]
    [TaskDescription("Stores the animate physics value. Returns Success.")]
    public class GetAnimatePhysics : Action
    {
        [Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
        public SharedGameObject targetGameObject;
        [Tooltip("Are the if animations are executed in the physics loop?")]
        [RequiredField]
        public SharedBool storeValue;

        // cache the animation component
        private Animation animation;
        private GameObject prevGameObject;

        public override void OnStart()
        {
            var currentGameObject = GetDefaultGameObject(targetGameObject.Value);
            if (currentGameObject != prevGameObject) {
                animation = currentGameObject.GetComponent<Animation>();
                prevGameObject = currentGameObject;
            }
        }

        public override TaskStatus OnUpdate()
        {
            if (animation == null) {
                Debug.LogWarning("Animation is null");
                return TaskStatus.Failure;
            }

            storeValue.Value = animation.animatePhysics;

            return TaskStatus.Success;
        }

        public override void OnReset()
        {
            targetGameObject = null;
            storeValue.Value = false;
        }
    }
}