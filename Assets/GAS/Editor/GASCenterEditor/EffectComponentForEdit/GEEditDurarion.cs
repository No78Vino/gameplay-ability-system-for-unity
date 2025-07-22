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
        public float time;
        
        [HorizontalGroup("A")]
        [LabelText("(单位)"),LabelWidth(50)]
        public TimeUnit Unit;
    }
}