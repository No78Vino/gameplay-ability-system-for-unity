using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Unity.UnityInput
{
    [TaskCategory("Unity/Input")]
    [TaskDescription("Returns success when the specified button is pressed.")]
    public class IsButtonDown : Conditional
    {
        [Tooltip("The name of the button")]
        public SharedString buttonName;

        public override TaskStatus OnUpdate()
        {
            return Input.GetButtonDown(buttonName.Value) ? TaskStatus.Success : TaskStatus.Failure;
        }

        public override void OnReset()
        {
            buttonName = "Fire1";
        }
    }
}