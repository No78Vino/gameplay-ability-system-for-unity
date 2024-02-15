using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Unity.UnityInput
{
    [TaskCategory("Unity/Input")]
    [TaskDescription("Stores the state of the specified button.")]
    public class GetButton : Action
    {
        [Tooltip("The name of the button")]
        public SharedString buttonName;
        [RequiredField]
        [Tooltip("The stored result")]
        public SharedBool storeResult;

        public override TaskStatus OnUpdate()
        {
            storeResult.Value = Input.GetButton(buttonName.Value);
            return TaskStatus.Success;
        }

        public override void OnReset()
        {
            buttonName = "Fire1";
            storeResult = false;
        }
    }
}