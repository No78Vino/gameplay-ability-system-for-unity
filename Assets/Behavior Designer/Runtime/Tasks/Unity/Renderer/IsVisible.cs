using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Unity.UnityRenderer
{
    [TaskCategory("Unity/Renderer")]
    [TaskDescription("Returns Success if the Renderer is visible, otherwise Failure.")]
    public class IsVisible : Conditional
    {
        [Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
        public SharedGameObject targetGameObject;

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

            return renderer.isVisible ? TaskStatus.Success : TaskStatus.Failure;
        }

        public override void OnReset()
        {
            targetGameObject = null;
        }
    }
}