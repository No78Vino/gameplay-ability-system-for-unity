using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GAS.Runtime
{
    [Serializable]
    public class AttrInASCustomConfig
    {
        [HideInInspector]
        public int AttrCode;
        
        [VerticalGroup("启用默认初始值",PaddingTop = 3f)]
        [HideLabel]
        public bool UseValueInit;
        
        [ShowIf(nameof(UseValueInit))]
        [LabelText("默认初始值")]
        public float ValueDefaultInit;
        
        [LabelText("钳制最小值")]
        public bool ClampMin;
        
        [LabelText("最小值")]
        [ShowIf(nameof(ClampMin))]
        public float ValueMin;
        
        [LabelText("钳制最大值")]
        public bool ClampMax;
        
        [LabelText("最大值")]
        [ShowIf(nameof(ClampMax))]
        public float ValueMax;

    }
}