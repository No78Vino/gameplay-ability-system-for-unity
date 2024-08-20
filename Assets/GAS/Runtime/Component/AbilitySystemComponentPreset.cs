using System.Linq;
using GAS.General;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GAS.Runtime
{
    [CreateAssetMenu(fileName = "AbilitySystemComponentPreset", menuName = "GAS/AbilitySystemComponentPreset")]
    public class AbilitySystemComponentPreset : ScriptableObject
    {
        private const int WIDTH_LABEL = 70;

        [TitleGroup("Base")]
        [HorizontalGroup("Base/H1", Width = 1 - 0.618f)]
        [TabGroup("Base/H1/V1", "Summary", SdfIconType.InfoSquareFill, TextColor = "#0BFFC5", Order = 1)]
        [HideLabel]
        [MultiLineProperty(10)]
        public string Description;

        [TabGroup("Base/H1/V2", "Attribute Sets", SdfIconType.PersonLinesFill, TextColor = "#FF7F00", Order = 2)]
        [LabelText(GASTextDefine.ASC_AttributeSet)]
        [LabelWidth(WIDTH_LABEL)]
        [ValueDropdown("@ValueDropdownHelper.AttributeSetChoices", IsUniqueList = true)]
        [ListDrawerSettings(ShowFoldout = true, ShowItemCount = false, DraggableItems = false)]
        [DisableContextMenu(disableForMember: false, disableCollectionElements: true)]
        [CustomContextMenu("排序", "@SortAttributeSets()")]
        public string[] AttributeSets;

        private void SortAttributeSets() => AttributeSets = AttributeSets.OrderBy(x => x).ToArray();

        [HorizontalGroup("Base/H2", Width = 1 - 0.618f)]
        [TabGroup("Base/H2/V1", "Tags", SdfIconType.TagsFill, TextColor = "#45B1FF", Order = 1)]
        [LabelText(GASTextDefine.ASC_BASE_TAG)]
        [ValueDropdown("@ValueDropdownHelper.GameplayTagChoices", IsUniqueList = true, HideChildProperties = true)]
        [ListDrawerSettings(ShowFoldout = true, ShowItemCount = false, DraggableItems = false)]
        [DisableContextMenu(disableForMember: false, disableCollectionElements: true)]
        [CustomContextMenu("排序", "@BaseTags = TagHelper.Sort($value)")]
        public GameplayTag[] BaseTags;

        [TabGroup("Base/H2/V2", "Abilities", SdfIconType.YinYang, TextColor = "#D6626E", Order = 2)]
        [LabelText(GASTextDefine.ASC_BASE_ABILITY)]
        [ListDrawerSettings(ShowFoldout = true, ShowItemCount = false, DraggableItems = false)]
        [DisableContextMenu(disableForMember: false, disableCollectionElements: true)]
        [CustomContextMenu("排序", "@SortBaseAbilities()")]
        [AssetSelector]
        [ValidateInput("@ValidateInput_BaseAbilities()", "存在无效的能力")]
        public AbilityAsset[] BaseAbilities;

        private void SortBaseAbilities() => BaseAbilities = BaseAbilities.OrderBy(x => x.name).ToArray();

        bool ValidateInput_BaseAbilities() => BaseAbilities != null && BaseAbilities.All(ability => ability != null);
    }
}