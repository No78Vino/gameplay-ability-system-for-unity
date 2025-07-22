using System;
using GAS.Runtime;
using Sirenix.OdinInspector;

namespace GAS.Editor
{
    [Serializable]
    public class GEEditPeriodModifier
    {
        [LabelText("目标属性集")]
        public int AttrSet;
        
        [LabelText("目标属性")]
        public int Attribute;
        
        [LabelText("操作类型")]
        public GEOperation Operation;

        [LabelText("修改器类型")]
        public int MMC;
    }
}