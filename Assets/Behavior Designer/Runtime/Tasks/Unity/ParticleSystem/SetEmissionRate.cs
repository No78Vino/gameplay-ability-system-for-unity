using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Unity.UnityParticleSystem
{
    [TaskCategory("Unity/ParticleSystem")]
    [TaskDescription("Sets the emission rate of the Particle System.")]
    public class SetEmissionRate : Action
    {
        [Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
        public SharedGameObject targetGameObject;
        [Tooltip("The emission rate of the ParticleSystem")]
        public SharedFloat emissionRate;

        private ParticleSystem particleSystem;
        private GameObject prevGameObject;

        public override void OnStart()
        {
            var currentGameObject = GetDefaultGameObject(targetGameObject.Value);
            if (currentGameObject != prevGameObject) {
                particleSystem = currentGameObject.GetComponent<ParticleSystem>();
                prevGameObject = currentGameObject;
            }
        }

        public override TaskStatus OnUpdate()
        {
            if (particleSystem == null) {
                Debug.LogWarning("ParticleSystem is null");
                return TaskStatus.Failure;
            }

#if !(UNITY_5_1 || UNITY_5_2)
            Debug.Log("Warning: SetEmissionRate is not used in Unity 5.3 or later.");
#else
            particleSystem.emissionRate = emissionRate.Value;
#endif

            return TaskStatus.Success;
        }

        public override void OnReset()
        {
            targetGameObject = null;
            emissionRate = 0;
        }
    }
}