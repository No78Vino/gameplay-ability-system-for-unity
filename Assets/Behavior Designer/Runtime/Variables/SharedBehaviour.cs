using UnityEngine;

namespace BehaviorDesigner.Runtime
{
    [System.Serializable]
    public class SharedBehaviour : SharedVariable<Behaviour>
    {
        public static explicit operator SharedBehaviour(Behaviour value) { return new SharedBehaviour { mValue = value }; }
    }
}