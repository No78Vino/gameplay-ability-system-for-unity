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
        [LabelText(SdfIconType.Plus, Text = "加法")]
        Add,

        [LabelText(SdfIconType.X, Text = "乘法")]
        Multiply,

        [LabelText(SdfIconType.Pencil, Text = "覆写")]
        Override
    }

    [Serializable]
    public struct GameplayEffectModifier
    {
        [LabelText("Attribute")]
        [LabelWidth(100)]
        [OnValueChanged("OnAttributeChanged")]
        [ValueDropdown("@ValueDropdownHelper.AttributeChoices", IsUniqueList = true)]
        [Tooltip("指的是GameplayEffect作用对象被修改的属性。")]
        [InfoBox("未选择属性", InfoMessageType.Error, VisibleIf = "@string.IsNullOrWhiteSpace($value)")]
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
        [EnumToggleButtons]
        public GEOperation Operation;

        [LabelWidth(100)]
        [AssetSelector]
        [Tooltip("ModifierMagnitudeCalculation，修改器，负责GAS中Attribute的数值计算逻辑。")]
        public ModifierMagnitudeCalculation MMC;

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
            AttributeSetName = split[0];
            AttributeShortName = split[1];
        }
    }
}