using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Unity.UnityRenderer
{
    [TaskCategory("Unity/Renderer")]
    [TaskDescription("Sets the material on the Renderer.")]
    public class SetMaterial : Action
    {
        [Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
        public SharedGameObject targetGameObject;
        [Tooltip("The material to set")]
        public SharedMaterial material;

        // cache the renderer component
        private Renderer renderer;
        private GameObject prevGameObject;

        public override void OnStart()
        {
            var currentGameObject = GetDefaultGameObject(targetGameObject.Value);
            if (currentGameObject != prevGameObject) {
                renderer = currentGameObject.GetComponent<Renderer>();
                prevGameObject = currentGameObject;
            }
        }

        public override TaskStatus OnUpdate()
        {
            if (renderer == null) {
                Debug.LogWarning("Renderer is null");
                return TaskStatus.Failure;
            }

            renderer.material = material.Value;
            return TaskStatus.Success;
        }

        public override void OnReset()
        {
            targetGameObject = null;
            material = null;
        }
    }
}