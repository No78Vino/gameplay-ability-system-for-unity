using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Unity.UnityGameObject
{
    [TaskCategory("Unity/GameObject")]
    [TaskDescription("Finds a GameObject by name. Returns success if an object is found.")]
    public class Find : Action
    {
        [Tooltip("The GameObject name to find")]
        public SharedString gameObjectName;
        [Tooltip("The object found by name")]
        [RequiredField]
        public SharedGameObject storeValue;

        public override TaskStatus OnUpdate()
        {
            storeValue.Value = GameObject.Find(gameObjectName.Value);

            return storeValue.Value != null ? TaskStatus.Success : TaskStatus.Failure;
        }

        public override void OnReset()
        {
            gameObjectName = null;
            storeValue = null;
        }
    }
}