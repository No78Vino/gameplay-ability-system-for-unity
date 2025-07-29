using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;

namespace GAS.Editor
{
    [Serializable]
    public class GEEditPeriod
    {
        [HorizontalGroup("A/B")]
        [LabelText("是否第一帧立即触发")]
        public bool firstTrigger;
        
        [VerticalGroup("A")]
        [HorizontalGroup("A/B")]
        [LabelText("间隔时间"),LabelWidth(50)]
        public int time;
        
        [VerticalGroup("A")]
        [LabelText("执行效果GE"),LabelWidth(50)]
        [ValueDropdown("@GasXlsxChoice.Effects()",IsUniqueList = true)]
        public List<int> effects;
    }
}