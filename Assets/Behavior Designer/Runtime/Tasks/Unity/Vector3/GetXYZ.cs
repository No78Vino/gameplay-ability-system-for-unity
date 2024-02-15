using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Unity.UnityVector3
{
    [TaskCategory("Unity/Vector3")]
    [TaskDescription("Stores the X, Y, and Z values of the Vector3.")]
    public class GetXYZ : Action
    {
        [Tooltip("The Vector3 to get the values of")]
        public SharedVector3 vector3Variable;
        [Tooltip("The X value")]
        [RequiredField]
        public SharedFloat storeX;
        [Tooltip("The Y value")]
        [RequiredField]
        public SharedFloat storeY;
        [Tooltip("The Z value")]
        [RequiredField]
        public SharedFloat storeZ;

        public override TaskStatus OnUpdate()
        {
            storeX.Value = vector3Variable.Value.x;
            storeY.Value = vector3Variable.Value.y;
            storeZ.Value = vector3Variable.Value.z;
            return TaskStatus.Success;
        }

        public override void OnReset()
        {
            vector3Variable = Vector3.zero;
            storeX = 0;
            storeY = 0;
            storeZ = 0;
        }
    }
}