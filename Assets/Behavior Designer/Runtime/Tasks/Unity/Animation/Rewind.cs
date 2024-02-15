using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Unity.UnityAnimation
{
    [TaskCategory("Unity/Animation")]
    [TaskDescription("Rewinds an animation. Rewinds all animations if animationName is blank. Returns Success.")]
    public class Rewind : Action
    {
        [Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
        public SharedGameObject targetGameObject;
        [Tooltip("The name of the animation")]
        public SharedString animationName;

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

            if (string.IsNullOrEmpty(animationName.Value)) {
                animation.Rewind();
            } else {
                animation.Rewind(animationName.Value);
            }

            return TaskStatus.Success;
        }

        public override void OnReset()
        {
            targetGameObject = null;
            animationName.Value = "";
        }
    }
}