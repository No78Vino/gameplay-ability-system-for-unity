using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Unity.UnityGameObject
{
    [TaskCategory("Unity/GameObject")]
    [TaskDescription("Destorys the specified GameObject immediately. Returns Success.")]
    public class DestroyImmediate : Action
    {
        [Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
        public SharedGameObject targetGameObject;

        public override TaskStatus OnUpdate()
        {
            var destroyGameObject = GetDefaultGameObject(targetGameObject.Value);
            GameObject.DestroyImmediate(destroyGameObject);

            return TaskStatus.Success;
        }

        public override void OnReset()
        {
            targetGameObject = null;
        }
    }
}