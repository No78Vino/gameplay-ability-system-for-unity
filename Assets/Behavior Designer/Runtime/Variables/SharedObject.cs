using UnityEngine;

namespace BehaviorDesigner.Runtime
{
    [System.Serializable]
    public class SharedObject : SharedVariable<Object>
    {
        public static explicit operator SharedObject(Object value) { return new SharedObject { mValue = value }; }
    }
}