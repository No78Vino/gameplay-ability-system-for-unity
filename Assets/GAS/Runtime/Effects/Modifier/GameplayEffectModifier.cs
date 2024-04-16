using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using GAS.General;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GAS.Runtime
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
        [Tooltip("指的是GameplayEffect作用对象被修改的属性。")]
        public string AttributeName;
        
        [HideInInspector]
        public string AttributeSetName;
        
        [HideInInspector]
        public string AttributeShortName;
        
        [LabelText("Magnitude")]
        [LabelWidth(100)]
        [Tooltip("修改器的基础数值。这个数值如何使用由MMC的运行逻辑决定。")]
        public float ModiferMagnitude;
        
        [LabelWidth(100)]
        [Tooltip("操作类型：是对属性的操作类型，\n有3种：\nAdd ： 加法（取值为负便是减法）\nMultiply： 乘法（除法取倒数即可）\nOverride：覆写属性值")]
        public  GEOperation Operation;
        
        [LabelWidth(100)]
        [AssetSelector]
        [Tooltip("ModifierMagnitudeCalculation，修改器，负责GAS中Attribute的数值计算逻辑。")]
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

        public float CalculateMagnitude(GameplayEffectSpec spec, float modifierMagnitude)
        {
            return MMC == null ? ModiferMagnitude : MMC.CalculateMagnitude(spec, modifierMagnitude);
        }

        public void SetModiferMagnitude(float value)
        {
            ModiferMagnitude = value;
        }

        void OnAttributeChanged()
        {
            var split = AttributeName.Split('.');
            AttributeSetName =  split[0];
            AttributeShortName = split[1];
        }
        
        public static void SetAttributeChoices()
        {
            Type attributeSetUtil = TypeUtil.FindTypeInAllAssemblies("GAS.Runtime.GAttrSetLib");
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