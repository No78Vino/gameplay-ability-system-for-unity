using System.Linq;
using GAS.General;
using GAS.Runtime;
using Sirenix.OdinInspector;
using Unity.Entities;

namespace GAS.RuntimeWithECS.Cue
{
    public abstract class NewGameplayCueBase
    {
        protected const int WIDTH_LABEL = 70;

        [TitleGroup("Base")]
        [HorizontalGroup("Base/H1")]
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
        // Tags
        [TabGroup("Base/H1/V3", "Tags", SdfIconType.TagsFill, TextColor = "#45B1FF", Order = 3)]
        [ListDrawerSettings(ShowFoldout = true, ShowItemCount = false)]
        [ValueDropdown("@ValueDropdownHelper.GameplayTagChoices", IsUniqueList = true, HideChildProperties = true)]
        [LabelText("RequiredTags - 持有所有标签才可触发")]
        public GameplayTag[] RequiredTags;

        [TabGroup("Base/H1/V3", "Tags")]
        [ListDrawerSettings(ShowFoldout = true, ShowItemCount = false)]
        [ValueDropdown("@ValueDropdownHelper.GameplayTagChoices", IsUniqueList = true, HideChildProperties = true)]
        [LabelText("ImmunityTags - 持有任意标签不可触发")]
        public GameplayTag[] ImmunityTags;

        public virtual bool Triggerable()
        {
            // if (owner == null) return false;
            // // 持有【所有】RequiredTags才可触发
            // if (!owner.HasAllTags(new GameplayTagSet(RequiredTags)))
            //     return false;
            //
            // // 持有【任意】ImmunityTags不可触发
            // if (owner.HasAnyTags(new GameplayTagSet(ImmunityTags)))
            //     return false;

            return true;
        }

        protected Entity _sourceEntity;
        protected CueSourceType _sourceType;
        protected Entity _targetAscEntity;
        
        public void SetSourceEntity(Entity e)
        {
            _sourceEntity = e;
        }
        
        public void SetTargetAscEntity(Entity e)
        {
            _targetAscEntity = e;
        }
        
        public void SetSourceType(CueSourceType sourceType)
        {
            _sourceType = sourceType;
        }
        
        public abstract void InitParameters(ICueParameter parameter);
    }
}