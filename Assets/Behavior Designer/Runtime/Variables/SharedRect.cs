using UnityEngine;

namespace BehaviorDesigner.Runtime
{
    [System.Serializable]
    public class SharedRect : SharedVariable<Rect>
    {
        public static implicit operator SharedRect(Rect value) { return new SharedRect { mValue = value }; }
    }
}