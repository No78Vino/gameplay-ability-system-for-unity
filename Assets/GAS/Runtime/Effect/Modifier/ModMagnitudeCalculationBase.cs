using System.Linq;
using GAS.General;
using Sirenix.OdinInspector;
using Unity.Entities;
using UnityEngine;

namespace GAS.Runtime
{
    public abstract class ModMagnitudeCalculationBase
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
        
        public abstract void InitParameters(XParam parameter);
        
        public abstract float CalculateMagnitude(MmcContext mmcContext, float magnitude);

        public void OnAddMmc(Entity gameplayEffect, int targetAttrSetCode, int targetAttrCode)
        {
            var context = MmcHelper.BuildMmcContext(gameplayEffect);  
            OnAdded(context,targetAttrSetCode,targetAttrCode);
        }  
        
        public void OnRemoveMmc()
        {
            OnRemoved();
        }  
        
        
        protected virtual void OnAdded(MmcContext context, int targetAttrSetCode, int targetAttrCode) { }
        
        protected virtual void OnRemoved() { }
        
    }
    
    public abstract class ModMagnitudeCalculationBase<T> : ModMagnitudeCalculationBase
        where T : XParam
    {
        public T Parameter { get; private set; }
        
        public override void InitParameters(XParam parameter)
        {
            if (parameter is T t)
                Parameter = t;
#if UNITY_EDITOR
            else
                Debug.LogError($"Parameter type mismatch: expected {typeof(T)}, but got {parameter.GetType()}");
#endif
        }
    }
}