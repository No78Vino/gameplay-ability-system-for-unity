using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Unity.UnityGameObject
{
    [TaskCategory("Unity/GameObject")]
    [TaskDescription("Finds a GameObject by tag. Returns success if an object is found.")]
    public class FindWithTag : Action
    {
        [Tooltip("The tag of the GameObject to find")]
        public SharedString tag;
        [Tooltip("Should a random GameObject be found?")]
        public SharedBool random;
        [Tooltip("The object found by name")]
        [RequiredField]
        public SharedGameObject storeValue;

        public override TaskStatus OnUpdate()
        {
            if (random.Value) {
                var gameObjects = GameObject.FindGameObjectsWithTag(tag.Value);
                if (gameObjects == null || gameObjects.Length == 0) { return TaskStatus.Failure; }
                storeValue.Value = gameObjects[Random.Range(0, gameObjects.Length)];
            } else {
                storeValue.Value = GameObject.FindWithTag(tag.Value);
            }

            return storeValue.Value != null ? TaskStatus.Success : TaskStatus.Failure;
        }

        public override void OnReset()
        {
            tag.Value = null;
            storeValue.Value = null;
        }
    }
}