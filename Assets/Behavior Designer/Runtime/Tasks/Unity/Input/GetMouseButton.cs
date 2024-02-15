using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Unity.UnityInput
{
    [TaskCategory("Unity/Input")]
    [TaskDescription("Stores the state of the specified mouse button.")]
    public class GetMouseButton : Action
    {
        [Tooltip("The index of the button")]
        public SharedInt buttonIndex;
        [RequiredField]
        [Tooltip("The stored result")]
        public SharedBool storeResult;

        public override TaskStatus OnUpdate()
        {
            storeResult.Value = Input.GetMouseButton(buttonIndex.Value);
            return TaskStatus.Success;
        }

        public override void OnReset()
        {
            buttonIndex = 0;
            storeResult = false;
        }
    }
}