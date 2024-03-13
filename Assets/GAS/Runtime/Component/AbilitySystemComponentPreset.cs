using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using GAS.Runtime.Ability;
using GAS.Runtime.Tags;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace GAS.Runtime.Component
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
        private const string GRP_DATA_PARAMETER = "DATA/H/Parameter";
        private const string GRP_DATA_TAG = "DATA/H/Tags";
        
        
        private const int WIDTH_LABLE = 100;
        private const int WIDTH_GRP_BASE_H_LEFT = 350;

        private const string ERROR_ABILITY = "Ability can't be NONE!!";


        private static IEnumerable AttributeSetChoice = new ValueDropdownList<string>();
        private static IEnumerable TagChoices = new ValueDropdownList<GameplayTag>();
        
        
        [BoxGroup(GRP_BASE,false)]
        [Title("Base Information", bold: true)]
        [HorizontalGroup(GRP_BASE_H,Width = WIDTH_GRP_BASE_H_LEFT)]
        [VerticalGroup(GRP_BASE_H_LEFT)]
        public string Name;
        
        [VerticalGroup(GRP_BASE_H_LEFT)]
        [Title("Description", bold: false)]
        [HideLabel]
        [MultiLineProperty(5)]
        public string Description;


        [Title("AttributeSet", bold: true)]
        [HorizontalGroup(GRP_BASE_H,PaddingLeft = 0.025f)]
        [VerticalGroup(GRP_BASE_H_RIGHT)]
        [LabelWidth(WIDTH_LABLE)]
        [ListDrawerSettings(Expanded = true)]
        [ValueDropdown("AttributeSetChoice")]
        public string[] AttributeSets;
        
        [Title("Base Tag",bold:true)]
        [BoxGroup(GRP_DATA,false)]
        [HorizontalGroup(GRP_DATA_H)]
        [VerticalGroup(GRP_DATA_PARAMETER)]
        [ListDrawerSettings(Expanded = true,ShowIndexLabels = false,ShowItemCount = false)]
        [ValueDropdown("TagChoices",HideChildProperties = true)]
        public GameplayTag[] BaseTags;
        
        [Title("Base Ability",bold:true)]
        [HorizontalGroup(GRP_DATA_H)]
        [VerticalGroup(GRP_DATA_TAG)]
        [ListDrawerSettings(Expanded = true)]
        [AssetSelector]
        [InfoBox(ERROR_ABILITY,InfoMessageType.Error,VisibleIf = "IsAbilityNone")]
        public AbilityAsset[] BaseAbilities;


        private void OnEnable()
        {
            SetAttributeSetChoices();
            SetTagChoices();
        }

        bool IsAbilityNone()
        {
            return BaseAbilities.Any(ability => ability == null);
        }
        
        static void SetAttributeSetChoices()
        {
            Type attributeSetUtil = Type.GetType($"GAS.Runtime.AttributeSet.GAttrSetLib, Assembly-CSharp");
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
            Type gameplayTagSumCollectionType = Type.GetType($"GAS.Runtime.Tags.GTagLib, Assembly-CSharp");
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