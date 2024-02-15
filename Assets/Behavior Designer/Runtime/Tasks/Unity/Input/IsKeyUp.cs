using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Unity.UnityInput
{
    [TaskCategory("Unity/Input")]
    [TaskDescription("Returns success when the specified key is released.")]
    public class IsKeyUp : Conditional
    {
        [Tooltip("The key to test")]
        public KeyCode key;

        public override TaskStatus OnUpdate()
        {
            return Input.GetKeyUp(key) ? TaskStatus.Success : TaskStatus.Failure;
        }

        public override void OnReset()
        {
            key = KeyCode.None;
        }
    }
}