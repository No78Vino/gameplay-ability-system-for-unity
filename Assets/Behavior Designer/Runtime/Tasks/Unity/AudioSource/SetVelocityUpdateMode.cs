using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Unity.UnityAudioSource
{
    [TaskCategory("Unity/AudioSource")]
    [TaskDescription("Sets the rolloff mode of the AudioSource. Returns Success.")]
    public class SetVelocityUpdateMode : Action
    {
        [Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
        public SharedGameObject targetGameObject;
        [Tooltip("The velocity update mode of the AudioSource")]
        public AudioVelocityUpdateMode velocityUpdateMode;

        private AudioSource audioSource;
        private GameObject prevGameObject;

        public override void OnStart()
        {
            var currentGameObject = GetDefaultGameObject(targetGameObject.Value);
            if (currentGameObject != prevGameObject) {
                audioSource = currentGameObject.GetComponent<AudioSource>();
                prevGameObject = currentGameObject;
            }
        }

        public override TaskStatus OnUpdate()
        {
            if (audioSource == null) {
                Debug.LogWarning("AudioSource is null");
                return TaskStatus.Failure;
            }

            audioSource.velocityUpdateMode = velocityUpdateMode;

            return TaskStatus.Success;
        }

        public override void OnReset()
        {
            targetGameObject = null;
            velocityUpdateMode = AudioVelocityUpdateMode.Auto;
        }
    }
}