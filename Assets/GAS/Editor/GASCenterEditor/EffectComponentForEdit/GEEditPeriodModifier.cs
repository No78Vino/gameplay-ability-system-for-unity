using System;
using System.Collections.Generic;
using GAS.Runtime;
using Sirenix.OdinInspector;

namespace GAS.Editor
{
    [Serializable]
    public class GEEditPeriodModifier
    {
        [LabelText("目标属性集")]
        [LabelWidth(70)]
        [ValueDropdown("@GasXlsxChoice.AttrSets()",IsUniqueList = true)]
        public int AttrSet;
        
        [LabelText("目标属性")]
        [LabelWidth(70)]
        [ValueDropdown("@GasXlsxChoice.Attributes(AttrSet)",IsUniqueList = true)]
        public int Attribute;
        
        [LabelText("基础模值")]
        [LabelWidth(70)]
        public float Magnitude;
        
        [LabelText("操作类型")]
        [LabelWidth(70)]
        [EnumToggleButtons]
        public GEOperation Operation;

        [LabelText("修改器类型")]
        [LabelWidth(70)]
        [ValueDropdown("@GasXlsxChoice.MMCs()",IsUniqueList = true)]
        public int MMC;

        private List<ValueDropdownItem> AttrChoices()
        {
            return GasXlsxChoice.Attributes(AttrSet);
        }
    }
}