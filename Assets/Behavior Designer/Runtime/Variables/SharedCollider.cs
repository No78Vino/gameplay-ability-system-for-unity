using UnityEngine;

namespace BehaviorDesigner.Runtime
{
    [System.Serializable]
    public class SharedCollider : SharedVariable<Collider>
    {
        public static implicit operator SharedCollider(Collider value) { return new SharedCollider { mValue = value }; }
    }
}