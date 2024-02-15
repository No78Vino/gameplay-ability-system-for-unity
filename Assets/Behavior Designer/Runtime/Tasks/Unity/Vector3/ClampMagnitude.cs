using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Unity.UnityVector3
{
    [TaskCategory("Unity/Vector3")]
    [TaskDescription("Clamps the magnitude of the Vector3.")]
    public class ClampMagnitude : Action
    {
        [Tooltip("The Vector3 to clamp the magnitude of")]
        public SharedVector3 vector3Variable;
        [Tooltip("The max length of the magnitude")]
        public SharedFloat maxLength;
        [Tooltip("The clamp magnitude resut")]
        [RequiredField]
        public SharedVector3 storeResult;

        public override TaskStatus OnUpdate()
        {
            storeResult.Value = Vector3.ClampMagnitude(vector3Variable.Value, maxLength.Value);
            return TaskStatus.Success;
        }

        public override void OnReset()
        {
            vector3Variable = Vector3.zero;
            storeResult = Vector3.zero;
            maxLength = 0;
        }
    }
}