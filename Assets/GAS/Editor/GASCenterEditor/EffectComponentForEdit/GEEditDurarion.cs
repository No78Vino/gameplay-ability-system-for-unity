using System;
using GAS.Runtime;
using Sirenix.OdinInspector;

namespace GAS.Editor
{
    [Serializable]
    public class GEEditDurarion
    {
        [HorizontalGroup("A")]
        [LabelText("时间"),LabelWidth(50)]
        public int time;
        
        [HorizontalGroup("A")]
        [LabelText("(单位)"),LabelWidth(50)]
        public TimeUnit Unit;
        
        [LabelText("是否激活时重置计时")]
        public bool ResetStartTimeWhenActivated;
    }
}