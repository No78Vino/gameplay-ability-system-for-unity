using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GAS.Runtime
{
    public enum GEOperation
    {
        [LabelText(SdfIconType.PlusLg, Text = "加")]
        Add = 0,

        [LabelText(SdfIconType.DashLg, Text = "减")]
        Minus = 3,

        [LabelText(SdfIconType.XLg, Text = "乘")]
        Multiply = 1,

        [LabelText(SdfIconType.SlashLg, Text = "除")]
        Divide = 4,

        [LabelText(SdfIconType.Pencil, Text = "替")]
        Override = 2,
    }

    [Flags]
    public enum SupportedOperation : byte
    {
        None = 0,

        [LabelText(SdfIconType.PlusLg, Text = "加")]
        Add = 1 << GEOperation.Add,

        [LabelText(SdfIconType.DashLg, Text = "减")]
        Minus = 1 << GEOperation.Minus,

        [LabelText(SdfIconType.XLg, Text = "乘")]
        Multiply = 1 << GEOperation.Multiply,

        [LabelText(SdfIconType.SlashLg, Text = "除")]
        Divide = 1 << GEOperation.Divide,

        [LabelText(SdfIconType.Pencil, Text = "替")]
        Override = 1 << GEOperation.Override,

        All = Add | Minus | Multiply | Divide | Override
    }

    [Serializable]
    public struct GameplayEffectModifier
    {
        private const int LABEL_WIDTH = 70;

        [LabelText("修改属性", SdfIconType.Fingerprint)]
        [LabelWidth(LABEL_WIDTH)]
        [OnValueChanged("OnAttributeChanged")]
        [ValueDropdown("@ValueDropdownHelper.AttributeChoices", IsUniqueList = true)]
        [Tooltip("指的是GameplayEffect作用对象被修改的属性。")]
        [InfoBox("未选择属性", InfoMessageType.Error, VisibleIf = "@string.IsNullOrWhiteSpace($value)")]
        [SuffixLabel("@ReflectionHelper.GetAttribute($value)?.CalculateMode")]
        [PropertyOrder(1)]
        public string AttributeName;

        [HideInInspector]
        public string AttributeSetName;

        [HideInInspector]
        public string AttributeShortName;

        [LabelText("运算参数", SdfIconType.Activity)]
        [LabelWidth(LABEL_WIDTH)]
        [Tooltip("修改器的基础数值。这个数值如何使用由MMC的运行逻辑决定。\nMMC未指定时直接使用这个值。")]
        [InfoBox("除数不能为零", InfoMessageType.Error,
            VisibleIf = "@Operation == GEOperation.Divide && ModiferMagnitude == 0 && MMC == null")]
        [PropertyOrder(3)]
        public float ModiferMagnitude;

        [LabelText("运算法则", SdfIconType.PlusSlashMinus)]
        [LabelWidth(LABEL_WIDTH)]
        [EnumToggleButtons]
        [PropertyOrder(2)]
        [ValidateInput("@ReflectionHelper.GetAttribute(AttributeName).IsSupportOperation($value)", "非法运算: 该属性不支持的此运算法则")]
        public GEOperation Operation;

        [LabelText("参数修饰", SdfIconType.CpuFill)]
        [LabelWidth(LABEL_WIDTH)]
        [AssetSelector]
        [Tooltip("ModifierMagnitudeCalculation，修改器，负责GAS中Attribute的数值计算逻辑。\n可以为空(不对\"计算参数\"做任何修改)。")]
        [PropertyOrder(4)]
        public ModifierMagnitudeCalculation MMC;

        // TODO
        // public readonly GameplayTagSet SourceTag;

        // TODO
        // public readonly GameplayTagSet TargetTag;

        public GameplayEffectModifier(
            string attributeName,
            float modiferMagnitude,
            GEOperation operation,
            ModifierMagnitudeCalculation mmc = null)
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

            if (ReflectionHelper.GetAttribute(AttributeName)?.CalculateMode !=
                CalculateMode.Stacking)
            {
                Operation = GEOperation.Override;
            }
        }
    }
}