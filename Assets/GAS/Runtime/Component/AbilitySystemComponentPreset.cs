using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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


        private static IEnumerable AttributeSetChoice = new ValueDropdownList<string>();
        private static IEnumerable TagChoices = new ValueDropdownList<GameplayTag>();
        
        
        [BoxGroup(GRP_BASE,false)]
        [Title(GASTextDefine.ABILITY_BASEINFO, bold: true)]
        [InfoBox(GASTextDefine.TIP_ASC_BASEINFO)]
        [HorizontalGroup(GRP_BASE_H,Width = WIDTH_GRP_BASE_H_LEFT)]
        [VerticalGroup(GRP_BASE_H_LEFT)]
        public string Name;
        
        [VerticalGroup(GRP_BASE_H_LEFT)]
        [Title("Description", bold: false)]
        [HideLabel]
        [MultiLineProperty(5)]
        public string Description;


        [Title(GASTextDefine.ASC_AttributeSet, bold: true)]
        [HorizontalGroup(GRP_BASE_H,PaddingLeft = 0.025f)]
        [VerticalGroup(GRP_BASE_H_RIGHT)]
        [LabelWidth(WIDTH_LABEL)]
        [ListDrawerSettings(Expanded = true, OnTitleBarGUI = "DrawAttributeSetsButtons")]
        [ValueDropdown("AttributeSetChoice", IsUniqueList = true)]
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
        
        [Title(GASTextDefine.ASC_BASE_TAG,bold:true)]
        [BoxGroup(GRP_DATA,false)]
        [HorizontalGroup(GRP_DATA_H)]
        [VerticalGroup(GRP_DATA_TAG)]
        [ListDrawerSettings(Expanded = true, ShowIndexLabels = false, ListElementLabelName ="Name", OnTitleBarGUI = "DrawBaseTagsButtons")]
        [ValueDropdown("TagChoices", HideChildProperties = true, IsUniqueList = true)]
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
        
        [Title(GASTextDefine.ASC_BASE_ABILITY,bold:true)]
        [HorizontalGroup(GRP_DATA_H)]
        [VerticalGroup(GRP_DATA_ABILITY)]
        [ListDrawerSettings(Expanded = true, OnTitleBarGUI = "DrawBaseAbilitiesButtons")]
        [AssetSelector]
        [InfoBox(ERROR_ABILITY,InfoMessageType.Error,VisibleIf = "IsAbilityNone")]
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

        private void OnEnable()
        {
            SetAttributeSetChoices();
            SetTagChoices();
        }

        bool IsAbilityNone()
        {
            return BaseAbilities!=null && BaseAbilities.Any(ability => ability == null);
        }
        
        static void SetAttributeSetChoices()
        {
            Type attributeSetUtil = TypeUtil.FindTypeInAllAssemblies("GAS.Runtime.GAttrSetLib");
            if(attributeSetUtil == null)
            {
                Debug.LogError("[EX] Type 'GAttrSetLib' not found. Please generate the AttributeSet CODE first!");
                AttributeSetChoice = new ValueDropdownList<string>();
                return;
            }
            FieldInfo attrSetTypeDictField = attributeSetUtil.GetField("AttrSetTypeDict", BindingFlags.Public | BindingFlags.Static);
            
            if (attrSetTypeDictField != null)
            {
                Dictionary<string,Type> attrFullNamesValue = (Dictionary<string,Type>)attrSetTypeDictField.GetValue(null);
                var choices = new ValueDropdownList<string>();
                foreach (var tag in attrFullNamesValue.Keys) choices.Add(tag,tag);
                AttributeSetChoice = choices;
            }
            else
            {
                AttributeSetChoice = new ValueDropdownList<string>();
            }
        }
        
        private static void SetTagChoices()
        {
            Type gameplayTagSumCollectionType = TypeUtil.FindTypeInAllAssemblies("GAS.Runtime.GTagLib");
            if(gameplayTagSumCollectionType == null)
            {
                Debug.LogError("[EX] Type 'GTagLib' not found. Please generate the TAGS CODE first!");
                TagChoices = new ValueDropdownList<GameplayTag>();
                return;
            }
            FieldInfo tagMapField = gameplayTagSumCollectionType.GetField("TagMap", BindingFlags.Public | BindingFlags.Static);

            if (tagMapField != null)
            {
                Dictionary<string, GameplayTag> tagMapValue = (Dictionary<string, GameplayTag>)tagMapField.GetValue(null);
                var tagChoices = tagMapValue.Values.ToList();
                var choices = new ValueDropdownList<GameplayTag>();
                foreach (var tag in tagChoices) choices.Add(tag.Name,tag);
                TagChoices = choices;
            }
            else
            {
                TagChoices = new ValueDropdownList<GameplayTag>();
            }
        }
    }
}