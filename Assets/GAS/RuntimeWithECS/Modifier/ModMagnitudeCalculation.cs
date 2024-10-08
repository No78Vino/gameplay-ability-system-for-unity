using System.Linq;
using GAS.General;
using Sirenix.OdinInspector;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace GAS.RuntimeWithECS.Modifier
{
    public abstract class ModMagnitudeCalculation
    {
        protected const int WIDTH_LABEL = 70;

        [TitleGroup("Base")]
        [HorizontalGroup("Base/H1", width: 1 - 0.618f)]
        [TabGroup("Base/H1/V1", "Summary", SdfIconType.InfoSquareFill, TextColor = "#0BFFC5", Order = 1)]
        [HideLabel]
        [MultiLineProperty(10)]
        public string Description;

#if UNITY_EDITOR
        [TabGroup("Base/H1/V2", "General", SdfIconType.AwardFill, TextColor = "#FF7F00", Order = 2)]
        [TabGroup("Base/H1/V2", "Detail", SdfIconType.TicketDetailedFill, TextColor = "#BC2FDE")]
        [LabelText("类型名称", SdfIconType.FileCodeFill)]
        [LabelWidth(WIDTH_LABEL)]
        [ShowInInspector]
        [PropertyOrder(-1)]
        public string TypeName => GetType().Name;

        [TabGroup("Base/H1/V2", "Detail")]
        [LabelText("类型全名", SdfIconType.FileCodeFill)]
        [LabelWidth(WIDTH_LABEL)]
        [ShowInInspector]
        [PropertyOrder(-1)]
        public string TypeFullName => GetType().FullName;

        [TabGroup("Base/H1/V2", "Detail")]
        [ListDrawerSettings(ShowFoldout = true, ShowItemCount = false, ShowPaging = false)]
        [ShowInInspector]
        [LabelText("继承关系")]
        [LabelWidth(WIDTH_LABEL)]
        [PropertyOrder(-1)]
        public string[] InheritanceChain => GetType().GetInheritanceChain().Reverse().ToArray();
#endif

        public abstract float CalculateMagnitude(Entity specEntity, float magnitude);

        public abstract void InitParameters(NativeArray<float> floatParams,NativeArray<int> intParams,NativeArray<FixedString32Bytes> stringParams);
    }
}