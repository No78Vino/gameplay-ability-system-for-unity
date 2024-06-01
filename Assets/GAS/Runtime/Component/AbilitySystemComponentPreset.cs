using System.Linq;
using GAS.General;
using Sirenix.OdinInspector;
using UnityEngine;
#if UNITY_EDITOR
using Sirenix.Utilities.Editor;
#endif

namespace GAS.Runtime
{
    [CreateAssetMenu(fileName = "AbilitySystemComponentPreset", menuName = "GAS/AbilitySystemComponentPreset")]
    public class AbilitySystemComponentPreset : ScriptableObject
    {
        private const int WIDTH_LABEL = 70;
        private const string ERROR_ABILITY = "Ability can't be NONE!!";

        [TitleGroup("Base")]
        [HorizontalGroup("Base/H1", Width = 1 / 3f)]
        [TabGroup("Base/H1/V1", "Summary", SdfIconType.InfoSquareFill, TextColor = "#0BFFC5", Order = 1)]
        [HideLabel]
        [MultiLineProperty(10)]
        public string Description;


        [TabGroup("Base/H1/V2", "Attribute Sets", SdfIconType.PersonLinesFill, TextColor = "#FF7F00", Order = 2)]
        [LabelText(GASTextDefine.ASC_AttributeSet)]
        [LabelWidth(WIDTH_LABEL)]
        [ListDrawerSettings(ShowFoldout = true, ShowItemCount = false, OnTitleBarGUI = "DrawAttributeSetsButtons")]
        [ValueDropdown("@ValueDropdownHelper.AttributeSetChoices", IsUniqueList = true)]
        public string[] AttributeSets;

        private void DrawAttributeSetsButtons()
        {
#if UNITY_EDITOR
            if (SirenixEditorGUI.ToolbarButton(SdfIconType.SortAlphaDown))
            {
                AttributeSets = AttributeSets.OrderBy(x => x).ToArray();
            }
#endif
        }

        [TabGroup("Base/H1/V3", "Tags", SdfIconType.TagsFill, TextColor = "#45B1FF", Order = 3)]
        [LabelText(GASTextDefine.ASC_BASE_TAG)]
        [ListDrawerSettings(ShowFoldout = true, ShowItemCount = false, OnTitleBarGUI = "DrawBaseTagsButtons")]
        [ValueDropdown("@ValueDropdownHelper.GameplayTagChoices", IsUniqueList = true, HideChildProperties = true)]
        public GameplayTag[] BaseTags;

        private void DrawBaseTagsButtons()
        {
#if UNITY_EDITOR
            if (SirenixEditorGUI.ToolbarButton(SdfIconType.SortAlphaDown))
            {
                BaseTags = BaseTags.OrderBy(x => x.Name).ToArray();
            }
#endif
        }

        [HorizontalGroup("Base/H2")]
        [TabGroup("Base/H2/V1", "Abilities", SdfIconType.YinYang, TextColor = "#D6626E", Order = 1)]
        [LabelText(GASTextDefine.ASC_BASE_ABILITY)]
        [ListDrawerSettings(ShowFoldout = true, ShowItemCount = false, OnTitleBarGUI = "DrawBaseAbilitiesButtons")]
        [AssetSelector]
        [InfoBox(ERROR_ABILITY, InfoMessageType.Error, VisibleIf = "@IsAbilityNone()")]
        public AbilityAsset[] BaseAbilities;

        private void DrawBaseAbilitiesButtons()
        {
#if UNITY_EDITOR
            if (SirenixEditorGUI.ToolbarButton(SdfIconType.SortAlphaDown))
            {
                BaseAbilities = BaseAbilities.OrderBy(x => x.name).ToArray();
            }
#endif
        }

        bool IsAbilityNone()
        {
            return BaseAbilities != null && BaseAbilities.Any(ability => ability == null);
        }
    }
}