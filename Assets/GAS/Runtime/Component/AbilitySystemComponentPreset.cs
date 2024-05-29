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
        private const string GRP_BASE = "Base Info";
        private const string GRP_BASE_H = "Base Info/H";
        private const string GRP_BASE_H_LEFT = "Base Info/H/Left";
        private const string GRP_BASE_H_RIGHT = "Base Info/H/Right";

        private const string GRP_DATA = "DATA";
        private const string GRP_DATA_H = "DATA/H";
        private const string GRP_DATA_TAG = "DATA/H/Tags";
        private const string GRP_DATA_ABILITY = "DATA/H/Abilities";


        private const int WIDTH_LABEL = 100;
        private const int WIDTH_GRP_BASE_H_LEFT = 350;

        private const string ERROR_ABILITY = "Ability can't be NONE!!";

        [BoxGroup(GRP_BASE, false)]
        [Title(GASTextDefine.ABILITY_BASEINFO, bold: true)]
        [InfoBox(GASTextDefine.TIP_ASC_BASEINFO)]
        [HorizontalGroup(GRP_BASE_H, Width = WIDTH_GRP_BASE_H_LEFT)]
        [VerticalGroup(GRP_BASE_H_LEFT)]
        public string Name;

        [VerticalGroup(GRP_BASE_H_LEFT)]
        [Title("Description", bold: false)]
        [HideLabel]
        [MultiLineProperty(5)]
        public string Description;


        [Title(GASTextDefine.ASC_AttributeSet, bold: true)]
        [HorizontalGroup(GRP_BASE_H, PaddingLeft = 0.025f)]
        [VerticalGroup(GRP_BASE_H_RIGHT)]
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

        [Title(GASTextDefine.ASC_BASE_TAG, bold: true)]
        [BoxGroup(GRP_DATA, false)]
        [HorizontalGroup(GRP_DATA_H)]
        [VerticalGroup(GRP_DATA_TAG)]
        [ListDrawerSettings(ShowFoldout = true, ShowItemCount = false,
            OnTitleBarGUI = "DrawBaseTagsButtons")]
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

        [Title(GASTextDefine.ASC_BASE_ABILITY, bold: true)]
        [HorizontalGroup(GRP_DATA_H)]
        [VerticalGroup(GRP_DATA_ABILITY)]
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