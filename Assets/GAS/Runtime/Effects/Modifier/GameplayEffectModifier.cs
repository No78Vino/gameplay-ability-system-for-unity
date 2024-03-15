using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using GAS.Runtime.Attribute;
using GAS.Editor;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GAS.Runtime.Effects.Modifier
{
    public enum GEOperation
    {
        Add,
        Multiply,
        Override
    }

    [Serializable]
    public struct GameplayEffectModifier
    {
        private static IEnumerable AttributeChoices = new ValueDropdownList<string>();
        
        [LabelText("Attribute")]
        [LabelWidth(100)]
        [OnValueChanged("OnAttributeChanged")]
        [ValueDropdown("AttributeChoices")]
        public string AttributeName;
        
        [HideInInspector]
        public string AttributeSetName;
        
        [HideInInspector]
        public string AttributeShortName;
        
        [LabelText("Magnitude")]
        [LabelWidth(100)]
        public float ModiferMagnitude;
        
        [LabelWidth(100)]
        public  GEOperation Operation;
        
        [LabelWidth(100)]
        [AssetSelector]
        public  ModifierMagnitudeCalculation MMC;
        
        // TODO
        // public readonly GameplayTagSet SourceTag;
        
        // TODO
        // public readonly GameplayTagSet TargetTag;
        
        public GameplayEffectModifier(
            string attributeName, 
            float modiferMagnitude,
            GEOperation operation,
            ModifierMagnitudeCalculation mmc)
        {
            AttributeName = attributeName;
            var splits = attributeName.Split('.');
            AttributeSetName = splits[0];
            AttributeShortName = splits[1];
            ModiferMagnitude = modiferMagnitude;
            Operation = operation;
            MMC = mmc;
        }

        public void SetModiferMagnitude(float value)
        {
            if (MMC is SetByCallerModCalculation)
            {
                ModiferMagnitude = value;
            }
            else
            {
                #if UNITY_EDITOR
                UnityEngine.Debug.LogError("[EX] this MMC is not SetByCallerModCalculation, can't set ModiferMagnitude!");
                #else
                UnityEngine.Debug.LogWarning("[EX] this MMC is not SetByCallerModCalculation, can't set ModiferMagnitude!");
                #endif
            }
        }

        void OnAttributeChanged()
        {
            var split = AttributeName.Split('.');
            AttributeSetName =  split[0];
            AttributeShortName = split[1];
        }
        
        public static void SetAttributeChoices()
        {
            Type attributeSetUtil = Type.GetType($"GAS.Runtime.AttributeSet.GAttrSetLib, Assembly-CSharp");
            if(attributeSetUtil == null)
            {
                Debug.LogError("[EX] Type 'GAttrSetLib' not found. Please generate the AttributeSet CODE first!");
                AttributeChoices = new ValueDropdownList<string>();
                return;
            }
            FieldInfo attrFullNamesField = attributeSetUtil.GetField("AttributeFullNames", BindingFlags.Public | BindingFlags.Static);
            
            if (attrFullNamesField != null)
            {
                List<string> attrFullNamesValue = (List<string>)attrFullNamesField.GetValue(null);
                var choices = new ValueDropdownList<string>();
                foreach (var tag in attrFullNamesValue) choices.Add(tag,tag);
                AttributeChoices = choices;
            }
            else
            {
                AttributeChoices = new ValueDropdownList<string>();
            }
        }
    }
}