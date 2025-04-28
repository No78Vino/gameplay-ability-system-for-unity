using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GAS.Runtime
{
    [Serializable]
    public struct AttrInASCustomConfig
    {
        [HideInInspector]
        public int AttrCode;
        
        [HorizontalGroup("A",order:1)]
        public bool UseValueInit;
        
        [HorizontalGroup("A",order:2)]
        [ShowIf(nameof(UseValueInit))]
        public float ValueDefaultInit;
        
        [HorizontalGroup("A",order:3)]
        public bool ClampMin;
        
        [HorizontalGroup("A",order:4)]
        [ShowIf(nameof(ClampMin))]
        public float ValueMin;
        
        [HorizontalGroup("A",order:5)]
        public bool ClampMax;
        
        [HorizontalGroup("A",order:6)]
        [ShowIf(nameof(ClampMax))]
        public float ValueMax;

    }
}