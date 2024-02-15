using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Unity.UnityAnimation
{
    [TaskCategory("Unity/Animation")]
    [TaskDescription("Cross fades an animation after previous animations has finished playing. Returns Success.")]
    public class CrossFadeQueued : Action
    {
        [Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
        public SharedGameObject targetGameObject;
        [Tooltip("The name of the animation")]
        public SharedString animationName;
        [Tooltip("The amount of time it takes to blend")]
        public float fadeLength = 0.3f;
        [Tooltip("Specifies when the animation should start playing")]
        public QueueMode queue = QueueMode.CompleteOthers;
        [Tooltip("The play mode of the animation")]
        public PlayMode playMode = PlayMode.StopSameLayer;

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

            animation.CrossFadeQueued(animationName.Value, fadeLength, queue, playMode);

            return TaskStatus.Success;
        }

        public override void OnReset()
        {
            targetGameObject = null;
            animationName.Value = "";
            fadeLength = 0.3f;
            queue = QueueMode.CompleteOthers;
            playMode = PlayMode.StopSameLayer;
        }
    }
}