using UnityEngine;

namespace BehaviorDesigner.Runtime
{
    [System.Serializable]
    public class SharedColor : SharedVariable<Color>
    {
        public static implicit operator SharedColor(Color value) { return new SharedColor { mValue = value }; }
    }
}