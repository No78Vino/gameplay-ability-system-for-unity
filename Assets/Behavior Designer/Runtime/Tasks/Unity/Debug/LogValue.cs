using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Unity.UnityDebug
{
    [TaskCategory("Unity/Debug")]
    [TaskDescription("Log a variable value.")]
    public class LogValue : Action
    {
        [Tooltip("The variable to output")]
        public SharedGenericVariable variable;

        public override TaskStatus OnUpdate()
        {
            Debug.Log(variable.Value.value.GetValue());

            return TaskStatus.Success;
        }

        public override void OnReset()
        {
            variable = null;
        }
    }
}