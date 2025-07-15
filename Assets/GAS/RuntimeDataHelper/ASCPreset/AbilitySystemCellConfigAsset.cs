using System;
using GAS.General;
using GAS.RuntimeDataHelper.Ability;
using GAS.RuntimeWithECS.Ability;
using GAS.RuntimeWithECS.AbilitySystemCell;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GAS.RuntimeDataHelper.ASCPreset
{
    [CreateAssetMenu(fileName = "AbilitySystemCellConfigAsset", menuName = "EX-GAS/ASC", order = 0)]
    public class AbilitySystemCellConfigAsset : ScriptableObject
    {
        private const int WIDTH_LABEL = 70;


        [TabGroup("Base", GASConstDefine.ASC_BASE_TAG, SdfIconType.TagsFill, TextColor = "#45B1FF", Order = 3)]
        [LabelText(GASConstDefine.ASC_BASE_TAG)]
        [ValueDropdown("@EXEditorHelper.GameplayTagCodeChoices", 
            IsUniqueList = true, 
            HideChildProperties = true)]
        public int[] baseTags;

        [ShowInInspector]
        [TabGroup("Base", GASConstDefine.ASC_AttributeSet, SdfIconType.PersonLinesFill, TextColor = "#FF7F00",
            Order = 2)]
        [LabelText(GASConstDefine.ASC_AttributeSet)]
        [ValueDropdown("@EditAttributeHelper.AttributeSetChoices", IsUniqueList = true)]
        public int[] _attributeSets;

        [ShowInInspector]
        [TabGroup("Base", GASConstDefine.ASC_BASE_ABILITY, SdfIconType.YinYang, TextColor = "#FF7F00",
            Order = 2)]
        [LabelText(GASConstDefine.ASC_BASE_ABILITY)]
        [LabelWidth(WIDTH_LABEL)]
        public AbilityConfigAsset[] baseAbilities;

        public AbilitySystemCellConfig GetConfig()
        {
            var t = baseTags ?? Array.Empty<int>();
            var attrSet = _attributeSets ?? Array.Empty<int>();
            var a = new AbilityConfig[baseAbilities.Length];
            for (var i = 0; i < baseAbilities.Length; i++) a[i] = baseAbilities[i].GetConfig();
            var level = 1;
            return new AbilitySystemCellConfig(t, attrSet, a, level);
        }
    }
}