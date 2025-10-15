using System;
using System.Collections.Generic;
using GAS.Runtime;
using Sirenix.OdinInspector;
using UnityEngine.Serialization;

namespace GAS.Editor
{
    [Serializable]
    public class GEEditStacking
    {
        [VerticalGroup("A")]
        [LabelText("堆叠码")]
        [LabelWidth(100)]
        public int code;
        
        [VerticalGroup("A")]
        [LabelText("堆叠类型")]
        [LabelWidth(100)]
        [EnumToggleButtons]
        public StackingType stackingType;
        
        [VerticalGroup("A")]
        [LabelText("限制层数")]
        [LabelWidth(100)]
        public int limitCount;

        [VerticalGroup("A")] 
        [LabelText("持续时间刷新策略")]
        [EnumToggleButtons]
        [LabelWidth(100)]
        public DurationRefreshPolicy durationRefreshPolicy;

        [VerticalGroup("A")]
        [LabelText("周期重置策略")]
        [EnumToggleButtons]
        [LabelWidth(100)]
        public PeriodResetPolicy periodResetPolicy;
        
        [FormerlySerializedAs("expirationPolicy")]
        [VerticalGroup("A")]
        [LabelText("过期策略")]
        [EnumToggleButtons]
        [LabelWidth(100)]
        public StackingExpirationPolicy stackingExpirationPolicy;

        [VerticalGroup("A")] 
        [LabelText("拒绝溢出时间重置")]
        [LabelWidth(120)]
        public bool DenyOverflowApplication;
        
        [VerticalGroup("A")]
        [LabelText("溢出清空层数")]
        [ShowIf(nameof(DenyOverflowApplication))]
        [LabelWidth(100)]
        public bool clearStackOnOverflow;

        [VerticalGroup("A")]
        [LabelText("溢出触发效果")]
        [LabelWidth(100)]
        [ValueDropdown("@GasXlsxChoice.Effects()", IsUniqueList = true)]
        public List<int> overflowEffects;
    }
}