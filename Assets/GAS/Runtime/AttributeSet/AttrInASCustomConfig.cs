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

        [VerticalGroup("属性",PaddingTop = 3f,Order = -1)]
        [ShowInInspector]
        [DisplayAsString(EnableRichText = true)]
        [HideLabel]
        public string attrName;
        
        [VerticalGroup("默认初始值")]
        [HideLabel]
        public float ValueDefaultInit;

        [VerticalGroup("钳制最小值")]
        [HideLabel]
        public bool ClampMin;

        [VerticalGroup("最小值")]
        [HideLabel]
        [ShowIf(nameof(ClampMin))]
        public float ValueMin;
        
        [VerticalGroup("钳制最大值")]
        [HideLabel]
        public bool ClampMax;
        
        [VerticalGroup("最大值")]
        [HideLabel]
        [ShowIf(nameof(ClampMax))]
        public float ValueMax;

    }
}