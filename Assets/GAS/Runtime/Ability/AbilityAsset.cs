using System;
using System.Collections;
using System.Linq;
using GAS.General;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace GAS.Runtime
{
    public abstract class AbilityAsset : ScriptableObject
    {
        protected const int WIDTH_LABEL = 70;

        private static IEnumerable AbilityClassChoice = new ValueDropdownList<string>();

        public abstract Type AbilityType();

        [TitleGroup("Base")]
        [HorizontalGroup("Base/H1")]
        [TabGroup("Base/H1/V1", "Summary", SdfIconType.InfoSquareFill, TextColor = "#0BFFC5", Order = 1)]
        [HideLabel]
        [MultiLineProperty(10)]
        public string Description;

        [TabGroup("Base/H1/V2", "General", SdfIconType.AwardFill, TextColor = "#FF7F00", Order = 2)]
        [LabelText("所属能力", SdfIconType.FileCodeFill)]
        [LabelWidth(WIDTH_LABEL)]
        [ShowInInspector]
        [InfoBox("Ability Class is NULL!!! Please check.", InfoMessageType.Error, VisibleIf = "@AbilityType() == null")]
        [PropertyOrder(-1)]
        public string InstanceAbilityClassFullName => AbilityType() != null ? AbilityType().FullName : null;

#if UNITY_EDITOR
        [TabGroup("Base/H1/V2", "General")]
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

        [TabGroup("Base/H1/V2", "General", SdfIconType.AwardFill)]
        [InfoBox(GASTextDefine.TIP_UNAME, InfoMessageType.None)]
        [LabelText("U-Name", SdfIconType.Fingerprint)]
        [LabelWidth(WIDTH_LABEL)]
        [InfoBox("无效的名字 - 不符合C#标识符命名规则", InfoMessageType.Error,
            "@GAS.General.Validation.Validations.IsValidVariableName($value) == false")]
        [InlineButton("@UniqueName = name", "Auto", Icon = SdfIconType.Hammer)]
        public string UniqueName;

        [TabGroup("Base/H1/V2", "General")]
        [Title("消耗&冷却", bold: true)]
        [LabelWidth(WIDTH_LABEL)]
        [AssetSelector]
        [LabelText(SdfIconType.HeartHalf, Text = GASTextDefine.ABILITY_EFFECT_COST)]
        public GameplayEffectAsset Cost;

        [TabGroup("Base/H1/V2", "General")]
        [LabelWidth(WIDTH_LABEL)]
        [AssetSelector]
        [LabelText(SdfIconType.StopwatchFill, Text = GASTextDefine.ABILITY_EFFECT_CD)]
        public GameplayEffectAsset Cooldown;

        [TabGroup("Base/H1/V2", "General")]
        [LabelWidth(WIDTH_LABEL)]
        [LabelText(SdfIconType.ClockFill, Text = GASTextDefine.ABILITY_CD_TIME)]
        [Unit(Units.Second)]
        public float CooldownTime;

        // Tags
        [TabGroup("Base/H1/V3", "Tags", SdfIconType.TagsFill, TextColor = "#45B1FF", Order = 3)]
        [ListDrawerSettings(ShowFoldout = true, ShowItemCount = false)]
        [ValueDropdown("@ValueDropdownHelper.GameplayTagChoices", IsUniqueList = true, HideChildProperties = true)]
        [Tooltip("描述性质的标签，用来描述Ability的特性表现，比如伤害、治疗、控制等。")]
        [FormerlySerializedAs("AssetTag")]
        public GameplayTag[] AssetTags;

        [TabGroup("Base/H1/V3", "Tags")]
        [ListDrawerSettings(ShowFoldout = true, ShowItemCount = false)]
        [ValueDropdown("@ValueDropdownHelper.GameplayTagChoices", IsUniqueList = true, HideChildProperties = true)]
        [LabelText("CancelAbility With Tags ")]
        [Space]
        [Tooltip("Ability激活时，Ability持有者当前持有的所有Ability中，拥有【任意】这些标签的Ability会被取消。")]
        public GameplayTag[] CancelAbilityTags;

        [TabGroup("Base/H1/V3", "Tags")]
        [ListDrawerSettings(ShowFoldout = true, ShowItemCount = false)]
        [ValueDropdown("@ValueDropdownHelper.GameplayTagChoices", IsUniqueList = true, HideChildProperties = true)]
        [LabelText("BlockAbility With Tags ")]
        [Space]
        [Tooltip("Ability激活时，Ability持有者当前持有的所有Ability中，拥有【任意】这些标签的Ability会被阻塞激活。")]
        public GameplayTag[] BlockAbilityTags;

        [TabGroup("Base/H1/V3", "Tags")]
        [ListDrawerSettings(ShowFoldout = true, ShowItemCount = false)]
        [ValueDropdown("@ValueDropdownHelper.GameplayTagChoices", IsUniqueList = true, HideChildProperties = true)]
        [Space]
        [Tooltip("Ability激活时，持有者会获得这些标签，Ability被失活时，这些标签也会被移除。")]
        [FormerlySerializedAs("ActivationOwnedTag")]
        public GameplayTag[] ActivationOwnedTags;

        [TabGroup("Base/H1/V3", "Tags")]
        [ListDrawerSettings(ShowFoldout = true, ShowItemCount = false)]
        [ValueDropdown("@ValueDropdownHelper.GameplayTagChoices", IsUniqueList = true, HideChildProperties = true)]
        [Space]
        [Tooltip("Ability只有在其拥有者拥有【所有】这些标签时才可激活。")]
        public GameplayTag[] ActivationRequiredTags;

        [TabGroup("Base/H1/V3", "Tags")]
        [ListDrawerSettings(ShowFoldout = true, ShowItemCount = false)]
        [ValueDropdown("@ValueDropdownHelper.GameplayTagChoices", IsUniqueList = true, HideChildProperties = true)]
        [Space]
        [Tooltip("Ability在其拥有者拥有【任意】这些标签时不能被激活。")]
        public GameplayTag[] ActivationBlockedTags;
        // public GameplayTag[] SourceRequiredTags;
        // public GameplayTag[] SourceBlockedTags;
        // public GameplayTag[] TargetRequiredTags;
        // public GameplayTag[] TargetBlockedTags;
    }


    public abstract class AbilityAssetT<T> : AbilityAsset where T : class
    {
        public sealed override Type AbilityType()
        {
            return typeof(T);
        }
    }
}