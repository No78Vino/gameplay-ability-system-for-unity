using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Unity.UnityAnimation
{
    [TaskCategory("Unity/Animation")]
    [TaskDescription("Sets the wrap mode to the specified value. Returns Success.")]
    public class SetWrapMode : Action
    {
        [Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
        public SharedGameObject targetGameObject;
        [Tooltip("How should time beyond the playback range of the clip be treated?")]
        public WrapMode wrapMode = WrapMode.Default;

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

            animation.wrapMode = wrapMode;

            return TaskStatus.Success;
        }

        public override void OnReset()
        {
            targetGameObject = null;
            wrapMode = WrapMode.Default;
        }
    }
}