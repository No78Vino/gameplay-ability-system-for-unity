using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Unity.UnityInput
{
    [TaskCategory("Unity/Input")]
    [TaskDescription("Stores the pressed state of the specified key.")]
    public class GetKey : Action
    {
        [Tooltip("The key to test.")]
        public KeyCode key;
        [RequiredField]
        [Tooltip("The stored result")]
        public SharedBool storeResult;

        public override TaskStatus OnUpdate()
        {
            storeResult.Value = Input.GetKey(key);
            return TaskStatus.Success;
        }

        public override void OnReset()
        {
            key = KeyCode.None;
            storeResult = false;
        }
    }
}