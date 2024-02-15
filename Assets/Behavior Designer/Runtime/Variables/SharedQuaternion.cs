using UnityEngine;

namespace BehaviorDesigner.Runtime
{
    [System.Serializable]
    public class SharedQuaternion : SharedVariable<Quaternion>
    {
        public static implicit operator SharedQuaternion(Quaternion value) { return new SharedQuaternion { mValue = value }; }
    }
}