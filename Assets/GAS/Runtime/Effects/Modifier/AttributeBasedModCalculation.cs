using Sirenix.OdinInspector;
using UnityEngine;

namespace GAS.Runtime
{
    [CreateAssetMenu(fileName = "AttributeBasedModCalculation", menuName = "GAS/MMC/AttributeBasedModCalculation")]
    public class AttributeBasedModCalculation : ModifierMagnitudeCalculation
    {
        public enum AttributeFrom
        {
            [LabelText("来源(Source)", SdfIconType.Magic)]
            Source,

            [LabelText("目标(Target)", SdfIconType.Person)]
            Target
        }

        public enum GEAttributeCaptureType
        {
            [LabelText("快照(SnapShot)", SdfIconType.Camera)]
            SnapShot,

            [LabelText("实时(Track)", SdfIconType.Speedometer2)]
            Track
        }

        [TabGroup("Default", "AttributeBasedModCalculation", SdfIconType.PersonBoundingBox, TextColor = "blue")]
        [InfoBox(" 以什么方式(Capture Type)从谁身上(Attribute From)捕获哪个属性的值(Attribute Name)。")]
        [EnumToggleButtons]
        [LabelText("捕获方式(Capture Type)")]
        public GEAttributeCaptureType captureType;

        [TabGroup("Default", "AttributeBasedModCalculation")]
        [EnumToggleButtons]
        [LabelText("捕获目标(Attribute From)")]
        public AttributeFrom attributeFromType;

        [TabGroup("Default", "AttributeBasedModCalculation")]
        [ValueDropdown("@ValueDropdownHelper.AttributeChoices", IsUniqueList = true)]
        [LabelText("属性的名称(Attribute Name)")]
        [OnValueChanged("@OnAttributeNameChanged()")]
        [InfoBox("未指定属性名称", InfoMessageType.Error, VisibleIf = "@string.IsNullOrWhiteSpace(attributeName)")]
        public string attributeName;

        [TabGroup("Default", "Details", SdfIconType.Bug, TextColor = "orange")]
        [ReadOnly]
        public string attributeSetName;

        [TabGroup("Default", "Details")]
        [ReadOnly]
        public string attributeShortName;

        [InfoBox("计算逻辑与ScalableFloatModCalculation一致, 公式：AttributeValue * k + b")]
        [TabGroup("Default", "AttributeBasedModCalculation")]
        [LabelText("系数(k)")]
        public float k = 1;

        [TabGroup("Default", "AttributeBasedModCalculation")]
        [LabelText("常量(b)")]
        public float b = 0;

        public override float CalculateMagnitude(GameplayEffectSpec spec, float modifierMagnitude)
        {
            if (attributeFromType == AttributeFrom.Source)
            {
                if (captureType == GEAttributeCaptureType.SnapShot)
                {
                    var snapShot = spec.SnapshotSourceAttributes;
                    var attribute = snapShot[attributeName];
                    return attribute * k + b;
                }
                else
                {
                    var attribute = spec.Source.GetAttributeCurrentValue(attributeSetName, attributeShortName);
                    return (attribute ?? 1) * k + b;
                }
            }

            if (captureType == GEAttributeCaptureType.SnapShot)
            {
                var snapShot = spec.SnapshotTargetAttributes;
                var attribute = snapShot[attributeName];
                return attribute * k + b;
            }
            else
            {
                var attribute = spec.Owner.GetAttributeCurrentValue(attributeSetName, attributeShortName);
                return (attribute ?? 1) * k + b;
            }
        }

        private void OnAttributeNameChanged()
        {
            if (!string.IsNullOrWhiteSpace(attributeName))
            {
                var split = attributeName.Split('.');
                attributeSetName = split[0];
                attributeShortName = split[1];
            }
            else
            {
                attributeSetName = null;
                attributeShortName = null;
            }
        }
    }
}