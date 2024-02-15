using UnityEngine;
using System.Collections.Generic;

namespace BehaviorDesigner.Runtime
{
    [System.Serializable]
    public class SharedGameObjectList : SharedVariable<List<GameObject>>
    {
        public static implicit operator SharedGameObjectList(List<GameObject> value) { return new SharedGameObjectList { mValue = value }; }
    }
}