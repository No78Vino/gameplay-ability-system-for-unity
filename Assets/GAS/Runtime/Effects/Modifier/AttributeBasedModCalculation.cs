using System.Linq;
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
        [ValidateInput("@AttributeValidator.IsValidAttributeName($value)", "属性名无效")]
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
            float attributeValue;
            if (attributeFromType == AttributeFrom.Source)
            {
                if (captureType == GEAttributeCaptureType.SnapShot)
                {
                    var snapShot = spec.SnapshotSourceAttributes;
                    if (snapShot == null || snapShot.TryGetValue(attributeName, out attributeValue) == false)
                    {
                        Debug.LogError($"Source snapshot Attribute '{attributeName}' not found in source snapshot for spec: '{spec.GameplayEffect.GameplayEffectName}'.");
                        attributeValue = 1;
                    }
                }
                else
                {
                    var attributeCurrentValue = spec.Source.GetAttributeCurrentValue(attributeSetName, attributeShortName);
                    if (attributeCurrentValue == null)
                    {
                        Debug.LogError($"Source Attribute '{attributeName}' not found in source for spec: '{spec.GameplayEffect.GameplayEffectName}'.");
                        attributeValue = 1;
                    }
                    else
                    {
                        attributeValue = attributeCurrentValue.Value;
                    }
                }
            }
            else
            {
                if (captureType == GEAttributeCaptureType.SnapShot)
                {
                    var snapShot = spec.SnapshotTargetAttributes;
                    if (snapShot == null || snapShot.TryGetValue(attributeName, out attributeValue) == false)
                    {
                        Debug.LogError($"Target snapshot Attribute '{attributeName}' not found in target snapshot for spec: '{spec.GameplayEffect.GameplayEffectName}'.");
                        attributeValue = 1;
                    }
                }
                else
                {
                    var attributeCurrentValue = spec.Owner.GetAttributeCurrentValue(attributeSetName, attributeShortName);
                    if (attributeCurrentValue == null)
                    {
                        Debug.LogError($"Source Attribute '{attributeName}' not found in source for spec: '{spec.GameplayEffect.GameplayEffectName}'.");
                        attributeValue = 1;
                    }
                    else
                    {
                        attributeValue = attributeCurrentValue.Value;
                    }
                }
            }

            return attributeValue * k + b;
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