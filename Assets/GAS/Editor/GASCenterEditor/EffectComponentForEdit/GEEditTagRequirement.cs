using System;
using System.Collections.Generic;
using GAS.Runtime;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GAS.Editor
{
    [System.Serializable]
    public class GEEditTagRequirement
    {
        [HorizontalGroup("Tags", Width = 0.33f)]
        [VerticalGroup("Tags/All")]
        [Title("All", Bold = false)]
        [ValueDropdown("@GasXlsxChoice.Tags()", IsUniqueList = true)]
        [LabelText(" ")]
        public List<int> All = new();

        [VerticalGroup("Tags/Any")]
        [Title("Any", Bold = false)]
        [ValueDropdown("@GasXlsxChoice.Tags()", IsUniqueList = true)]
        [LabelText(" ")]
        public List<int> Any = new();

        [VerticalGroup("Tags/None")]
        [Title("None", Bold = false)]
        [ValueDropdown("@GasXlsxChoice.Tags()", IsUniqueList = true)]
        [LabelText(" ")]
        public List<int> None = new();

        public bool HasAnyValue()
        {
            return (All != null && All.Count > 0)
                || (Any != null && Any.Count > 0)
                || (None != null && None.Count > 0);
        }
    }
}